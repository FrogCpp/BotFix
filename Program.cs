using System;
using System.Reflection;
using System.Text;
using static BotFix.MainProgram;
using static BotFix.SplitDemo;



namespace BotFix
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            //SplitDemo.Run();

            MainProgram.Run();

            while (true)
            {
                Thread.Sleep(999999);
            }
        }
    }
}