using System;

namespace Everett.Interop
{
    public sealed class ModuleInteropException : Exception
    {
        // Internal Instance Data
        private readonly string _name;

        // .Ctor
        private ModuleInteropException(string name, ModuleInteropFailureReason reason)
        {
            _name = name;
            Reason = reason;
        }

        // Factory .Ctor
        // Todo: Maybe this should be a method like everything else!
        internal static ModuleInteropException PlatformNotSupported => new ModuleInteropException(null, ModuleInteropFailureReason.PlatformNotSupported);
        
        // Note: We're assuming 'moduleName' is always valid, since it's only used internally!
        internal static ModuleInteropException MethodMissing(string methodName)
        {
            return new ModuleInteropException(methodName, ModuleInteropFailureReason.MethodMissing);
        }

        // Note: We're assuming 'moduleName' is always valid, since it's only used internally!
        internal static ModuleInteropException ModuleCouldNotBeLoaded(string fileName)
        {
            return new ModuleInteropException(fileName, ModuleInteropFailureReason.ModuleCouldNotBeLoaded);
        }

        // Note: We're assuming 'moduleName' is always valid, since it's only used internally!
        internal static ModuleInteropException MethodCouldNotBeLoaded(string methodName)
        {
            return new ModuleInteropException(methodName, ModuleInteropFailureReason.MethodCouldNotBeLoaded);
        }

        // Properties
        public ModuleInteropFailureReason Reason { get; }

        public override string Message
        {
            get
            {
                switch (Reason)
                {
                    case ModuleInteropFailureReason.PlatformNotSupported:   return "Platform not suported.";
                    case ModuleInteropFailureReason.ModuleMissing:          return string.Format("Cannot find module '{0}'.", _name);
                    case ModuleInteropFailureReason.MethodMissing:          return string.Format("Cannot find method '{0}'.", _name);
                    case ModuleInteropFailureReason.ModuleCouldNotBeLoaded: return string.Format("Cannot load module '{0}'.", _name);
                    case ModuleInteropFailureReason.MethodCouldNotBeLoaded: return string.Format("Cannot load method '{0}'.", _name);
                }

                // Note: This code cannot be reached! (It's here to satisfy the compiler)
                throw new InvalidProgramException();
            }
        }
    }
}