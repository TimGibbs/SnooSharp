using AutoFixture;
using FluentAssertions;

namespace SnooSharp.Tests;

public class LevelExtensionsTests
{
    [Fact]
    public void StartAndEndWork()
    {
        const int duration = 300;
        var date = new DateTime(2022, 01, 12, 12, 0, 0);
        var fixture = new Fixture();
        var level = fixture.Build<Level>()
            .With(o => o.startTime,date.ToString(Constants.Formats.LevelDateTimeFormat))
            .With(o=>o.stateDuration,duration)
            .Create();
        var st = level.Start();
        var et = level.End();
        st.Should().Be(date);
        et.Should().Be(date.AddSeconds(duration));

    }
}