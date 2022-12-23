using System;
using System.Runtime.InteropServices;

namespace Everett.Interop
{
    // Todo: Introduce Cache!
    public static class ModuleLoader
    {
        // Methods
        public static T LoadAsDelegate<T>(string moniker, string method)
        {
            if (moniker is null)
            {
                throw new ArgumentNullException(nameof(moniker));
            }

            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException(nameof(method));
            }

            var moduleName = ModuleResolver.Resolve(moniker);
            var module = LoadModule(moduleName);

            if (module == IntPtr.Zero)
            {
                throw ModuleInteropException.ModuleCouldNotBeLoaded(moduleName);
            }

            var address = GetProcessAddress(module, method);

            if (address == IntPtr.Zero)
            {
                throw ModuleInteropException.MethodMissing(method);
            }

            var callback = (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));

            if (callback is null)
            {
                throw ModuleInteropException.MethodCouldNotBeLoaded(method);
            }

            return callback;
        }

        // Helpers
        private static IntPtr LoadModule(string fileName)
        {
            switch (PlatformUtility.Platform)
            {
                case Platform.Mac:
                case Platform.Unix:    return ModuleLoaderInterop.LoadLibraryUnix(fileName, 2);
                case Platform.Windows: return ModuleLoaderInterop.LoadLibraryWindows(fileName);
            }

            throw ModuleInteropException.PlatformNotSupported;
        }

        private static IntPtr GetProcessAddress(IntPtr module, string processName)
        {
            switch (PlatformUtility.Platform)
            {
                case Platform.Mac:
                case Platform.Unix:    return ModuleLoaderInterop.GetProcAddressUnix(module, processName);
                case Platform.Windows: return ModuleLoaderInterop.GetProcAddressWindows(module, processName);
            }

            throw ModuleInteropException.PlatformNotSupported;
        }
    }
}