using System;
using System.Globalization;
using System.Linq;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class ArgumentExtensions
    {
        public static string GetStringFromArgument(this string[] args, string argName, string defaultResult = null)
        {
            string result = defaultResult;
            if (args is { Length: > 0 })
            {
                string s = args.FirstOrDefault(a => a.ToLower().StartsWith(argName.ToLower()));
                if (s != null && s.Length > argName.Length)
                {
                    result = s[argName.Length..];
                }
            }
            return result;
        }

        public static int GetIntFromArgument(this string[] args, string argName, int defaultResult = default)
        {
            var result = defaultResult;

            if (args is { Length: > 0 })
            {
                var s = args.FirstOrDefault(a => a.ToLower().StartsWith(argName.ToLower()));

                if (s == null || s.Length <= argName.Length) return result;
                var stringResult = s[argName.Length..];

                if (int.TryParse(stringResult, out var extractedInt))
                {
                    result = extractedInt;
                }
            }
            return result;
        }

        public static bool HasArgument(this string[] args, string argName, bool exactMatch = false)
        {
            if (args == null)
            {
                return false;
            }

            return exactMatch
                ? args.Any(a => string.Compare(a, argName, StringComparison.InvariantCultureIgnoreCase) == 0)
                : args.Any(a => a.StartsWith(argName, true, CultureInfo.InvariantCulture));
        }
    }
}
