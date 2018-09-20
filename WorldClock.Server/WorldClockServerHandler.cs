using System;
using DotNetty.Transport.Channels;
using NLog;
using WorldClock.Common;
using DayOfWeek = WorldClock.Common.DayOfWeek;

namespace WorldClock.Server {
  public class WorldClockServerHandler : SimpleChannelInboundHandler<TimeZoneIds> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    protected override void ChannelRead0(IChannelHandlerContext ctx, TimeZoneIds timeZoneIds) {
      DateTime currentDateTime = DateTime.Now;

      LocalTimes localTimes = new LocalTimes();
      foreach (string timeZoneId in timeZoneIds.TimeZoneIds_) {
        DateTime dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentDateTime, timeZoneId);

        localTimes.LocalTimes_.Add(new LocalTime {
          Year = (uint) dateTime.Year,
          Month = (uint) dateTime.Month,
          DayOfMonth = (uint) dateTime.Day,
          DayOfWeek = (DayOfWeek) dateTime.DayOfWeek,
          Hour = (uint) dateTime.Hour,
          Minute = (uint) dateTime.Minute,
          Second = (uint) dateTime.Second
        });
      }

      ctx.WriteAsync(localTimes);
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) {
      context.Flush();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Exception in server handler.");
      context.CloseAsync();
    }
  }
}
