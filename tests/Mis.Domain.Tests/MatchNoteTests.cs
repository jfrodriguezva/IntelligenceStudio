using Mis.Domain.Football;
using Xunit;

namespace Mis.Domain.Tests;

public sealed class MatchNoteTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(131)]
    public void ConstructorRejectsMinuteOutsideMatchRange(int minute)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(minute: minute));
    }

    [Fact]
    public void ConstructorTrimsTagAndText()
    {
        var note = Create(tag: " PRESSING ", text: " Win the ball high ");

        Assert.Equal("PRESSING", note.Tag);
        Assert.Equal("Win the ball high", note.Text);
    }

    private static MatchNote Create(int minute = 10, string tag = "TACTICAL", string text = "Observation") =>
        new(Guid.NewGuid(), Guid.NewGuid(), minute, tag, text, DateTimeOffset.UtcNow);
}
