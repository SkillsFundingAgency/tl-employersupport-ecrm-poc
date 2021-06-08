using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace tl.employersupport.ecrm.poc.application.tests
{
    public class ArgumentExtensionsTests
    {
        [Fact]
        public void GetStringFromArgumentReturnsExpectedValue()
        {
            var input = new[] { "value:abc", "anothervalue:xyz" };

            input.GetStringFromArgument("value:")
                .Should().Be("abc");
        }

        [Fact]
        public void GetIntFromArgumentReturnsExpectedValue()
        {
            var input = new[] { "value:123", "anothervalue:xyz" };
            input.GetIntFromArgument("value:", -1)
                .Should().Be(123);
        }

        [Fact]
        public void GetIntFromArgumentReturnsDefaultValue()
        {
            var input = new[] { "value:abc", "anothervalue:xyz" };
            const int defaultValue = 2;
            input.GetIntFromArgument("notvalue:", defaultValue)
                .Should().Be(defaultValue);
        }

        [Fact]
        public void GetStringFromArgumentReturnsDefaultValue()
        {
            var input = new[] { "value:abc", "anothervalue:xyz" };
            const string defaultValue = "default";
            input.GetStringFromArgument("notvalue:", defaultValue)
                .Should().Be(defaultValue);
        }

        [Fact]
        public void GetStringFromArgumentReturnsDefaultValueForNullArgs()
        {
            string[] input = null;
            const string defaultValue = "default";
            // ReSharper disable once ExpressionIsAlwaysNull
            input.GetStringFromArgument("notvalue:", defaultValue)
                .Should().Be(defaultValue);
        }

        [Fact]
        public void GetStringFromArgumentReturnsNullDefaultValue()
        {
            var input = new[] { "value:abc", "anothervalue:xyz" };
            input.GetStringFromArgument("notvalue:")
                .Should().BeNull();
        }

        [Fact]
        public void HasArgumentReturnsExpectedResult()
        {
            var input = new[] { "arg1", "arg2" };
            input.HasArgument("arg1").Should().BeTrue("Expected arg1 to be found");
            input.HasArgument("arg2").Should().BeTrue("Expected arg2 to be found");
            input.HasArgument("arg3").Should().BeFalse("Expected arg3 to not be found");
        }

        [Fact]
        public void HasArgumentReturnsExpectedResultForNullArgs()
        {
            ((string[]) null).HasArgument("arg1")
                .Should().BeFalse("Expected arg1 to not be found in null args");
        }

        [Fact]
        public void HasArgumentWithExactMatchReturnsExpectedResult()
        {
            var input = new[] { "matchOFF" };
            input.HasArgument("match").Should().BeTrue("Expected 'match' to be found for non-exact match");
            input.HasArgument("match", true).Should().BeFalse("Expected 'match' to not be found for exact match");
            input.HasArgument("matchOFF", true).Should().BeTrue("Expected 'matchOFF' to be found for exact match");
        }
    }
}
