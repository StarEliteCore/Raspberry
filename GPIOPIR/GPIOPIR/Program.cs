using System.Device.Gpio;

namespace GPIOPIR
{
    class Program
    {

        static void Main(string[] args)
        {
            // HC-SR501 OUT Pin
            int hcsr501Pin = 17;
            // 初始化 PIR 传感器
            // LED Pin
            int ledPin = 27;
            Blink blink = new Blink(ledPin);
            using Hcsr501 sensor = new Hcsr501(hcsr501Pin, PinNumberingScheme.Logical);
            sensor.pinChangeEvent += (object sender, PinValueChangedEventArgs pinValueChangedEventArgs) =>
            {
                blink.Flicker(pinValueChangedEventArgs.ChangeType == PinEventTypes.Rising);
            };
        }
    }
}
