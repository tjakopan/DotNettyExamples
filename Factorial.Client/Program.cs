using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Factorial.Client {
  internal static class Program {
    private const string Host = "127.0.0.1";
    private const int Port = 8322;
    public const int Count = 1000;

    private static async Task RunClientAsync() {
      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap b = new Bootstrap();
        b.Group(group)
          .Channel<TcpSocketChannel>()
          .Handler(new FactorialClientInitializer());

        // Make a new connection.
        IChannel boostrapChannel = await b.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port));

        // Get the handler instance to retrieve the answer.
        FactorialClientHandler handler = (FactorialClientHandler) boostrapChannel.Pipeline.Last();

        // Print out the answer.
        Console.WriteLine($"Factorial of {Count} is: {handler.GetFactorial()}");
      } finally {
        group.ShutdownGracefullyAsync().Wait(1000);
      }
    }

    public static void Main(string[] args) {
      RunClientAsync().Wait();
    }
  }
}
