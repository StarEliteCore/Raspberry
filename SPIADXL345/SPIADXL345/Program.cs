using System;
using System.Device.Spi;
using System.Numerics;
using System.Threading;

namespace SPIADXL345
{
    class Program
    {
        static void Main(string[] args)
        {
            SpiConnectionSettings settings = new SpiConnectionSettings(busId: 0, chipSelectLine: 0)
            {
                ClockFrequency = ADXL345.SpiClockFrequency,
                Mode = ADXL345.SpiMode
            };
            SpiDevice device = SpiDevice.Create(settings);

            using ADXL345 sensor = new ADXL345(device);
            while (true)
            {
                Vector3 data = sensor.Acceleration;

                Console.WriteLine($"X: {data.X.ToString("0.00")} g");
                Console.WriteLine($"Y: {data.Y.ToString("0.00")} g");
                Console.WriteLine($"Z: {data.Z.ToString("0.00")} g");
                Console.WriteLine();

                Thread.Sleep(500);
            }
        }
    }
}
