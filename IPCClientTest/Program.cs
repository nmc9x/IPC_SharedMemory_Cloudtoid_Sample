using IPCSharedMemory;
using System.Text;

namespace IPCClientTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
           using IPCSharedHelper ipc= new("my-queue",isReceiver:true);
           Console.ReadKey();
        }
    }
}
