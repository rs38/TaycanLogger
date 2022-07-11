using System;
using Xunit;
using FluentAssertions;
using System.IO;
using OBDSession;

namespace TaycanLogger.Tests
{
    public class TimestamponverterTests
    {
        [Theory]
        [InlineData("56D8B644", "2021-11-12 11")]
        [InlineData("573d2365", "2021-12-30 18")]
        [InlineData("58f51ddd", "2022-03-26 17")]
        public void Test1(string timestamp, string expectedDate)
        {
            var result = TimestampConverter.ConvertToDateTime(timestamp);
            result.ToString("yyyy-MM-dd HH").Should().Be(expectedDate);
        }
    }
}