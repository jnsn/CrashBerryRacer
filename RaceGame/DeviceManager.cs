using System.Threading.Tasks;
using SenseHatApi;

namespace RaceGame
{
    public class DeviceManager
    {
        public DeviceManager()
        {
            LedHat = new LedHat();
            SensorHat = new SensorHat();
        }

        public LedHat LedHat { get; }
        public SensorHat SensorHat { get; }

        public async Task SetupSensors()
        {
            await LedHat.InitHardware();
            SensorHat.InitHardware();
        }
    }
}