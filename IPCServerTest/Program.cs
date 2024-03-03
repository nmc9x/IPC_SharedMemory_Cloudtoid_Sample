
using IPCSharedMemory;
using System.Text;

namespace IPCServerTest
{
    internal class Program
    {
      
        static void Main(string[] args)
        {
            //Thread 1: Send data 0 - 99
            Thread serverThread = new(() =>
            {
                using IPCSharedHelper ipc = new("my-queue");
                uint count = 0;
                byte[] buffer = new byte[32];
                while (count< 100) 
                {
                    byte[] byteConvert =  Encoding.ASCII.GetBytes(count.ToString());
                    buffer = byteConvert;
                    ipc.SendData(buffer);
                    count++;
                    Thread.Sleep(1);
                }
            });
            serverThread.Start();

            // Thread 2: send data 900 - 999
            Thread serverThread2 = new(() =>
            {
                using IPCSharedHelper ipc = new("my-queue");
                uint count = 900;
                byte[] buffer = new byte[32];
                while (count < 1000)
                {
                    byte[] byteConvert = Encoding.ASCII.GetBytes(count.ToString());
                    buffer = byteConvert;
                    ipc.SendData(buffer);
                    count++;
                    Thread.Sleep(1);
                }
            });
            serverThread2.Start();

            Console.ReadKey();
        }
    }
}
