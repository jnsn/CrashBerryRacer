using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaceGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DeviceManager _deviceManager;
        private GameEngine _gameEngine;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _deviceManager = new DeviceManager();
            _gameEngine = new GameEngine(_deviceManager);

            await _deviceManager.SetupSensors();

            while (true)
            {
                await _gameEngine.Run();
            }
        }
    }
}