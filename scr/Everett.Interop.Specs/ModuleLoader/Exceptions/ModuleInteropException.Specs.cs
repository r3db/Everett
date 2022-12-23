using System;

namespace Everett.Interop
{
    [TestFixture]
    public sealed class ModuleInteropExceptionSpecs
    {
        // Factory .Ctor
        [Test]
        public void AssertPlatformNotSupportedFactoryCtor()
        {
            // Act
            var exception = ModuleInteropException.PlatformNotSupported;

            // Assert
            Assert.AreEqual("Platform not suported.", exception.Message);
            Assert.AreEqual(ModuleInteropFailureReason.PlatformNotSupported, exception.Reason);
        }
        
        [Test]
        public void AssertMethodMissingFactoryCtor()
        {
            // Arrange
            const string methodName = "Some Method 1...";

            // Act
            var exception = ModuleInteropException.MethodMissing(methodName);

            // Assert
            Assert.AreEqual(string.Format("Cannot find method '{0}'.", methodName), exception.Message);
            Assert.AreEqual(ModuleInteropFailureReason.MethodMissing, exception.Reason);
        }

        [Test]
        public void AssertModuleCouldNotBeLoadedFactoryCtor()
        {
            // Arrange
            const string moduleName = "Some Module 2...";

            // Act
            var exception = ModuleInteropException.ModuleCouldNotBeLoaded(moduleName);

            // Assert
            Assert.AreEqual(string.Format("Cannot load module '{0}'.", moduleName), exception.Message);
            Assert.AreEqual(ModuleInteropFailureReason.ModuleCouldNotBeLoaded, exception.Reason);
        }

        [Test]
        public void AssertMethodCouldNotBeLoadedFactoryCtor()
        {
            // Arrange
            const string moduleName = "Some Method 2...";

            // Act
            var exception = ModuleInteropException.MethodCouldNotBeLoaded(moduleName);

            // Assert
            Assert.AreEqual(string.Format("Cannot load method '{0}'.", moduleName), exception.Message);
            Assert.AreEqual(ModuleInteropFailureReason.MethodCouldNotBeLoaded, exception.Reason);
        }
    }
}