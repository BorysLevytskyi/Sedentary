using System;
using NUnit.Framework;
using Sedentary.Framework;

namespace Sedentary.Tests
{
    [TestFixture]
    public class BclExtensionsTests
    {
        [Test]
        [TestCase("10:23:11.85", "01:00:00", "10:00:00", TestName = "Round to a start of the hour")]
        [TestCase("10:23:11.85","00:01:00", "10:23:00", TestName = "Round to a start of the minute")]
        [TestCase("10:23:11.85", "00:00:01", "10:23:11", TestName = "Round to a start of the second")]
        public void ShouldRoundToAWindow(string timeStr, string windowStr, string expectedStr)
        {
            TimeSpan time = TimeSpan.Parse(timeStr);
            TimeSpan window = TimeSpan.Parse(windowStr);
            TimeSpan expected = TimeSpan.Parse(expectedStr);

            TimeSpan actual = time.RoundTo(window);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}