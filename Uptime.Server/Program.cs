﻿using System;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Examples.Common;
using NLog;

namespace Uptime.Server {
  internal static class Program {
    private const int Port = 8080;
    private static readonly UptimeServerHandler Handler = new UptimeServerHandler();

    private static async Task RunServerAsync() {
      IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
      IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();
      try {
        ServerBootstrap b = new ServerBootstrap();
        b.Group(bossGroup, workerGroup)
          .Channel<TcpServerSocketChannel>()
          .Handler(new LoggingHandler(LogLevel.Info))
          .ChildHandler(
            new ActionChannelInitializer<ISocketChannel>(channel => channel.Pipeline.AddLast(Handler)));

        IChannel ch = await b.BindAsync(Port);

        Console.ReadLine();

        await ch.CloseAsync();
//        await ch.CloseCompletion;
      } finally {
        Task.WaitAll(workerGroup.ShutdownGracefullyAsync(), bossGroup.ShutdownGracefullyAsync());
      }
    }

    public static void Main(string[] args) {
      RunServerAsync().Wait();
    }
  }
}
