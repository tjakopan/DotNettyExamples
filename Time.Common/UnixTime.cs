using System;

namespace Time.Common {
  public class UnixTime {
    public long Ticks { get; }

    public UnixTime() {
      Ticks = DateTime.Now.Ticks;
    }

    public UnixTime(long ticks) {
      Ticks = ticks;
    }

    public override string ToString() {
      return new DateTime(Ticks).ToString();
    }
  }
}
