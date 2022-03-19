using System;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using Windows.Web.Http;
using Windows.Storage;

namespace My_Osmand_Client {

    public sealed partial class MainPage : Page {

        private CancellationTokenSource _cts = null;

        public MainPage() {

            // Initialize app
            InitializeComponent();

            // Read settings from device appdata
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            try {
                enabled.IsChecked = localSettings.Values["enabled"] as bool?;
                deviceId.Text = localSettings.Values["deviceId"] as string;
                serverUrl.Text = localSettings.Values["serverUrl"] as string;
                serverPort.Text = localSettings.Values["serverPort"] as string;
                precision.Text = localSettings.Values["precision"] as string;
                updateFrequency.Text = localSettings.Values["updateFrequency"] as string;
            }
            catch { }
        }

        private async void saveSettings(object sender, RoutedEventArgs e) {

            // Write settings to device appdata
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["enabled"] = enabled.IsChecked;
            localSettings.Values["deviceId"] = deviceId.Text;
            localSettings.Values["serverUrl"] = serverUrl.Text;
            localSettings.Values["serverPort"] = serverPort.Text;
            localSettings.Values["precision"] = precision.Text;
            localSettings.Values["updateFrequency"] = updateFrequency.Text;





            // Get position privacy permission and act accordingly
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    _cts = new CancellationTokenSource();
                    CancellationToken token = _cts.Token;
                    uint desiredAccuracyInMetersValue;
                    uint.TryParse(precision.Text, out desiredAccuracyInMetersValue);
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = desiredAccuracyInMetersValue };
                    Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);
                    UpdateLocationData(pos);
                    break;
                case GeolocationAccessStatus.Denied:
                    System.Diagnostics.Debug.WriteLine("Denied");
                    break;
                case GeolocationAccessStatus.Unspecified:
                    System.Diagnostics.Debug.WriteLine("Unspecified");
                    break;
            }





        }

        private async void UpdateLocationData(Geoposition position) {

            // Read GPS data
            String latitude = position.Coordinate.Point.Position.Latitude.ToString().Replace(",", ".");
            String longitude = position.Coordinate.Point.Position.Longitude.ToString().Replace(",", ".");
            String accuracy = position.Coordinate.Accuracy.ToString().Replace(",", ".").Replace(",", ".");

            // Prepare url variables
            String host = serverUrl.Text;
            String port = serverPort.Text;
            String id = deviceId.Text;
            DateTime localDate = DateTime.Now;
            DateTimeOffset dto = new DateTimeOffset(localDate.Year, localDate.Month, localDate.Day, localDate.Hour, localDate.Minute, localDate.Second, TimeSpan.Zero);
            String timestamp = dto.ToUnixTimeSeconds().ToString();

            // Build url
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(host + ":" + port + "/?id=" + id + "&timestamp=" + timestamp + "&lat=" + latitude + "&lon=" + longitude + "&accuracy=" + accuracy);
            System.Diagnostics.Debug.WriteLine(uri);
            // Send HTTP request.
            HttpStringContent content = new HttpStringContent("");
            await httpClient.PostAsync(uri, content);

        }
    }
}
