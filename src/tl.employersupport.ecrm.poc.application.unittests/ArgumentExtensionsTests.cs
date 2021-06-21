using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace tl.employersupport.ecrm.poc.application.unittests
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
        public void GetStringFromArgumentWithSeparatorReturnsExpectedValue()
        {
            var input = new[] { "value:abc", "separatedValue:xyz" };
            input.GetStringFromArgument("separatedValue", ":")
                .Should().Be("xyz");
        }

        [Fact]
        public void GetStringFromArgumentReturnsDefaultValue()
        {
            var input = new[] { "value:abc", "anothervalue:xyz" };
            const string defaultValue = "default";
            input.GetStringFromArgument("notvalue:", defaultResult: defaultValue)
                .Should().Be(defaultValue);
        }

        [Fact]
        public void GetStringFromArgumentReturnsDefaultValueForNullArgs()
        {
            string[] input = null;
            const string defaultValue = "default";
            // ReSharper disable once ExpressionIsAlwaysNull
            input.GetStringFromArgument("notvalue:", defaultResult: defaultValue)
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
        public void GetIntFromArgumentReturnsExpectedValue()
        {
            var input = new[] { "value:123", "anothervalue:xyz" };
            input.GetIntFromArgument("value:", -1)
                .Should().Be(123);
        }

        [Fact]
        public void GetIntFromArgumentWithSeparatorReturnsExpectedValue()
        {
            var input = new[] { "nothing_here", "ticketId:4474" };
            input.GetIntFromArgument("ticketId", ":", 1)
                .Should().Be(4474);
        }

        [Fact]
        public void GetIntFromArgumentReturnsDefaultValue()
        {
            var input = new[] { "value:abc", "anothervalue:xyz" };
            const int defaultValue = 2;
            input.GetIntFromArgument("notvalue:", defaultResult: defaultValue)
                .Should().Be(defaultValue);
        }
        
        [Fact]
        public void GetLongFromArgumentReturnsExpectedValue()
        {
            var input = new[] { "value:123", "anothervalue:xyz" };
            input.GetLongFromArgument("value:", -1)
                .Should().Be(123L);
        }

        [Fact]
        public void GetLongFromArgumentWithSeparatorReturnsExpectedValue()
        {
            var input = new[] { "nothing_here", "ticketId:4474567890" };
            input.GetLongFromArgument("ticketId", ":", 1)
                .Should().Be(4474567890L);
        }

        [Fact]
        public void GetLongFromArgumentReturnsDefaultValue()
        {
            var input = new[] { "value:abc", "anothervalue:xyz" };
            const int defaultValue = 2;
            input.GetLongFromArgument("notvalue:", defaultResult: defaultValue)
                .Should().Be(defaultValue);
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
