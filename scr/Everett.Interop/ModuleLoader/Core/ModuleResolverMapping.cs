using System;

namespace Everett.Interop
{
    public sealed class ModuleResolverMapping
    {
        public string Name { get; set; }
        public Platform Platform { get; set; }
        public ProcessorArchitecture Architecture { get; set; }
    }
}