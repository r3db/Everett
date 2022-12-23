using System;
using System.Collections.Generic;
using System.IO;

namespace Everett.Interop
{
    // Todo: This class should not even be here! Should be specific for the generator! This way we can also remove the "InternalsVisibleTo" crap!
    internal static class CudaCuDnnUtility
    {
        // Internal Const Data
        private const EnvironmentVariableTarget Target = EnvironmentVariableTarget.Process;

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
            var path = GetCudaCuDnnDllPath();

            foreach (var mapping in item.Value)
            {
                mapping.Name = path;
            }
        }

        // Helpers
        private static string GetCudaCuDnnDllPath()
        {
            var supportedVersions = new List<string>
            {
                "cudnn64_7.dll",
                "cudnn64_6.dll",
                "cudnn64_5.dll",
            };

            var path = GetCudaTookitVersionPath();

            foreach (var item in supportedVersions)
            {
                var fullPath = string.Format(@"{0}\{1}", path, item);

                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            throw new NotSupportedException(string.Format("CuDnn is not installed or could no be found at '{0}'.", path));
        }

        private static string GetCudaTookitVersionPath()
        {
            var ev = Environment.GetEnvironmentVariable("CUDA_PATH", Target)?.TrimEnd('\\');

            if (ev is null)
            {
                throw new NotSupportedException("CUDA Toolkit is not installed or environment variable 'CUDA_PATH' not set.");
            }

            return ev + @"\bin";
        }
    }
}