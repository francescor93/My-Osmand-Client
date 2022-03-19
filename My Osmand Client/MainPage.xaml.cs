using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

namespace My_Osmand_Client {

    public sealed partial class MainPage : Page {

        private static bool? enabled;
        private static string deviceId;
        private static string serverUrl;
        private static string serverPort;
        private static string precision;
        private static string updateFrequency;

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
                enabled = localSettings.Values["enabled"] as bool?;
                deviceId = localSettings.Values["deviceId"] as string;
                serverUrl = localSettings.Values["serverUrl"] as string;
                serverPort = localSettings.Values["serverPort"] as string;
                precision = localSettings.Values["precision"] as string;
                updateFrequency = localSettings.Values["updateFrequency"] as string;
            }
            catch { }
        }

        private void SaveSettings(object sender, RoutedEventArgs e) {

            // Write settings to device appdata
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["enabled"] = config_enabled.IsChecked;
            localSettings.Values["deviceId"] = config_deviceId.Text;
            localSettings.Values["serverUrl"] = config_serverUrl.Text;
            localSettings.Values["serverPort"] = config_serverPort.Text;
            localSettings.Values["precision"] = config_precision.Text;
            localSettings.Values["updateFrequency"] = config_updateFrequency.Text;

            // And reload to app instance
            LoadSettings();

        }
    }
}
