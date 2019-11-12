using System;
using System.Device.Gpio;

namespace GPIOPIR
{
    class Hcsr501 : IDisposable
    {
        private GpioController _controller;
        private readonly int _outPin;
        public event PinChangeEventHandler pinChangeEvent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pin">OUT Pin</param>
        public Hcsr501(int outPin, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _outPin = outPin;

            _controller = new GpioController(pinNumberingScheme);
            _controller.OpenPin(outPin, PinMode.Input);
            if (pinChangeEvent != null)
                _controller.RegisterCallbackForPinValueChangedEvent(_outPin, PinEventTypes.Falling, pinChangeEvent);
        }

        // 为指定引脚的值改变时注册回调（即上文中提到的 GPIO 中断）
        // PinEventTypes 是值改变的类型，包括上升沿（Rising，0->1）和下降沿（Falling，1->0），注意当设置为 None 时不会触发
        // PinChangeEventHandler 为回调事件

        /// <summary>
        /// 是否检测到人体
        /// </summary>
        public bool IsMotionDetected => _controller.Read(_outPin) == PinValue.High;

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }
    }
}
