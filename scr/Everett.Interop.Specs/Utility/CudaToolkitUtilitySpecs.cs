using System.Globalization;

namespace Everett.Interop
{
    [TestFixture]
    public sealed class CudaToolkitUtilitySpecs
    {
        #region Internal Const Data

        private const EnvironmentVariableTarget Target = EnvironmentVariableTarget.Process;

        #endregion

        #region Methods

        #region Version Agnostic

        [Test]
        public void AssertRegisterMethodInstanceConditions()
        {
            var message = string.Format("Value cannot be null. (Parameter 'mappings')", Utility.NewLine);

            Assert.That(() => CudaToolkitUtility.Register(null),
                Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(message)
            );
        }

        [Test]
        public void AssertRegisterMethodWhenVersionDoesNotExist()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(1, 0), @"C:\SomeLocation\Version-1.0"))
            {
                // Assert
                Assert.That(() => CudaToolkitUtility.Register(mappings),
                    Throws.TypeOf<NotSupportedException>().With.Message.EqualTo("CUDA Toolkit is not installed or version not supported.\r\nSupported Versions: 7.5; 8.0; 9.0; 10.0; 11.0; 12.0")
                );
            }
        }

        [Test]
        public void AssertRegisterMethodWhenCudaDoesNotExist()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            {
                Environment.SetEnvironmentVariable("CUDA_PATH", null, EnvironmentVariableTarget.Process);

                // Assert
                Assert.That(() => CudaToolkitUtility.Register(mappings),
                    Throws.TypeOf<NotSupportedException>().With.Message.EqualTo("CUDA Toolkit is not installed or environment variable 'CUDA_PATH' not set.")
                );
            }
        }

        #endregion

        #region Windows

        #region Using Installed Version

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsUsingInstalledVersion()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.DefaultCudaToolkitEnvironmentVariable())
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win32.dll", name);
            }
        }
        
        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsUsingInstalledVersion()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);
            
            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.DefaultCudaToolkitEnvironmentVariable())
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win64.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsWithVersionOnLibraryNameUsingInstalledVersion()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.DefaultCudaToolkitEnvironmentVariable())
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual(string.Format("win32_{0}.dll", GetVersionMoniker()), name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsWithVersionOnLibraryNameUsingInstalledVersion()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.DefaultCudaToolkitEnvironmentVariable())
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual(string.Format("win64_{0}.dll", GetVersionMoniker()), name);
            }
        }

        #endregion

        #region Using Version 7.5

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsUsingVersion75()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(7, 5), @"C:\SomeLocation\Version\v7.5"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win32.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsUsingVersion75()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(7, 5), @"C:\SomeLocation\Version\v7.5"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win64.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsWithVersionOnLibraryNameUsingVersion75()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(7, 5), @"C:\SomeLocation\Version\v7.5"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win32_75.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsWithVersionOnLibraryNameUsingVersion75()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(7, 5), @"C:\SomeLocation\Version\v7.5"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win64_75.dll", name);
            }
        }

        #endregion

        #region Using Version 8.0

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsUsingVersion80()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(8, 0), @"C:\SomeLocation\Version\v8.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual(@"win32.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsUsingVersion80()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(8, 0), @"C:\SomeLocation\Version\v8.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win64.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsWithVersionOnLibraryNameUsingVersion80()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(8, 0), @"C:\SomeLocation\Version\v8.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win32_80.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsWithVersionOnLibraryNameUsingVersion80()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(8, 0), @"C:\SomeLocation\Version\v8.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual(@"win64_80.dll", name);
            }
        }

        #endregion

        #region Using Version 9.0

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsUsingVersion90()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(9, 0), @"C:\SomeLocation\Version\v9.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win32.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsUsingVersion90()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration01(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(9, 0), @"C:\SomeLocation\Version\v9.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win64.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX86WindowsWithVersionOnLibraryNameUsingVersion90()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(9, 0), @"C:\SomeLocation\Version\v9.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win32_90.dll", name);
            }
        }

        [Test]
        public void AssertRegisterMethodWhenRunningOnX64WindowsWithVersionOnLibraryNameUsingVersion90()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                Assert.Ignore("Only Runs in Windows");
            }

            // Act
            var moniker = Guid.NewGuid().ToString();
            var mappings = CreateConfiguration02(moniker);

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            using (Mock.CudaToolkitEnvironmentVariable(new Version(9, 0), @"C:\SomeLocation\Version\v9.0"))
            {
                // Arrange
                CudaToolkitUtility.Register(mappings);

                // Assert
                var name = ModuleResolver.Resolve(moniker);
                Assert.AreEqual("win64_90.dll", name);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Helpers

        [OneTimeTearDown]
        public void AssertNumberOfIgnoreTests()
        {
            if (PlatformUtility.Platform == Platform.Windows)
            {
                // 2 Unix + 2 Mac
                Assert.AreEqual(2 + 2, TestContext.CurrentContext.Result.SkipCount);
            }

            if (PlatformUtility.Platform == Platform.Unix)
            {
                // 18 Windows + 2 Mac
                Assert.AreEqual(18 + 2, TestContext.CurrentContext.Result.SkipCount);
            }

            if (PlatformUtility.Platform == Platform.Mac)
            {
                // 18 Windows + 2 Unix
                Assert.AreEqual(18 + 2, TestContext.CurrentContext.Result.SkipCount);
            }
        }

        private static IDictionary<string, List<ModuleResolverMapping>> CreateConfiguration01(string moniker)
        {
            return new Dictionary<string, List<ModuleResolverMapping>>
            {
                { moniker, new List<ModuleResolverMapping> {
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Windows, Name = "win32.dll" },
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Windows, Name = "win64.dll" },
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Unix,    Name = "unix32.so" },
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Unix,    Name = "unix64.so" },
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac,     Name = "mac32.lib" },
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Mac,     Name = "mac64.lib" },
                }}
            };
        }

        private static IDictionary<string, List<ModuleResolverMapping>> CreateConfiguration02(string moniker)
        {
            return new Dictionary<string, List<ModuleResolverMapping>>
            {
                { moniker, new List<ModuleResolverMapping> {
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Windows, Name = "win32_{0}.dll" },
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Windows, Name = "win64_{0}.dll" },
                }}
            };
        }

        private static string GetVersionMoniker()
        {
            var ev = Environment.GetEnvironmentVariable("CUDA_PATH", Target)?.TrimEnd('\\');

            if (ev == null)
            {
                throw new NotSupportedException("CUDA Toolkit is not installed or environment variable 'CUDA_PATH' not set.");
            }

            var index = ev.LastIndexOf("\\", StringComparison.InvariantCulture);
            var version = ev.Substring(index + 1);

            var supportedVersions = new Dictionary<string, Version>
            {
                { "v7.5",  new Version( 7, 5) },
                { "v8.0",  new Version( 8, 0) },
                { "v9.0",  new Version( 9, 0) },
                { "v10.0", new Version(10, 0) },
                { "v11.0", new Version(11, 0) },
                { "v12.0", new Version(12, 0) },
            };

            if (supportedVersions.ContainsKey(version))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}", supportedVersions[version].Major, supportedVersions[version].Minor);
            }

            throw new NotSupportedException(string.Format("CUDA Toolkit is not installed or version not supported.{0}Supported Versions: {1}", Environment.NewLine, string.Join("; ", supportedVersions.Values)));
        }

        #endregion
    }
}