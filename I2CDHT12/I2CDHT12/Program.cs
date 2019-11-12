using System;
using System.Device.I2c;
using System.Threading;

namespace I2CDHT12
{
    class Program
    {
        static void Main(string[] args)
        {
            I2cConnectionSettings settings = new I2cConnectionSettings(1, DHT12.DefaultI2cAddress);
            I2cDevice device = I2cDevice.Create(settings);

            using DHT12 dht = new DHT12(device);
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"温度: {dht.Temperature.ToString("0.0")} °C, 湿度: {dht.Humidity.ToString("0.0")} %");
                Thread.Sleep(10000);
            }
        }
    }
}
