using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Examples.Logging;

namespace Telnet.Client {
  internal static class Program {
    private const string Host = "127.0.0.1";
    private const int Port = 8023;

    private static async Task RunClientAsync() {
      LoggingHelper.SetNLogLogger();

      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap b = new Bootstrap();
        b.Group(group)
          .Channel<TcpSocketChannel>()
          .Handler(new TelnetClientInitializer());

        // Start the connection attempt.
        IChannel ch = await b.ConnectAsync(IPAddress.Parse(Host), Port);

        // Read command from stdin.
        Task lastWriteTask = null;
        for (;;) {
          string line = Console.ReadLine();
          if (line == null) {
            break;
          }

          // Sends the received line to the server.
          lastWriteTask = ch.WriteAndFlushAsync(line + "\r\n");

          // If user typed the 'bye' command, wait until the server closes the connection.
          if ("bye".Equals(line.ToLower())) {
            await ch.CloseCompletion;
            break;
          }
        }

        // Wait until all messages are flushed before closing the channel.
        if (lastWriteTask != null) {
          await lastWriteTask;
        }
      } finally {
        await group.ShutdownGracefullyAsync();
      }
    }

    public static void Main(string[] args) {
      RunClientAsync().Wait();
    }
  }
}
