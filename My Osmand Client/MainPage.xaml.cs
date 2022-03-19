using System;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using Windows.Web.Http;
using Windows.Storage;

namespace My_Osmand_Client {

    public sealed partial class MainPage : Page {

        private static CancellationTokenSource _cts = null;
        private static bool? enabled;
        private static string deviceId;
        private static string serverUrl;
        private static string serverPort;
        private static string precision;
        private static string updateFrequency;

        public MainPage() {

            // Initialize app
            InitializeComponent();

            // Read settings from device appdata
            LoadSettings();

            // Write loaded settings to main window
            config_enabled.IsChecked = enabled;
            config_deviceId.Text = deviceId;
            config_serverUrl.Text = serverUrl;
            config_serverPort.Text = serverPort;
            config_precision.Text = precision;
            config_updateFrequency.Text = updateFrequency;

        }
        private void SaveSettings(object sender, RoutedEventArgs e) {

            // Write settings to device appdata
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["enabled"] = config_enabled.IsChecked;
            localSettings.Values["deviceId"] = config_deviceId.Text;
            localSettings.Values["serverUrl"] = config_serverUrl.Text;
            localSettings.Values["serverPort"] = config_serverPort.Text;
            localSettings.Values["precision"] = config_precision.Text;
            localSettings.Values["updateFrequency"] = config_updateFrequency.Text;

            // And reload to app instance
            LoadSettings();

        }

        private void LoadSettings() {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            try {
                enabled = localSettings.Values["enabled"] as bool?;
                deviceId = localSettings.Values["deviceId"] as string;
                serverUrl = localSettings.Values["serverUrl"] as string;
                serverPort = localSettings.Values["serverPort"] as string;
                precision = localSettings.Values["precision"] as string;
                updateFrequency = localSettings.Values["updateFrequency"] as string;
            }
            catch { }
        }

        public static async void GetLocation() {
            while (true)
            {

                // Get position privacy permission and act accordingly
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    _cts = new CancellationTokenSource();
                    CancellationToken token = _cts.Token;
                    uint desiredAccuracyInMetersValue;
                    uint.TryParse(precision, out desiredAccuracyInMetersValue);
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = desiredAccuracyInMetersValue };
                    Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);
                    UpdateLocationData(pos);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No location permissions");
                }

                // Wait for updateFrequency before exeucting another reading
                int frequencySeconds;
                int.TryParse(updateFrequency, out frequencySeconds);
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
            Uri uri = new Uri(serverUrl + ":" + serverPort + "/?id=" + deviceId + "&timestamp=" + timestamp + "&lat=" + latitude + "&lon=" + longitude + "&accuracy=" + accuracy);

            // Send HTTP request.
            HttpStringContent content = new HttpStringContent("");
            await httpClient.PostAsync(uri, content);

        }
    }
}
