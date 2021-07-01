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
        [InlineData("", "", "")]
        [InlineData(null, "", "")]
        public void NameSplitterExtensions_SplitName_Returns_Expected_Results(string input, string expectedFirstName, string expectedLastName)
        {
            var (firstName, lastName) = 
                input.SplitName();

            firstName.Should().Be(expectedFirstName);
            lastName.Should().Be(expectedLastName);
        }
    }
}
