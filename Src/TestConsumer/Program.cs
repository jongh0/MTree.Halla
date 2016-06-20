using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var consumer = new TestConsumer();

            Thread.Sleep(1000);
            consumer.StartConsume();

            Console.ReadLine();
            consumer.StopConsume();
        }
    }
}
