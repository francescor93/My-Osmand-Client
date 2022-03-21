using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

namespace My_Osmand_Client {

    public sealed partial class MainPage : Page {

        private static bool? enabled = false;
        private static string deviceId = "0";
        private static string serverUrl = "";
        private static string serverPort = "5055";
        private static string precision = "";
        private static string updateFrequency = "60";

        public static bool? Enabled { get => enabled; }
        public static string DeviceId { get => deviceId; }
        public static string ServerUrl { get => serverUrl; }
        public static string ServerPort { get => serverPort; }
        public static string Precision { get => precision; }
        public static string UpdateFrequency { get => updateFrequency; }

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

        private void LoadSettings() {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            try {
                if (localSettings.Values.ContainsKey("enabled")) { enabled = localSettings.Values["enabled"] as bool?; }
                if (localSettings.Values.ContainsKey("deviceId")) { deviceId = localSettings.Values["deviceId"] as string; }
                if (localSettings.Values.ContainsKey("serverUrl")) { serverUrl = localSettings.Values["serverUrl"] as string; }
                if (localSettings.Values.ContainsKey("serverPort")) { serverPort = localSettings.Values["serverPort"] as string; }
                if (localSettings.Values.ContainsKey("precision")) { precision = localSettings.Values["precision"] as string; }
                if (localSettings.Values.ContainsKey("updateFrequency")) { updateFrequency = localSettings.Values["updateFrequency"] as string; }
                if (enabled != true) {
                    WriteInfoMessage("Service is down. Update your settings to activate it.");
                }
            }
            catch {
                WriteInfoMessage("An error has occurred while loading settings");
            }
        }

        private async void SaveSettings(object sender, RoutedEventArgs e) {

            // Write settings to device appdata
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["enabled"] = config_enabled.IsChecked;
            localSettings.Values["deviceId"] = config_deviceId.Text;
            localSettings.Values["serverUrl"] = config_serverUrl.Text;
            localSettings.Values["serverPort"] = config_serverPort.Text;
            localSettings.Values["precision"] = config_precision.Text;
            localSettings.Values["updateFrequency"] = config_updateFrequency.Text;

            // Reload to app instance
            LoadSettings();

            // Restart update timer
            Geolocation geolocationService = new Geolocation();
            await geolocationService.BeginExtendedExecution();

        }

        public void WriteInfoMessage(string message) {
            infoText.Text = message;
        }
    }
}
