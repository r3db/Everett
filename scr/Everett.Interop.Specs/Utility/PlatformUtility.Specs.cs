using System;

namespace Everett.Interop
{
    [TestFixture]
    public sealed class PlatformUtilitySpecs
    {
        // Methods
        [Test]
        public void AssertPlatformPropertyWhenRunningOnWindows()
        {
            using (Mock.Platform(PlatformID.Win32NT))
            {
                Assert.AreEqual(Platform.Windows, PlatformUtility.Platform);
            }

            using (Mock.Platform(PlatformID.Win32S))
            {
                Assert.AreEqual(Platform.Windows, PlatformUtility.Platform);
            }

            using (Mock.Platform(PlatformID.Win32Windows))
            {
                Assert.AreEqual(Platform.Windows, PlatformUtility.Platform);
            }
        }

        [Test]
        public void AssertPlatformPropertyWhenRunningOnUnix()
        {
            using (Mock.Platform(PlatformID.Unix))
            {
                Assert.AreEqual(Platform.Unix, PlatformUtility.Platform);
            }
        }

        [Test]
        public void AssertPlatformPropertyWhenRunningOnMac()
        {
            using (Mock.Platform(PlatformID.MacOSX))
            {
                Assert.AreEqual(Platform.Mac, PlatformUtility.Platform);
            }
        }

        [Test]
        public void AssertArchitecturePropertyWhenRunningX86()
        {
            using (Mock.Architecture(ProcessorArchitecture.X86))
            {
                Assert.AreEqual(ProcessorArchitecture.X86, PlatformUtility.Architecture);
            }
        }

        [Test]
        public void AssertArchitecturePropertyWhenRunningX64()
        {
            using (Mock.Architecture(ProcessorArchitecture.X64))
            {
                Assert.AreEqual(ProcessorArchitecture.X64, PlatformUtility.Architecture);
            }
        }
    }
}