using System;

namespace Everett.Interop
{
    public enum ModuleInteropFailureReason
    {
        PlatformNotSupported,
        ModuleMissing,
        MethodMissing,
        ModuleCouldNotBeLoaded,
        MethodCouldNotBeLoaded,
    }
}