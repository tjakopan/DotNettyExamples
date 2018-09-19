using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;

namespace Uptime.Client {
  internal static class Program {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public const string Host = "127.0.0.1";
    public const int Port = 8080;
    public static readonly TimeSpan ReconnectDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ReadTimeout = TimeSpan.FromSeconds(10);

    private static readonly UptimeClientHandler Handler = new UptimeClientHandler();
    private static readonly Bootstrap Bs = new Bootstrap();

    private static async Task RunClientAsync() {
      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bs.Group(group)
          .Channel<TcpSocketChannel>()
          .RemoteAddress(IPAddress.Parse(Host), Port)
          .Handler(new ActionChannelInitializer<ISocketChannel>(ch =>
            ch.Pipeline.AddLast(new IdleStateHandler(ReadTimeout, TimeSpan.Zero, TimeSpan.Zero), Handler)));

        await Bs.ConnectAsync();

        Console.ReadLine();
      } finally {
        await group.ShutdownGracefullyAsync();
      }
    }

    public static void Main(string[] args) {
      RunClientAsync().Wait();
    }

    public static void Connect() {
      Bs.ConnectAsync().ContinueWith(task => {
        if (task.Exception == null) return;

        Handler.StartTime = -1;
        Logger.Error(task.Exception, "Failed to connect.");
      });
    }
  }
}
