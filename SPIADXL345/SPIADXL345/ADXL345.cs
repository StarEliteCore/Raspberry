using System;
using System.Buffers.Binary;
using System.Device.Spi;
using System.Numerics;

namespace SPIADXL345
{
    /// <summary>
    /// ADXL345加速度传感器数据手册地址 :http://wenku.baidu.com/view/87a1cf5c312b3169a451a47e.html
    /// </summary>
    class ADXL345 : IDisposable
    {
        #region 寄存器地址
        private const byte ADLX_POWER_CTL = 0x2D;      // 电源控制地址
        private const byte ADLX_DATA_FORMAT = 0x31;     // 范围地址
        private const byte ADLX_X0 = 0x32;              // X轴数据地址
        private const byte ADLX_Y0 = 0x34;              // Y轴数据地址
        private const byte ADLX_Z0 = 0x36;              // Z轴数据地址
        #endregion

        private SpiDevice _sensor = null;

        private readonly int _range = 16;               // 测量范围（-8，8）
        private const int Resolution = 1024;            // 分辨率

        #region SpiSetting
        /// <summary>
        /// ADX1345 SPI 时钟频率
        /// </summary>
        public const int SpiClockFrequency = 5000000;

        /// <summary>
        /// ADX1345 SPI 传输模式
        /// </summary>
        public const SpiMode SpiMode = System.Device.Spi.SpiMode.Mode3;
        #endregion

        /// <summary>
        /// 加速度
        /// </summary>
        public Vector3 Acceleration => ReadAcceleration();

        /// <summary>
        /// 实例化一个 ADX1345
        /// </summary>
        /// <param name="sensor">SpiDevice</param>
        public ADXL345(SpiDevice sensor)
        {
            _sensor = sensor;

            // 设置 ADXL345 测量范围
            // 数据手册 P28，表 21
            Span<byte> dataFormat = stackalloc byte[] { ADLX_DATA_FORMAT, 0b_0000_0010 };
            // 设置 ADXL345 为测量模式
            // 数据手册 P24
            Span<byte> powerControl = stackalloc byte[] { ADLX_POWER_CTL, 0b_0000_1000 };

            _sensor.Write(dataFormat);
            _sensor.Write(powerControl);
        }

        /// <summary>
        /// 读取加速度
        /// </summary>
        /// <returns>加速度</returns>
        private Vector3 ReadAcceleration()
        {
            int units = Resolution / _range;

            // 7 = 1个地址 + 3轴数据（每轴数据2字节）
            Span<byte> writeBuffer = stackalloc byte[7];
            Span<byte> readBuffer = stackalloc byte[7];

            writeBuffer[0] = ADLX_X0;
            _sensor.TransferFullDuplex(writeBuffer, readBuffer);
            Span<byte> readData = readBuffer.Slice(1);      // 切割空白数据

            // 将小端数据转换成正常的数据
            short AccelerationX = BinaryPrimitives.ReadInt16LittleEndian(readData.Slice(0, 2));
            short AccelerationY = BinaryPrimitives.ReadInt16LittleEndian(readData.Slice(2, 2));
            short AccelerationZ = BinaryPrimitives.ReadInt16LittleEndian(readData.Slice(4, 2));

            Vector3 accel = new Vector3
            {
                X = (float)AccelerationX / units,
                Y = (float)AccelerationY / units,
                Z = (float)AccelerationZ / units
            };

            return accel;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _sensor?.Dispose();
            _sensor = null;
        }
    }
}
