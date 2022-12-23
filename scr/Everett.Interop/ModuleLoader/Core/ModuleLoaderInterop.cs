using System;
using System.Runtime.InteropServices;

namespace Everett.Interop
{
    internal static class ModuleLoaderInterop
    {
        // Internal Const Data
        private const string LibraryNameWindows = "kernel32";
        private const string LibraryNameUnix = "libdl";

        // External Methods
        [DllImport(LibraryNameWindows, EntryPoint = "LoadLibrary")]
        internal static extern IntPtr LoadLibraryWindows(string fileName);

        [DllImport(LibraryNameUnix, EntryPoint = "dlopen")]
        internal static extern IntPtr LoadLibraryUnix(string name, int flags);

        [DllImport(LibraryNameWindows, EntryPoint = "GetProcAddress")]
        internal static extern IntPtr GetProcAddressWindows(IntPtr module, string processName);
        
        [DllImport(LibraryNameUnix, EntryPoint = "dlsym")]
        internal static extern IntPtr GetProcAddressUnix(IntPtr module, string processName);
    }
}