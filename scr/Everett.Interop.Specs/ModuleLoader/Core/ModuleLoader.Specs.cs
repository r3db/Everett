using System.Text;

namespace Everett.Interop
{
    // Todo: Check what happens if the module is not registered!!
    [TestFixture]
    public sealed class ModuleLoaderSpecs
    {
        // Delegates
        private delegate void InteropMethod(int value, out bool isX64, StringBuilder message);

        // Methods
        [Test]
        public void AssertLoadAsDelegateInstanceConditions()
        {
            var message = "Value cannot be null. (Parameter 'moniker')";

            Assert.That(() => ModuleLoader.LoadAsDelegate<object>(null, null),
                Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(message)
            );

            Assert.That(() => ModuleLoader.LoadAsDelegate<object>("...", null),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("method")
            );

            Assert.That(() => ModuleLoader.LoadAsDelegate<object>("...", string.Empty),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("method")
            );

            Assert.That(() => ModuleLoader.LoadAsDelegate<object>("...", "   "),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("method")
            );
        }

        [Test]
        public void AssertLoadAsDelegateInstanceConditionsWhenModuleCannotBeFound()
        {
            // Todo: Check what happens if the module is not registered!
            // Arrange
            ModuleResolver.Register("Cool", new List<ModuleResolverMapping>
            {
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Windows, Name = "Cool_x86.dll"   },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Windows, Name = "Cool_x64.dll"   },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Unix,    Name = "Cool_x86.so"    },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Unix,    Name = "Cool_x64.so"    },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac,     Name = "Cool_x86.dylib" },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Mac,     Name = "Cool_x64.dylib" },
            });

            var message = string.Format("Cannot load module 'Cool_{0}.{1}'.", 
                Environment.Is64BitProcess ? "x64" : "x86",
                Utility.GetFileExtension(PlatformUtility.Platform));

            // Act, Assert
            Assert.That(() => ModuleLoader.LoadAsDelegate<object>("Cool", "..."),
                Throws.TypeOf<ModuleInteropException>().With.Message.EqualTo(message)
            );
        }

        //[Test]
        //public void AssertLoadAsDelegateInstanceConditionsWhenMethodCannotBeFound()
        //{
        //    // Arrange
        //    var name = Guid.NewGuid().ToString();

        //    var sourceCode = SourceCode(@"
        //        __extern void Method1(int value, bool* isX64, char* message)
	       //     {	
		      //      isX64[0] = IsX64Architecture();

		      //      #ifdef _WIN32
        //                sprintf(message, ""Calling from Windows! [%d]\n"", value);
        //            #elif __APPLE__
        //                sprintf(message, ""Calling from Mac! [%d]\n"", value);
        //            #elif linux
        //                sprintf(message, ""Calling from Unix! [%d]\n"", value);
        //            #endif
        //        }");

        //    Compile(sourceCode, name, PlatformUtility.Platform);

        //    // Act, Assert
        //    Assert.That(() => ModuleLoader.LoadAsDelegate<InteropMethod>(name, "Method2"),
        //        Throws.TypeOf<ModuleInteropException>().With.Message.EqualTo("Cannot find method 'Method2'.")
        //    );
        //}

        //[Test]
        //public void AssertLoadAsDelegateMethod()
        //{
        //    // Arrange
        //    var name = Guid.NewGuid().ToString();

        //    var sourceCode = SourceCode(@"
        //        __extern void Method1(int value, bool* isX64, char* message)
	       //     {	
		      //      isX64[0] = IsX64Architecture();

		      //      #ifdef _WIN32
        //                sprintf(message, ""Calling from Windows! [%d]\n"", value);
        //            #elif __APPLE__
        //                sprintf(message, ""Calling from Mac! [%d]\n"", value);
        //            #elif linux
        //                sprintf(message, ""Calling from Unix! [%d]\n"", value);
        //            #endif
        //        }");

        //    Compile(sourceCode, name, PlatformUtility.Platform);

        //    // Act
        //    var actual = Invoke(name, 3456);

        //    // Assert
        //    Assert.AreEqual(Environment.Is64BitProcess, actual.Item1);
        //    Assert.AreEqual(string.Format("Calling from {0}! [3456]\n", PlatformUtility.Platform), actual.Item2);
        //}

        //[Test]
        //public void AssertLoadAsDelegateMethodForX64Windows()
        //{
        //    if (Environment.Is64BitProcess == false || PlatformUtility.Platform != Platform.Windows)
        //    {
        //        Assert.Ignore("Only Runs in X64-Windows");
        //    }

        //    // Arrange
        //    var name = Guid.NewGuid().ToString();

        //    var sourceCode = SourceCode(@"
        //        __extern void Method1(int value, bool* isX64, char* message)
	       //     {	
		      //      isX64[0] = IsX64Architecture();

		      //      #ifdef _WIN32
        //                sprintf(message, ""Calling from Windows! [%d]\n"", value);
        //            #elif __APPLE__
        //                sprintf(message, ""Calling from Mac! [%d]\n"", value);
        //            #elif linux
        //                sprintf(message, ""Calling from Unix! [%d]\n"", value);
        //            #endif
        //        }");
            
        //    Compile(sourceCode, name, Platform.Windows);

        //    // Act
        //    var actual = Invoke(name, 3456);

        //    // Assert
        //    Assert.AreEqual(true, actual.Item1);
        //    Assert.AreEqual("Calling from Windows! [3456]\n", actual.Item2);
        //}

        [OneTimeTearDown]
        public void AssertNumberOfIgnoreTests()
        {
            Assert.AreEqual(3, TestContext.CurrentContext.Result.SkipCount);
        }

        // Helpers
        private static string SourceCode(string code)
        {
            var sb = new StringBuilder();

            sb.AppendLine(CreateDefaultIncludes() + Environment.NewLine);
            sb.AppendLine(DefineDllExport()       + Environment.NewLine);
            sb.AppendLine(CreateDefaultMethods()  + Environment.NewLine);

            sb.AppendLine("extern \"C\"");
            sb.AppendLine("{");
            sb.AppendLine(Sanitize(code, 4));
            sb.AppendLine("}");

            return sb.ToString()
                .Replace("\t", new string(' ', 4))
                .Replace(new string(' ', 16), string.Empty);
        }

        private static string CreateDefaultIncludes()
        {
            return Sanitize(@"
                #include <stdio.h>
            ");
        }

        private static string DefineDllExport()
        {
            return Sanitize(@"
                #ifdef _WIN32
	                #define __extern __declspec(dllexport)
                #endif
                #ifdef linux
	                #define __extern extern
                #endif
                #ifdef __APPLE__
	                #define __extern extern
                #endif");
        }

        private static string CreateDefaultMethods()
        {
            return Sanitize(@"
                bool IsX64Architecture()
	            {
		            if (sizeof(void*) == 4)
	                {
	                    return false;
                    }
                    else if (sizeof(void*) == 8)
                    {
                        return true;
                    }
                }");
        }

        private static string Sanitize(string code, int padding = 0)
        {
            return string.Join(Environment.NewLine, code
                .Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => string.IsNullOrWhiteSpace(x) == false)
                .Select(x => new string(' ', padding) + x));
        }

        private static void Compile(string source, string name, Platform platform)
        {
            var fileName = string.Format("{0}.cpp", name);
            File.WriteAllText(fileName, source);

            var compiler = new NativeCompiler(platform);

            if (PlatformUtility.Platform == Platform.Windows)
            {
                compiler.Compile(fileName, ProcessorArchitecture.X64);
                compiler.Compile(fileName, ProcessorArchitecture.X86);
            }
            else
            {
                compiler.Compile(fileName, ProcessorArchitecture.X64);
            }

            File.Delete(fileName);

            switch (platform)
            {
                case Platform.Windows:
                {
                    File.Delete(string.Format("{0}.obj", name));
                    File.Delete(string.Format("{0}_x86.exp", name));
                    File.Delete(string.Format("{0}_x86.lib", name));
                    File.Delete(string.Format("{0}_x64.exp", name));
                    File.Delete(string.Format("{0}_x64.lib", name));
                    break;
                }
                case Platform.Unix:
                case Platform.Mac:
                {
                    File.Delete(string.Format("{0}_x86.o", name));
                    File.Delete(string.Format("{0}_x64.o", name));
                    break;
                }
            }

            var directory = Environment.CurrentDirectory;
            var separator = PlatformUtility.Platform == Platform.Windows ? @"\" : "/";

            ModuleResolver.Register(name, new List<ModuleResolverMapping>
            {
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Windows, Name = string.Format("{0}{1}{2}_x86.dll",   directory, separator, name) },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Windows, Name = string.Format("{0}{1}{2}_x64.dll",   directory, separator, name) },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Unix,    Name = string.Format("{0}{1}{2}_x86.so",    directory, separator, name) },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Unix,    Name = string.Format("{0}{1}{2}_x64.so",    directory, separator, name) },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X86, Platform = Platform.Mac,     Name = string.Format("{0}{1}{2}_x86.dylib", directory, separator, name) },
                new ModuleResolverMapping { Architecture = ProcessorArchitecture.X64, Platform = Platform.Mac,     Name = string.Format("{0}{1}{2}_x64.dylib", directory, separator, name) },
            });
        }

        private static Tuple<bool, string> Invoke(string name, int value)
        {
            bool isX64;
            var message = new StringBuilder(100);

            var call = ModuleLoader.LoadAsDelegate<InteropMethod>(name, "Method1");
            call(value, out isX64, message);

            return new Tuple<bool, string>(isX64, message.ToString());
        }
    }
}