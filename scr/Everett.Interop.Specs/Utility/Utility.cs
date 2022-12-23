using System;

namespace Everett.Interop
{
    internal static class Utility
    {
        internal static string GetFileExtension(Platform platform)
        {
            switch (platform)
            {
                case Platform.Windows: return "dll";
                case Platform.Unix:    return "so";
                case Platform.Mac:     return "dylib";
            }

            throw new NotSupportedException("Platform is not supported!");
        }

        internal static string NewLine
        {
            get
            {
                switch (PlatformUtility.Platform)
                {
                    case Platform.Windows: return "\r\n";
                    case Platform.Unix:    return "\n";
                    case Platform.Mac:     return "\n";
                }

                throw new NotSupportedException();
            }
        }
    }
}