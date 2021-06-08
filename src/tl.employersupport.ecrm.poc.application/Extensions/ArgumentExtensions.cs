using System;
using System.Globalization;
using System.Linq;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class ArgumentExtensions
    {
        //public static string GetStringFromArgument(this string[] args, string argName)
        //{
        //    return args.GetStringFromArgument(argName, "", null);
        //}

        public static string GetStringFromArgument(this string[] args, string argName, string separator = null, string defaultResult = default)
        {
            var result = defaultResult;
            if (args is { Length: > 0 })
            {
                var s = args.FirstOrDefault(a => a.ToLower().StartsWith(argName.ToLower()));
                if (s != null && s.Length > argName.Length)
                {
                    result = s[argName.Length..].Remove(0, separator?.Length ?? 0);
                }
            }
            return result;
        }

        public static int GetIntFromArgument(this string[] args, string argName, int defaultResult = default)
        {
            return args.GetIntFromArgument(argName, "", defaultResult);
        }

        public static int GetIntFromArgument(this string[] args, string argName, string separator, int defaultResult = default)
        {
            var result = defaultResult;

            if (args is { Length: > 0 })
            {
                var s = args.FirstOrDefault(a => a.ToLower().StartsWith(argName.ToLower()));

                if (s == null || s.Length <= argName.Length) return defaultResult;

                var stringResult = s[argName.Length..].Trim();

                if (!string.IsNullOrWhiteSpace(separator) && stringResult.StartsWith(separator))
                {
                    stringResult = stringResult.Remove(0, separator.Length);
                }

                if (int.TryParse(stringResult, out var extractedInt))
                {
                    return extractedInt;
                }
            }

            return defaultResult;
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
