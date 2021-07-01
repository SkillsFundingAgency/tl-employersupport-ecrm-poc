using FluentAssertions;
using tl.employersupport.ecrm.poc.application.Extensions;
using Xunit;

namespace tl.employersupport.ecrm.poc.application.unittests.Extensions
{
    public class NameSplitterExtensionsTests
    {
        [Theory(DisplayName = nameof(NameSplitterExtensions) + " Data Tests")]
        [InlineData("Bob Bobs", "Bob", "Bobs")]
        [InlineData("Bob", "Bob", "")]
        [InlineData("Corporal Kevin Swells", "Kevin", "Swells")]
        [InlineData("Mrs. Joan Johnson", "Joan", "Johnson")]
        public void NameSplitterExtensions_SplitName_Returns_Expected_Results(string input, string first, string last)
        {
            var result = input.SplitName();
            result.First.Should().Be(first);
            result.Last.Should().Be(last);
        }
    }
}
