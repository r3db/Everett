using System;
using System.Runtime.InteropServices;

namespace Everett.Interop
{
    internal static class PlatformUtility
    {
        // Internal Static Data
        // Note: This field is not readonly because of... testing... :S
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static bool _is64BitProcess = Environment.Is64BitProcess;
        // Note: This field is not readonly because of... testing... :S
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Platform _platform = GetPlatform();

        // Methods
        // Note: We don't convert this into an auto-property because we want to fake it!
        // ReSharper disable once ConvertToAutoProperty
        internal static Platform Platform => _platform;

        internal static ProcessorArchitecture Architecture => _is64BitProcess
            ? ProcessorArchitecture.X64
            : ProcessorArchitecture.X86;

        // Helpers
        private static Platform GetPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                {
                    return GetUnixVariant();
                }
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                {
                    return Platform.Windows;
                }
                case PlatformID.MacOSX:
                {
                    return Platform.Mac;
                }
                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private static Platform GetUnixVariant()
        {
            var result = Platform.Unix;
            var buffer = Marshal.AllocHGlobal(8192);

            if (uname(buffer) == 0)
            {
                var variant = Marshal.PtrToStringAnsi(buffer);
                // Todo: Use "is not" instead!
                if (variant is not null && variant.Equals("Darwin", StringComparison.Ordinal))
                {
                    result = Platform.Mac;
                }
            }

            Marshal.FreeHGlobal(buffer);
            return result;
        }

        // Drivers
        [DllImport("libc")]
        private static extern int uname(IntPtr buf);
    }
}