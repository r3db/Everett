using System.Diagnostics;

namespace Everett.Interop
{
    internal sealed class NativeCompiler
    {
        // Internal Instance Data
        private readonly Platform _platform;
        private readonly NativeCompilerInfo _compilerInfo = GetCompilerInfo().First();

        // .Ctor
        internal NativeCompiler(Platform platform)
        {
            _platform = platform;
        }

        // Methods
        internal void Compile(string source, ProcessorArchitecture architecture)
        {
            var args = GetArguments(source, _compilerInfo, _platform, architecture);

            foreach (var item in args)
            {
                var process = CreateProcess(item, _platform, architecture);

                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidProgramException(output);
                }
            }
        }
        
        // Helpers
        private static IEnumerable<NativeCompilerInfo> GetCompilerInfo()
        {
            if (PlatformUtility.Platform != Platform.Windows)
            {
                return new List<NativeCompilerInfo>
                {
                    new NativeCompilerInfo
                    {
                        Version = new Version(),
                        X86Location = "gcc",
                        X64Location = "gcc"
                    }
                };
            }

            return new[]
            {
                new NativeCompilerInfo {
                    Version     = new Version(),
                    X86Location = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\VC\\Tools\\MSVC\\14.34.31933\\bin\\Hostx64\\x64\\cl.exe",
                    X64Location = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\VC\\Tools\\MSVC\\14.34.31933\\bin\\Hostx64\\x64\\cl.exe",
                }
            };

            //var result = new List<NativeCompilerInfo>();
            //var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            //var registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            //var versions = new List<Version>()
            //{
            //    new Version("2022"),
            //    new Version("14.0"),
            //    new Version("12.0"),
            //    new Version("11.0"),
            //};

            //foreach (var version in versions)
            //{
            //    var key = string.Format(@"SOFTWARE\Microsoft\VisualStudio\{0}.{1}", version.Major, version.Minor);

            //    if (registry.OpenSubKey(key) == null)
            //    {
            //        continue;
            //    }

            //    var win32Path = string.Format(@"{0}\Microsoft Visual Studio {1}.{2}\VC\bin\cl.exe", basePath, version.Major, version.Minor);
            //    var win64Path = string.Format(@"{0}\Microsoft Visual Studio {1}.{2}\VC\bin\amd64\cl.exe", basePath, version.Major, version.Minor);

            //    if (File.Exists(win32Path) && File.Exists(win64Path))
            //    {
            //        result.Add(new NativeCompilerInfo
            //        {
            //            Version = version,
            //            X86Location = win32Path,
            //            X64Location = win64Path,
            //        });
            //    }
            //}

            //return result;
        }

        private static IEnumerable<string> GetArguments(string source, NativeCompilerInfo info, Platform platform, ProcessorArchitecture architecture)
        {
            switch (platform)
            {
                case Platform.Windows: return GetArgumentsForWindows(source, info, platform, architecture);
                case Platform.Unix:    return GetArgumentsForUnix(source, platform, architecture);
                case Platform.Mac:     return GetArgumentsForUnix(source, platform, architecture);
            }

            throw new NotSupportedException();
        }

        private static IEnumerable<string> GetArgumentsForWindows(string source, NativeCompilerInfo info, Platform platform, ProcessorArchitecture architecture)
        {
            const string format = "\"{0}\"";
            var libs = string.Join(" ", GetLibraries(info, architecture).Select(x => string.Format(format, x)));
            var includes = string.Join(" ", GetIncludesForWindows().Select(x => string.Format(format, x)));
            var output = string.Format("/Fe\"{0}_{1}.{2}\"", Path.GetFileNameWithoutExtension(source), architecture.ToString().ToLowerInvariant(), Utility.GetFileExtension(platform));

            yield return string.Format("/LD \"{0}\" /DYNAMICBASE {1} /I{2} {3}", source, libs, includes, output);
        }

        private static IEnumerable<string> GetArgumentsForUnix(string source, Platform platform, ProcessorArchitecture architecture)
        {
            var machine = architecture == ProcessorArchitecture.X86
                ? "m32"
                : "m64";

            var output1 = string.Format("{0}_{1}.o", Path.GetFileNameWithoutExtension(source), architecture.ToString().ToLowerInvariant());
            var output2 = string.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(source), architecture.ToString().ToLowerInvariant(), Utility.GetFileExtension(platform));

