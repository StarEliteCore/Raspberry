using System.Device.Gpio;
using System.Threading;

namespace GPIOBlink
{
    public class Blink
    {
        public static void Flicker(int delay)
        {
            // 定义引脚
            int pinNumber = 17;
            // 获取 GPIO 控制器
            using GpioController controller = new GpioController(PinNumberingScheme.Logical);
            // 打开引脚 17
            controller.OpenPin(pinNumber, PinMode.Output);
            for (int i = 0; i < 40; i++)
            { 
                // 等待 1s
                Thread.Sleep(delay);
                // 打开 LED
                controller.Write(pinNumber, PinValue.High);
                // 定义延迟时间
                // 等待 1s
                Thread.Sleep(delay);
                // 关闭 LED
                controller.Write(pinNumber, PinValue.Low);
            }
        }
    }
}
