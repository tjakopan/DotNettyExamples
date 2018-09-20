using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DotNetty.Transport.Channels;
using NLog;
using WorldClock.Common;

namespace WorldClock.Client {
  public class WorldClockClientHandler : SimpleChannelInboundHandler<LocalTimes> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // Stateful properties.
    private volatile IChannel _channel;
    private readonly BlockingCollection<LocalTimes> _answer = new BlockingCollection<LocalTimes>();

    public WorldClockClientHandler() : base(false) {
    }

    public IList<string> GetLocalTimes(IEnumerable<string> timeZoneIdsEnumerable) {
      TimeZoneIds timeZoneIds = new TimeZoneIds();

      foreach (string timeZoneId in timeZoneIdsEnumerable) {
        timeZoneIds.TimeZoneIds_.Add(timeZoneId);
      }

      _channel.WriteAndFlushAsync(timeZoneIds);

      LocalTimes localTimes = _answer.Take();

      IList<string> result = new List<string>();
      foreach (LocalTime lt in localTimes.LocalTimes_) {
        result.Add($"{lt.Year}-{lt.Month}-{lt.DayOfMonth} {lt.Hour}:{lt.Minute}:{lt.Second} {lt.DayOfWeek}");
      }

      return result;
    }

    public override void ChannelRegistered(IChannelHandlerContext context) {
      _channel = context.Channel;
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, LocalTimes localTimes) {
      _answer.Add(localTimes);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Exception in client handler.");
      context.CloseAsync();
    }
  }
}
