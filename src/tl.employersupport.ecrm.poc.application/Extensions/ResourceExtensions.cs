using System;
using System.IO;
using System.Reflection;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class ResourceExtensions
    {
        public static Stream GetManifestResourceStream(this string resourcePath, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var stream = assembly
                .GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                throw new Exception($"Stream for '{resourcePath}' not found.");
            }
            
            return stream;
        }

        public static string ReadManifestResourceStreamAsString(this string resourcePath, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            using var stream = resourcePath.GetManifestResourceStream(assembly);
            using var stringReader = new StreamReader(stream);
            return stringReader.ReadToEnd();
        }
    }
}
