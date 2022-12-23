using System;
using System.Collections.Generic;

namespace Everett.Interop
{
    internal sealed class CudaToolkitEnvironmentVariableSetter : IDisposable
    {
        // Internal Const Data
        private const EnvironmentVariableTarget Target = EnvironmentVariableTarget.Process;

        // Internal Static Data
        private readonly string[] _availableKeys = {
            "CUDA_PATH",
            "CUDA_PATH_V7_5",
            "CUDA_PATH_V8_0",
            "CUDA_PATH_V9_0",
            "CUDA_PATH_V10_0",
            "CUDA_PATH_V11_0",
            "CUDA_PATH_V12_0",
        };

        // Methods
        internal CudaToolkitEnvironmentVariableSetter()
            : this(GetMachineWideVersion(), GetMachineWidePath())
        {
        }

        internal CudaToolkitEnvironmentVariableSetter(Version version, string path)
        {
            foreach (var item in _availableKeys)
            {
                Environment.SetEnvironmentVariable(item, null, Target);
            }

            switch (PlatformUtility.Platform)
            {
                case Platform.Mac:  throw new NotImplementedException();
                case Platform.Unix: throw new NotImplementedException();
                case Platform.Windows:
                {
                    Environment.SetEnvironmentVariable("CUDA_PATH", path, Target);
                    Environment.SetEnvironmentVariable(string.Format("CUDA_PATH_V{0}_{1}", version.Major, version.Minor), path, Target);
                    break;
                }
            }
        }

        public void Dispose()
        {
        }

        // Helpers
        private static Version GetMachineWideVersion()
        {
            switch (PlatformUtility.Platform)
            {
                case Platform.Mac:     throw new NotImplementedException();
                case Platform.Unix:    throw new NotImplementedException();
                case Platform.Windows: return GetMachineWideVersionForWindows();
            }

            throw new PlatformNotSupportedException();
        }

        private static Version GetMachineWideVersionForWindows()
        {
            var ev = Environment.GetEnvironmentVariable("CUDA_PATH", EnvironmentVariableTarget.Machine)?.TrimEnd('\\');

            if (ev == null)
            {
                throw new NotSupportedException("CUDA Toolkit is not installed or environment variable 'CUDA_PATH' not set.");
            }

            var index = ev.LastIndexOf("\\", StringComparison.InvariantCulture);
            var version = ev.Substring(index + 1);

            var supportedVersions = new Dictionary<string, Version>
            {
                {"v7.5",  new Version( 7, 5)},
                {"v8.0",  new Version( 8, 0)},
                {"v9.0",  new Version( 9, 0)},
                {"v10.0", new Version(10, 0)},
                {"v11.0", new Version(11, 0)},
                {"v12.0", new Version(12, 0)},
            };

            if (supportedVersions.ContainsKey(version))
            {
                return supportedVersions[version];
            }

            throw new NotSupportedException(string.Format("CUDA Toolkit is not installed or version not supported.{0}Supported Versions: {1}", Environment.NewLine, string.Join("; ", supportedVersions.Values)));
        }
        
        private static string GetMachineWidePath()
        {
            switch (PlatformUtility.Platform)
            {
                case Platform.Mac:     throw new NotImplementedException();
                case Platform.Unix:    throw new NotImplementedException();
                case Platform.Windows: return GetMachineWidePathForWindows();
            }

            throw new PlatformNotSupportedException();
        }

        private static string GetMachineWidePathForWindows()
        {
            var path = Environment.GetEnvironmentVariable("CUDA_PATH", EnvironmentVariableTarget.Machine);

            if (path == null)
            {
                throw new NotSupportedException("CUDA Toolkit not installed or version not supported.");
            }

            return path;
        }
    }
}