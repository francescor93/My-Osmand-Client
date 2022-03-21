using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Devices.Geolocation;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI.Core;

namespace My_Osmand_Client {
    internal class Geolocation {

        private Timer periodicTimer = null;
        private ExtendedExecutionSession session = null;

        private void ClearExtendedExecution() {
            if (session != null) {
                session.Revoked -= SessionRevoked;
                session.Dispose();
                session = null;
            }

            if (periodicTimer != null) {
                periodicTimer.Dispose();
                periodicTimer = null;
            }
        }

        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args) {
            /*await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Reason)
                {
                    case ExtendedExecutionRevokedReason.Resumed:
                        rootPage.NotifyUser("Extended execution revoked due to returning to foreground.", NotifyType.StatusMessage);
                        break;

                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        rootPage.NotifyUser("Extended execution revoked due to system policy.", NotifyType.StatusMessage);
                        break;
                }

                EndExtendedExecution();
            });*/
            System.Diagnostics.Debug.WriteLine("Session revoked");
        }

        private async Task<Geolocator> StartLocationTrackingAsync() {

            Geolocator geolocator = null;

            // Get position privacy permission
            var accessStatus = await Geolocator.RequestAccessAsync();

            // If use of the location is permitted, create a new Geolocator instance to return
            if (accessStatus == GeolocationAccessStatus.Allowed) {
                uint.TryParse(MainPage.Precision, out uint desiredAccuracyInMetersValue);
                geolocator = new Geolocator { ReportInterval = 2000, DesiredAccuracyInMeters = desiredAccuracyInMetersValue };
            }

            // Else print a message
            else {
                System.Diagnostics.Debug.WriteLine("No location permissions");
            }

            return geolocator;
        }

        private async void OnTimer(object state) {
            var geolocator = (Geolocator)state;
            if (geolocator == null) {
                System.Diagnostics.Debug.WriteLine("No geolocator");
            }
            else {
                Geoposition geoposition = await geolocator.GetGeopositionAsync();
                if (geoposition == null) {
                    System.Diagnostics.Debug.WriteLine("Cannot get current location");
                }
                else {
                    UpdateLocationData(geoposition);
                }
            }
        }

        public async Task BeginExtendedExecution() {

            ClearExtendedExecution();

            if ((bool)MainPage.Enabled) {
                var newSession = new ExtendedExecutionSession {
                    Reason = ExtendedExecutionReason.LocationTracking,
                    Description = "Tracking your location"
                };
                newSession.Revoked += SessionRevoked;
                ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

                if (result == ExtendedExecutionResult.Allowed) {
                    session = newSession;
                    Geolocator geolocator = await StartLocationTrackingAsync();
                    int.TryParse(MainPage.UpdateFrequency, out int frequencySeconds);
                    periodicTimer = new Timer(OnTimer, geolocator, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(frequencySeconds));
                }
                else {
                    newSession.Dispose();
                }
            }
        }

        private async void UpdateLocationData(Geoposition position) {

            // Read GPS data
            String latitude = position.Coordinate.Point.Position.Latitude.ToString().Replace(",", ".");
            String longitude = position.Coordinate.Point.Position.Longitude.ToString().Replace(",", ".");
            String accuracy = position.Coordinate.Accuracy.ToString().Replace(",", ".").Replace(",", ".");

            // Calculate current unix timestamp
            DateTime localDate = DateTime.Now;
            DateTimeOffset dto = new DateTimeOffset(localDate.Year, localDate.Month, localDate.Day, localDate.Hour, localDate.Minute, localDate.Second, TimeSpan.Zero);
            String timestamp = dto.ToUnixTimeSeconds().ToString();

            // Build url
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(MainPage.ServerUrl + ":" + MainPage.ServerPort + "/?id=" + MainPage.DeviceId + "&timestamp=" + timestamp + "&lat=" + latitude + "&lon=" + longitude + "&accuracy=" + accuracy);

            // Send HTTP request.
            HttpStringContent content = new HttpStringContent("");
            await httpClient.PostAsync(uri, content);
        }
    }
}
