using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var publisher = new Publisher();

            Thread.Sleep(1000);
            publisher.StartPublising();

            Console.ReadLine();

            publisher.StopPublishing();
        }
    }
}
