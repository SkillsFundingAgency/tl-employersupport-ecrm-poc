using NameParser;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class NameSplitterExtensions
    {
        public static (string First, string Last) SplitName(this string name)
        {
            var nameParser = new HumanName(name);
            return (nameParser.First, nameParser.Last);
        }
    }
}
