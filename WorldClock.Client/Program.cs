using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Examples.Logging;

namespace WorldClock.Client {
  internal static class Program {
    private const string Host = "127.0.0.1";
    private const int Port = 8463;

    private static readonly IList<string> TimeZoneIds = new List<string> {
      "SE Asia Standard Time",
      "Central Europe Standard Time",
      "Pacific Standard Time"
    };

    private static async Task RunClientAsync() {
      LoggingHelper.SetNLogLogger();

      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap b = new Bootstrap();
        b.Group(group)
          .Channel<TcpSocketChannel>()
          .Handler(new WorldClockClientInitializer());

        // Make a new connection.
        IChannel ch = await b.ConnectAsync(IPAddress.Parse(Host), Port);

        // Get the handler instance to initiate request.
        WorldClockClientHandler handler = ch.Pipeline.Get<WorldClockClientHandler>();

        // Request and get the response.
        IList<string> response = handler.GetLocalTimes(TimeZoneIds);

        // Close the connection.
        await ch.CloseAsync();

        // Print the response at last but not least.
        for (int i = 0; i < TimeZoneIds.Count; i++) {
          Console.WriteLine($"{TimeZoneIds[i]}: {response[i]}");
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
