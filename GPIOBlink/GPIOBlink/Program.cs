using System;

namespace GPIOBlink
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Blink.Flicker(0x3E8);
        }
    }
}
