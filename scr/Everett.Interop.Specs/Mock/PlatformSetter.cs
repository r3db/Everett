using System;
using System.Reflection;

namespace Everett.Interop
{
    internal sealed class PlatformSetter: IDisposable
    {
        // Internal Instance Data
        private readonly Platform _backup;

        // Methods
        internal PlatformSetter(Platform platform)
        {
            _backup = PlatformUtility.Platform;
            SetPlatform(platform);
        }

        public void Dispose()
        {
            SetPlatform(_backup);
        }

        // Helpers
        private static void SetPlatform(Platform platform)
        {
            var field = typeof(PlatformUtility).GetField("_platform", BindingFlags.Static | BindingFlags.NonPublic);
            field?.SetValue(null, platform);
        }
    }
}