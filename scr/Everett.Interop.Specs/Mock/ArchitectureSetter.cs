using System;
using System.Reflection;

namespace Everett.Interop
{
    internal sealed class ArchitectureSetter : IDisposable
    {
        // Internal Instance Data
        private readonly ProcessorArchitecture _backup;

        // Methods
        internal ArchitectureSetter(ProcessorArchitecture architecture)
        {
            _backup = PlatformUtility.Architecture;
            SetArchitecture(architecture);
        }

        public void Dispose()
        {
            SetArchitecture(_backup);
        }

        // Helpers
        private static void SetArchitecture(ProcessorArchitecture architecture)
        {
            var field = typeof(PlatformUtility).GetField("_is64BitProcess", BindingFlags.Static | BindingFlags.NonPublic);
            field?.SetValue(null, architecture == ProcessorArchitecture.X64);
        }
    }
}