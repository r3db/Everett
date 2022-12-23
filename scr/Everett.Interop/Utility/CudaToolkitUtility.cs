using System;
using System.Collections.Generic;
using System.Globalization;

namespace Everett.Interop
{
    // Todo: This class should not even be here! Should be specific for the generator! This way we can also remove the "InternalsVisibleTo" crap!
    internal static class CudaToolkitUtility
    {
        // Internal Const Data
        private const EnvironmentVariableTarget Target = EnvironmentVariableTarget.Process;

        // Internal Static Data
        private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        // Methods
        internal static void Register(IDictionary<string, List<ModuleResolverMapping>> mappings)
        {
            if (mappings is null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            foreach (var item in mappings)
            {
                if (PlatformUtility.Platform == Platform.Windows)
                {
                    AlterMappingsForWindows(item);
                }

                ModuleResolver.Register(item.Key, item.Value);
            }
        }
        
        private static void AlterMappingsForWindows(KeyValuePair<string, List<ModuleResolverMapping>> item)
        {
            var version = GetVersionForWindows();

            foreach (var mapping in item.Value)
            {
                if (mapping.Name.Contains("{0}"))
                {
                    var versionedName = string.Format(_culture, "{0}{1}", version.Major, version.Minor);
                    mapping.Name = string.Format(_culture, mapping.Name, versionedName);
                }
            }
        }

        // Helpers
        // Todo: There's a better of doing this, by looking at 'CUDA_PATH_V{X}_{Y}'
        private static Version GetVersionForWindows()
        {
            var ev = Environment.GetEnvironmentVariable("CUDA_PATH", Target)?.TrimEnd('\\');

            if (ev is null)
            {
                throw new NotSupportedException("CUDA Toolkit is not installed or environment variable 'CUDA_PATH' not set.");
            }

            var index = ev.LastIndexOf("\\", StringComparison.InvariantCulture);
            var version = ev.Substring(index + 1);

            var supportedVersions = new Dictionary<string, Version>
            {
                { "v7.5",  new Version(7, 5)},
                { "v8.0",  new Version(8, 0)},
                { "v9.0",  new Version(9, 0)},
                { "v10.0", new Version(10, 0)},
                { "v11.0", new Version(11, 0)},
                { "v12.0", new Version(12, 0)},
            };

            if (supportedVersions.ContainsKey(version))
            {
                return supportedVersions[version];
            }

            throw new NotSupportedException(string.Format(_culture,
                "CUDA Toolkit is not installed or version not supported.{0}Supported Versions: {1}",
                Environment.NewLine, string.Join("; ", supportedVersions.Values)));
        }
    }
}