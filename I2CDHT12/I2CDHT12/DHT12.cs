using System;
using System.Device.I2c;

namespace I2CDHT12
{
    /// <summary>
    /// DHT12温湿度传感器数据手册地址: https://wenku.baidu.com/view/325b7096eff9aef8941e06f9.html
    /// </summary>
    class DHT12 : IDisposable
    {
        /// <summary>
        /// DHT12 默认 I2C 地址
        /// </summary>
        public const byte DefaultI2cAddress = 0x5C;    // 若数据手册中给的是8位的I2C地址要记得右移1位

        private I2cDevice _sensor;

        private double _temperature;
        /// <summary>
        /// DHT12 温度
        /// </summary>
        public double Temperature
        {
            get
            {
                ReadData();
                return _temperature;
            }
        }

        private double _humidity;
        /// <summary>
        /// DHT12 湿度
        /// </summary>
        public double Humidity
        {
            get
            {
                ReadData();
                return _humidity;
            }
        }

        /// <summary>
        /// 实例化一个 DHT12 对象
        /// </summary>
        /// <param name="sensor">I2C Device</param>
        public DHT12(I2cDevice sensor)
        {
            _sensor = sensor;
        }

        private void ReadData()
        {
            Span<byte> readBuff = stackalloc byte[5];

            // 数据手册第三页提供了寄存器地址表

            // DHT12 湿度寄存器地址
            _sensor.WriteByte(0x00);
            // 连续读取数据
            // 湿度整数位，湿度小数位，温度整数位，温度小数位，校验和
            _sensor.Read(readBuff);

            // 校验数据，校验方法见数据手册第五页
            // 校验位=湿度高位+湿度低位+温度高位+温度低位
            if ((readBuff[4] == ((readBuff[0] + readBuff[1] + readBuff[2] + readBuff[3]) & 0xFF)))
            {
                // 温度小数位的范围在0-9，所以与上0x7F即可
                double temp = readBuff[2] + (readBuff[3] & 0x7F) * 0.1;
                // 温度小数位第8个bit为1则表示采样得出的温度为负温
                temp = (readBuff[3] & 0x80) == 0 ? temp : -temp;

                double humi = readBuff[0] + readBuff[1] * 0.1;

                _temperature = temp;
                _humidity = humi;
            }
            else
            {
                _temperature = double.NaN;
                _humidity = double.NaN;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sensor.Dispose();
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DHT12()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
