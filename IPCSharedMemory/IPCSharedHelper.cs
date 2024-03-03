using Cloudtoid.Interprocess;
using System.Collections.Concurrent;
using System.Text;

namespace IPCSharedMemory
{
    public class IPCSharedHelper: IDisposable
    {
        readonly QueueFactory _factory;
        readonly QueueOptions _options;
        readonly string _queueName;
        readonly long _capacity;
        readonly int _messageBuffer;
        readonly IPublisher _publisher;

        public byte[]? ReceiveData;
        public ConcurrentQueue<byte[]> MessageQueue = new();

        public IPCSharedHelper(string queueName, long capacity = 1024*1024, int messageBuffer=100, bool isReceiver=false)
        {
            _queueName = queueName;
            _capacity = capacity;
            _messageBuffer = messageBuffer;
            _factory = new();
            _options = new(queueName: _queueName, capacity: _capacity);
            _publisher = _factory.CreatePublisher(_options);

            // Only use for receive data purpose
            if (isReceiver )
            {
                ExcuteMesageQueue();
                ListenReceiveData();
            }
        }
  
        public void SendData(byte[] data)
        {
            if (_publisher.TryEnqueue(data))
            {
                Console.WriteLine("Enqueue "
                    + Encoding.ASCII.GetString(data));
            }
        }

        public void ListenReceiveData()
        {
            Thread listenDataThread = new(() =>
            {
                using ISubscriber subscriber = _factory.CreateSubscriber(_options);
                while (true)
                {
                    byte[] messageBuffer = new byte[_messageBuffer];
                        if (subscriber.TryDequeue(messageBuffer, default, out ReadOnlyMemory<byte> message))
                        {
                            MessageQueue.Enqueue(message.ToArray());
                        }
                    Thread.Sleep(1);
                    //Thread.Yield();
                }
            })
            {
                IsBackground = true,
            };
            listenDataThread.Start();
        }

        public void ExcuteMesageQueue()
        {
            Thread excuteMessageThread = new(() =>
            {
                while (true)
                {
                        if (!MessageQueue.IsEmpty)
                        {
                            MessageQueue.TryDequeue(out byte[]? result);
                            if (result != null)
                            {
                                ReceiveData = (byte[])result.Clone();
                                Console.WriteLine("Message Dequeue "
                                    + Encoding.ASCII.GetString(ReceiveData));
                            }
                        }
                    Thread.Sleep(1);
                }
            })
            {
                IsBackground= true
            };
            excuteMessageThread.Start();
        }

        public void Dispose()
        {
            _publisher?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
