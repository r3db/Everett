using System;

namespace Everett.Interop
{
    internal static class Mock
    {
        // Methods
        internal static PlatformSetter Platform(PlatformID platform)
        {
            return new PlatformSetter(GetPlatform(platform));
        }

        internal static ArchitectureSetter Architecture(ProcessorArchitecture architecture)
        {
            return new ArchitectureSetter(architecture);
        }

        internal static CudaToolkitEnvironmentVariableSetter CudaToolkitEnvironmentVariable(Version version, string path)
        {
            return new CudaToolkitEnvironmentVariableSetter(version, path);
        }

        internal static CudaToolkitEnvironmentVariableSetter DefaultCudaToolkitEnvironmentVariable()
        {
            return new CudaToolkitEnvironmentVariableSetter();
        }

        // Helpers
        private static Platform GetPlatform(PlatformID platform)
        {
            switch (platform)
            {
                case PlatformID.Unix:
                {
                    return Interop.Platform.Unix;
                }
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                {
                    return Interop.Platform.Windows;
                }
                case PlatformID.MacOSX:
                {
                    return Interop.Platform.Mac;
                }
                default:
                {
                    throw new DllNotFoundException();
                }
            }
        }
    }
}