            yield return string.Format(@"-{0} -c -Wall -Werror -fpic {1} -o {2}", machine, source, output1);
            yield return string.Format(@"-{0} -shared -o {1} {2}", machine, output2, output1);
        }

        private static IEnumerable<string> GetLibraries(NativeCompilerInfo info, ProcessorArchitecture architecture)
        {
            switch (architecture)
            {
                case ProcessorArchitecture.X86: return GetLibrariesForWindowsX86(info.Version);
                case ProcessorArchitecture.X64: return GetLibrariesForWindowsX64(info.Version);
            }

            throw new NotSupportedException("ProcessorArchitecture not supported!");
        }

        private static IEnumerable<string> GetLibrariesForWindowsX86(Version version)
        {
            var vm = string.Format("{0}.{1}", version.Major, version.Minor);

            return new List<string>
            {
                string.Format(@"C:\Program Files (x86)\Microsoft Visual Studio {0}\VC\lib\libcmt.lib", vm),
                string.Format(@"C:\Program Files (x86)\Microsoft Visual Studio {0}\VC\lib\oldnames.lib", vm),
                string.Format(@"C:\Program Files (x86)\Microsoft Visual Studio {0}\VC\lib\libvcruntime.lib", vm),
                @"C:\Program Files (x86)\Windows Kits\8.1\Lib\winv6.3\um\x86\uuid.lib",
                @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Lib\kernel32.lib",
                @"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.10240.0\ucrt\x86\libucrt.lib",
            };
        }

        private static IEnumerable<string> GetLibrariesForWindowsX64(Version version)
        {
            var vm = string.Format("{0}.{1}", version.Major, version.Minor);

            return new List<string>
            {
                string.Format(@"C:\Program Files (x86)\Microsoft Visual Studio {0}\VC\lib\amd64\libcmt.lib", vm),
                string.Format(@"C:\Program Files (x86)\Microsoft Visual Studio {0}\VC\lib\amd64\oldnames.lib", vm),
                string.Format(@"C:\Program Files (x86)\Microsoft Visual Studio {0}\VC\lib\amd64\libvcruntime.lib", vm),
                @"C:\Program Files (x86)\Windows Kits\8.1\Lib\winv6.3\um\x64\uuid.lib",
                @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Lib\x64\kernel32.lib",
                @"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.10240.0\ucrt\x64\libucrt.lib",
            };
        }

        private static IEnumerable<string> GetIncludesForWindows()
        {
            return new List<string>
            {
                @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.10240.0\ucrt\stdio.h",
            };
        }

        private Process CreateProcess(string arguments, Platform platform, ProcessorArchitecture architecture)
        {
            switch (platform)
            {
                case Platform.Windows: return CreateProcessForWindows(arguments, architecture);
                case Platform.Unix:    return CreateProcessForUnix(arguments, architecture);
                case Platform.Mac:     return CreateProcessForUnix(arguments, architecture);
            }

            throw new NotSupportedException("ProcessorArchitecture not supported!");
        }

        private Process CreateProcessForWindows(string arguments, ProcessorArchitecture architecture)
        {
            const string includeIdentifier = "INCLUDE";

            var fileName = architecture == ProcessorArchitecture.X64
                ? _compilerInfo.X64Location
                : _compilerInfo.X86Location;

            var include = Environment.GetEnvironmentVariable(includeIdentifier);

            var includes = new[]
            {
                @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\include\",
                @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.10240.0\ucrt\"
            };

            var process =  new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments              = arguments,
                    CreateNoWindow         = true,
                    UseShellExecute        = false,
                    RedirectStandardOutput = true,
                    FileName               = fileName,
                    WindowStyle            = ProcessWindowStyle.Hidden,
                }
            };

            var environmentVariables = process.StartInfo.EnvironmentVariables;

            if (environmentVariables.ContainsKey(includeIdentifier) == false)
            {
                environmentVariables.Add(includeIdentifier, string.Format("{0};{1}", include, string.Join(";", includes)));
            }

            return process;
        }

        private Process CreateProcessForUnix(string arguments, ProcessorArchitecture architecture)
        {
            var fileName = architecture == ProcessorArchitecture.X64
                ? _compilerInfo.X64Location
                : _compilerInfo.X86Location;

            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = fileName,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }
            };
        }
    }
}