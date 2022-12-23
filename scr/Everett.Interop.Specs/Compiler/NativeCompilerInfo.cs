using System;

namespace Everett.Interop
{
    internal sealed class NativeCompilerInfo
    {
        public Version Version { get; set; }
        public string X86Location { get; set; }
        public string X64Location { get; set; }
    }
}