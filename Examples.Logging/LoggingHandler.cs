using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using NLog;

namespace Examples.Common {
  public class LoggingHandler : ChannelHandlerAdapter {
    private static readonly LogLevel DefaultLevel = LogLevel.Debug;
    private readonly Logger _logger;
    private readonly LogLevel _level;

    public override bool IsSharable => true;

    public LoggingHandler() : this(DefaultLevel) {
    }

    public LoggingHandler(LogLevel level) : this(typeof(LoggingHandler), level) {
    }

    public LoggingHandler(Type type) : this(type, DefaultLevel) {
    }

    public LoggingHandler(Type type, LogLevel level) {
      if (type == null) {
        throw new NullReferenceException(nameof(type));
      }

      _logger = LogManager.GetLogger(type.FullName);
      _level = level;
    }

    public override void ChannelRegistered(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "REGISTERED"));
      }

      context.FireChannelRegistered();
    }

    public override void ChannelUnregistered(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "UNREGISTERED"));
      }

      context.FireChannelUnregistered();
    }

    public override void ChannelActive(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "ACTIVE"));
      }

      context.FireChannelActive();
    }

    public override void ChannelInactive(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "INACTIVE"));
      }

      context.FireChannelInactive();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, Format(context, "EXCEPTION", exception), exception));
      }

      context.FireExceptionCaught(exception);
    }

    public override void UserEventTriggered(IChannelHandlerContext context, object evt) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "USER_EVENT", evt));
      }

      context.FireUserEventTriggered(evt);
    }

    public override Task BindAsync(IChannelHandlerContext context, EndPoint localAddress) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "BIND", localAddress));
      }

      return context.BindAsync(localAddress);
    }

    public override Task ConnectAsync(IChannelHandlerContext context, EndPoint remoteAddress, EndPoint localAddress) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "CONNECT", remoteAddress, localAddress));
      }

      return context.ConnectAsync(remoteAddress, localAddress);
    }

    public override Task DisconnectAsync(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "DISCONNECT"));
      }

      return context.DisconnectAsync();
    }

    public override Task CloseAsync(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "CLOSE"));
      }

      return context.CloseAsync();
    }

    public override Task DeregisterAsync(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "DEREGISTER"));
      }

      return context.DeregisterAsync();
    }

    public override void ChannelRead(IChannelHandlerContext context, object message) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "RECEIVED", message));
      }

      context.FireChannelRead(message);
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "RECEIVED_COMPLETE"));
      }

      context.FireChannelReadComplete();
    }

    public override void ChannelWritabilityChanged(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "WRITABILITY", context.Channel.IsWritable));
      }

      context.FireChannelWritabilityChanged();
    }

    public override void HandlerAdded(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "HANDLER_ADDED"));
      }
    }

    public override void HandlerRemoved(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "HANDLER_REMOVED"));
      }
    }

    public override void Read(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "READ"));
      }

      context.Read();
    }

    public override Task WriteAsync(IChannelHandlerContext context, object message) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "WRITE"));
      }

      return context.WriteAsync(message);
    }

    public override void Flush(IChannelHandlerContext context) {
      if (_logger.IsEnabled(_level)) {
        _logger.Log(_level, Format(context, "FLUSH"));
      }

      context.Flush();
    }

    protected virtual string Format(IChannelHandlerContext ctx, string eventName) {
      string str = ctx.Channel.ToString();
      return new StringBuilder(str.Length + 1 + eventName.Length).Append(str).Append(' ').Append(eventName).ToString();
    }

    protected virtual string Format(IChannelHandlerContext ctx, string eventName, object arg) {
      switch (arg) {
        case IByteBuffer buffer:
          return FormatByteBuffer(ctx, eventName, buffer);
        case IByteBufferHolder holder:
          return FormatByteBufferHolder(ctx, eventName, holder);
      }

      return FormatSimple(ctx, eventName, arg);
    }

    protected virtual string Format(IChannelHandlerContext ctx, string eventName, object firstArg, object secondArg) {
      if (secondArg == null)
        return FormatSimple(ctx, eventName, firstArg);
      string str1 = ctx.Channel.ToString();
      string str2 = firstArg.ToString();
      string str3 = secondArg.ToString();
      StringBuilder stringBuilder =
        new StringBuilder(str1.Length + 1 + eventName.Length + 2 + str2.Length + 2 + str3.Length);
      stringBuilder.Append(str1).Append(' ').Append(eventName).Append(": ").Append(str2).Append(", ").Append(str3);
      return stringBuilder.ToString();
    }

    private string FormatByteBuffer(IChannelHandlerContext ctx, string eventName, IByteBuffer msg) {
      string str = ctx.Channel.ToString();
      int readableBytes = msg.ReadableBytes;
      if (readableBytes == 0) {
        StringBuilder stringBuilder = new StringBuilder(str.Length + 1 + eventName.Length + 4);
        stringBuilder.Append(str).Append(' ').Append(eventName).Append(": 0B");
        return stringBuilder.ToString();
      }

      int num = readableBytes / 16 + (readableBytes % 15 == 0 ? 0 : 1) + 4;
      StringBuilder dump = new StringBuilder(str.Length + 1 + eventName.Length + 2 + 10 + 1 + 2 + num * 80);
      dump.Append(str).Append(' ').Append(eventName).Append(": ").Append(readableBytes).Append('B').Append('\n');
      ByteBufferUtil.AppendPrettyHexDump(dump, msg);
      return dump.ToString();
    }

    private string FormatByteBufferHolder(IChannelHandlerContext ctx, string eventName, IByteBufferHolder msg) {
      string str1 = ctx.Channel.ToString();
      string str2 = msg.ToString();
      IByteBuffer content = msg.Content;
      int readableBytes = content.ReadableBytes;
      if (readableBytes == 0) {
        StringBuilder stringBuilder = new StringBuilder(str1.Length + 1 + eventName.Length + 2 + str2.Length + 4);
        stringBuilder.Append(str1).Append(' ').Append(eventName).Append(", ").Append(str2).Append(", 0B");
        return stringBuilder.ToString();
      }

      int num = readableBytes / 16 + (readableBytes % 15 == 0 ? 0 : 1) + 4;
      StringBuilder dump =
        new StringBuilder(str1.Length + 1 + eventName.Length + 2 + str2.Length + 2 + 10 + 1 + 2 + num * 80);
      dump.Append(str1).Append(' ').Append(eventName).Append(": ").Append(str2).Append(", ").Append(readableBytes)
        .Append('B').Append('\n');
      ByteBufferUtil.AppendPrettyHexDump(dump, content);
      return dump.ToString();
    }

    private string FormatSimple(IChannelHandlerContext ctx, string eventName, object msg) {
      string str1 = ctx.Channel.ToString();
      string str2 = msg.ToString();
      return new StringBuilder(str1.Length + 1 + eventName.Length + 2 + str2.Length).Append(str1).Append(' ')
        .Append(eventName).Append(": ").Append(str2).ToString();
    }
  }
}
