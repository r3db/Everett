using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Everett.Interop
{
    [TestFixture]
    public sealed class ModuleResolverSpecs
    {
        // Methods
        [Test]
        public void AssertRegisterMethodInstanceConditions()
        {
            var message0 = "Value cannot be null. (Parameter 'moniker')";
            var message1 = "Value cannot be null. (Parameter 'mappings')";

            Assert.That(() => ModuleResolver.Register(null, null),
                Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(message0)
            );

            Assert.That(() => ModuleResolver.Register(string.Empty, null),
                Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(message1)
            );
        }

        [Test]
        public void AssertRegisterMethodWhenMappingsAreEmpty()
        {
            // Arrange
            const string moniker = "Something 1...";

            // Act
            ModuleResolver.Register(moniker, new List<ModuleResolverMapping>());

            // Assert
            var message = string.Format("The specified moniker '{0}' is not valid. (Parameter 'moniker')", moniker);

            Assert.That(() => ModuleResolver.Resolve(moniker),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo(message)
            );
        }

        [Test]
        public void AssertRegisterMethod()
        {
            // Arrange
            const string moniker = "Something 2...";
            const string expected = "Some Dll 1...";

            using (Mock.Platform(PlatformID.MacOSX))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            {
                // Act
                ModuleResolver.Register(moniker, new List<ModuleResolverMapping>
                {
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac, Name = expected }
                });

                // Assert
                var actual = ModuleResolver.Resolve(moniker);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AssertResolveMethodInstanceConditions()
        {
            var message0 = "Value cannot be null. (Parameter 'moniker')";
            var message1 = "The specified moniker '' is not valid. (Parameter 'moniker')";
            var message2 = "The specified moniker 'Some Moniker 1...' is not valid. (Parameter 'moniker')";

            Assert.That(() => ModuleResolver.Resolve(null),
                Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(message0)
            );

            Assert.That(() => ModuleResolver.Resolve(string.Empty),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo(message1)
            );

            Assert.That(() => ModuleResolver.Resolve("Some Moniker 1..."),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo(message2)
            );
        }

        [Test]
        public void AssertResolveMethodWhenOnWindowsAndX86()
        {
            // Arrange
            const string moniker = "Something 3...";
            const string expected = "WinX86_1";

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            {
                // Act
                ModuleResolver.Register(moniker, CreateDefaultMappings());

                var actual = ModuleResolver.Resolve(moniker);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AssertResolveMethodWhenOnWindowsAndX64()
        {
            // Arrange
            const string moniker = "Something 4...";
            const string expected = "WinX64_1";

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            {
                // Act
                ModuleResolver.Register(moniker, CreateDefaultMappings());

                var actual = ModuleResolver.Resolve(moniker);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AssertResolveMethodWhenOnUnixAndX86()
        {
            // Arrange
            const string moniker = "Something 3...";
            const string expected = "UnixX86_1";

            using (Mock.Platform(PlatformID.Unix))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            {
                // Act
                ModuleResolver.Register(moniker, CreateDefaultMappings());

                var actual = ModuleResolver.Resolve(moniker);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AssertResolveMethodWhenOnUnixAndX64()
        {
            // Arrange
            const string moniker = "Something 4...";
            const string expected = "UnixX64_1";

            using (Mock.Platform(PlatformID.Unix))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            {
                // Act
                ModuleResolver.Register(moniker, CreateDefaultMappings());

                var actual = ModuleResolver.Resolve(moniker);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AssertResolveMethodWhenOnMacAndX86()
        {
            // Arrange
            const string moniker = "Something 5...";
            const string expected = "MacX86_1";

            using (Mock.Platform(PlatformID.MacOSX))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            {
                // Act
                ModuleResolver.Register(moniker, CreateDefaultMappings());

                var actual = ModuleResolver.Resolve(moniker);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AssertResolveMethodWhenOnMacAndX64()
        {
            // Arrange
            const string moniker = "Something 6...";
            const string expected = "MacX64_1";

            using (Mock.Platform(PlatformID.MacOSX))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            {
                // Act
                ModuleResolver.Register(moniker, CreateDefaultMappings());

                var actual = ModuleResolver.Resolve(moniker);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AssertResolveMethodWhenUsingRepeatedMoniker()
        {
            // Arrange
            const string moniker = "Something 7...";
            const string expected1 = "Some Dll 1...";
            const string expected2 = "Some Dll 2...";

            using (Mock.Platform(PlatformID.MacOSX))
            using (Mock.Architecture(ProcessorArchitecture.X86))
            {
                ModuleResolver.Register(moniker, new List<ModuleResolverMapping>
                {
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac, Name = expected1 }
                });

                ModuleResolver.Register(moniker, new List<ModuleResolverMapping>
                {
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac, Name = expected2 }
                });

                // Act
                var actual = ModuleResolver.Resolve(moniker);

                // Assert
                Assert.AreEqual(expected1, actual);
            }
        }

        [Test]
        public void AssertResolveMethodWhenMonikerIsNotRegistered()
        {
            // Arrange
            const string moniker = "Something 8...";

            var message = PlatformUtility.Platform == Platform.Unix
                ? string.Format("The specified moniker '{0}' is not valid. (Parameter 'moniker')", moniker)
                : string.Format("The specified moniker '{0}' is not valid. (Parameter 'moniker')", moniker);

            // Act, Assert
            Assert.That(() => ModuleResolver.Resolve(moniker),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo(message)
            );
        }

        [Test]
        public void AssertResolveMethodWhenMonikerIsRegisteredButArchitectureIsNotMapped()
        {
            // Arrange
            const string moniker = "Something 8...";

            using (Mock.Platform(PlatformID.Win32NT))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            {
                ModuleResolver.Register(moniker, new List<ModuleResolverMapping>
                {
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Windows, Name = "WinX86" },
                });

                // Act, Assert
                Assert.That(() => ModuleResolver.Resolve(moniker),
                    Throws.TypeOf<ArgumentException>()
                        .With.Message
                        .StartWith(string.Format("The specified moniker '{0}' is not valid.", moniker))
                );
            }
        }

        [Test]
        public void AssertResolveMethodWhenMonikerIsRegisteredButPlatformIsNotMapped()
        {
            // Arrange
            const string moniker = "Something 8...";
            var newLine = Utility.NewLine;
            var originalPlatform = PlatformUtility.Platform;

            using (Mock.Platform(PlatformID.MacOSX))
            using (Mock.Architecture(ProcessorArchitecture.X64))
            {
                ModuleResolver.Register(moniker, new List<ModuleResolverMapping>
                {
                    new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Windows, Name = "WinX86" },
                });

                var message = originalPlatform == Platform.Unix
                    ? string.Format("The specified moniker '{0}' is not valid. (Parameter 'moniker')", moniker)
                    : string.Format("The specified moniker '{0}' is not valid. (Parameter 'moniker')", moniker);

                // Act, Assert
                Assert.That(() => ModuleResolver.Resolve(moniker),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo(message)
                );
            }
        }

        // Helpers
        private static IEnumerable<ModuleResolverMapping> CreateDefaultMappings()
        {
            return new List<ModuleResolverMapping>
            {
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Windows, Name = "WinX86_1"  },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Windows, Name = "WinX86_2"  },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Windows, Name = "WinX64_1"  },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Windows, Name = "WinX64_2"  },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Unix,    Name = "UnixX86_1" },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Unix,    Name = "UnixX86_2" },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Unix,    Name = "UnixX64_1" },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Unix,    Name = "UnixX64_2" },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac,     Name = "MacX86_1"  },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac,     Name = "MacX86_2"  },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Mac,     Name = "MacX64_1"  },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Mac,     Name = "MacX64_2"  },
            };
        }
    }
}