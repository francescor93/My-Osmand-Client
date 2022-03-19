using System;
using System.Threading;
using Windows.Web.Http;
using Windows.Devices.Geolocation;

namespace My_Osmand_Client {
    internal class Geolocation {

        private static CancellationTokenSource _cts = null;

        public static async void GetLocation() {

            // Loop indefinitely
            while (true) {

                // If location sending is enabled
                if ((bool)MainPage.Enabled) {

                    // Get position privacy permission
                    var accessStatus = await Geolocator.RequestAccessAsync();

                    // If use of the location is permitted, please read it and submit it
                    if (accessStatus == GeolocationAccessStatus.Allowed) {
                        _cts = new CancellationTokenSource();
                        CancellationToken token = _cts.Token;
                        uint.TryParse(MainPage.Precision, out uint desiredAccuracyInMetersValue);
                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = desiredAccuracyInMetersValue };
                        Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);
                        UpdateLocationData(pos);
                    }

                    // Else print a message
                    else {
                        System.Diagnostics.Debug.WriteLine("No location permissions");
                    }
                }

                // Wait for updateFrequency seconds before taking another reading
                int.TryParse(MainPage.UpdateFrequency, out int frequencySeconds);
                await System.Threading.Tasks.Task.Delay(frequencySeconds * 1000);
            }
        }

        private static async void UpdateLocationData(Geoposition position) {

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
