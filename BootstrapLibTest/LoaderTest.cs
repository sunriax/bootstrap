using RaGae.BootstrapLib.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace BootstrapLibTest
{
    public class LoaderTest
    {
        private static string loaderPath = "Loader";

        private static IEnumerable<(bool, bool)> defaultOptions = new List<(bool, bool)>()
        {
            (false, false),
            (false, true),
            (true, true)
        };

        public static IEnumerable<object[]> GetConfiguration_Passing(string function)
        {
            string path;

            switch (function)
            {
                case nameof(Loader.LoadConfig):
                    path = "*.nosection.json";
                    break;
                case nameof(Loader.LoadConfigSection):
                    path = "*.section.json";
                    break;
                default:
                    throw new XunitException("TILT: should not be reached!");
            }

            foreach ((bool optional, bool reload) item in defaultOptions)
            {
                foreach (string file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), loaderPath), path))
                {
                    switch (function)
                    {
                        case nameof(Loader.LoadConfig):
                            yield return new object[] { Path.Combine(loaderPath, Path.GetFileName(file)), item.optional, item.reload };
                            yield return new object[] { file, item.optional, item.reload };
                            break;
                        case nameof(Loader.LoadConfigSection):
                            yield return new object[] { Path.Combine(loaderPath, Path.GetFileName(file)), null, item.optional, item.reload };
                            yield return new object[] { Path.Combine(loaderPath, Path.GetFileName(file)), nameof(DemoConfig), item.optional, item.reload };
                            yield return new object[] { file, null, item.optional, item.reload };
                            yield return new object[] { file, nameof(DemoConfig), item.optional, item.reload };
                            break;
                        default:
                            throw new XunitException("TILT: should not be reached!");
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetConfiguration_Passing), nameof(Loader.LoadConfig))]
        public void LoadConfig_Passing(string file, bool optional, bool reload)
        {
            DemoConfig demo = Loader.LoadConfig<DemoConfig>(file, optional, reload);

            Assert.Equal(1.ToString(), demo.Value.ToString());

            if (file.Contains("empty"))
                Assert.Null(demo.Array);
            else
            {
                Assert.IsType<List<DemoArrayConfig>>(demo.Array);
                Assert.Equal(1.ToString(), demo.Array.ElementAt(0).Value.ToString());
                Assert.Equal(ErrorCode.OK, demo.Array.ElementAt(0).Error);
            }
        }

        [Theory]
        [MemberData(nameof(GetConfiguration_Passing), nameof(Loader.LoadConfigSection))]
        public void LoadConfigSection_Passing(string file, string section, bool optional, bool reload)
        {
            DemoConfig demo = Loader.LoadConfigSection<DemoConfig>(file, section, optional, reload);

            Assert.Equal(1.ToString(), demo.Value.ToString());

            if (file.Contains("empty"))
                Assert.Null(demo.Array);
            else
            {
                Assert.IsType<List<DemoArrayConfig>>(demo.Array);
                Assert.Equal(1.ToString(), demo.Array.ElementAt(0).Value.ToString());
                Assert.Equal(ErrorCode.OK, demo.Array.ElementAt(0).Error);
            }
        }

        public static IEnumerable<object[]> GetConfiguration_Failing(string function)
        {
            string path;

            switch (function)
            {
                case nameof(Loader.LoadConfig):
                    path = "*.nosection.failing.json";
                    break;
                case nameof(Loader.LoadConfigSection):
                    path = "*.section.failing.json";
                    break;
                default:
                    throw new XunitException("TILT: should not be reached!");
            }

            foreach ((bool optional, bool reload) item in defaultOptions)
            {
                foreach (string file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), loaderPath), path))
                {
                    switch (function)
                    {
                        case nameof(Loader.LoadConfig):
                            yield return new object[] { Path.Combine(loaderPath, Path.GetFileName(file)), item.optional, item.reload };
                            yield return new object[] { file, item.optional, item.reload };
                            break;
                        case nameof(Loader.LoadConfigSection):
                            yield return new object[] { Path.Combine(loaderPath, Path.GetFileName(file)), null, item.optional, item.reload };
                            yield return new object[] { Path.Combine(loaderPath, Path.GetFileName(file)), nameof(DemoConfig), item.optional, item.reload };
                            yield return new object[] { file, null, item.optional, item.reload };
                            yield return new object[] { file, nameof(DemoConfig), item.optional, item.reload };
                            break;
                        default:
                            throw new XunitException("TILT: should not be reached!");
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetConfiguration_Failing), nameof(Loader.LoadConfig))]
        public void LoadConfig_Failing(string file, bool optional, bool reload)
        {
            DemoConfig demo = null;

            Exception ex = Assert.Throws<TargetInvocationException>(() => demo = Loader.LoadConfig<DemoConfig>(file, optional, reload));

            Assert.Null(demo);
            Assert.NotNull(ex.InnerException);
            
            if (file.Contains("array"))
                Assert.Equal(nameof(DemoArrayConfig), ex.InnerException.Message);
            else
                Assert.Equal(nameof(DemoConfig), ex.InnerException.Message);
        }

        [Theory]
        [MemberData(nameof(GetConfiguration_Failing), nameof(Loader.LoadConfigSection))]
        public void LoadConfigSection_Failing(string file, string section, bool optional, bool reload)
        {
            DemoConfig demo = null;

            Exception ex = Assert.Throws<TargetInvocationException>(() => demo = Loader.LoadConfigSection<DemoConfig>(file, section, optional, reload));

            Assert.Null(demo);
            Assert.NotNull(ex.InnerException);

            if (file.Contains("array"))
                Assert.Equal(nameof(DemoArrayConfig), ex.InnerException.Message);
            else
                Assert.Equal(nameof(DemoConfig), ex.InnerException.Message);
        }

        private static IEnumerable<(bool, bool)> optionalOptions = new List<(bool, bool)>()
        {
            (true, false),
            (true, true)
        };

        public static IEnumerable<object[]> GetConfigurationOptional_Passing(string function)
        {
            foreach ((bool optional, bool reload) item in optionalOptions)
            {
                switch (function)
                {
                    case nameof(Loader.LoadConfig):
                        yield return new object[] { "not.existing.json", item.optional, item.reload };
                        yield return new object[] { Path.Combine(Directory.GetCurrentDirectory(), "not.existing.json"), item.optional, item.reload };
                        break;
                    case nameof(Loader.LoadConfigSection):
                        yield return new object[] { "not.existing.json", null, item.optional, item.reload };
                        yield return new object[] { "not.existing.json", nameof(DemoConfig), item.optional, item.reload };
                        yield return new object[] { Path.Combine(Directory.GetCurrentDirectory(), "not.existing.json"), null, item.optional, item.reload };
                        yield return new object[] { Path.Combine(Directory.GetCurrentDirectory(), "not.existing.json"), nameof(DemoConfig), item.optional, item.reload };
                        break;
                    default:
                        throw new XunitException("TILT: should not be reached!");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetConfigurationOptional_Passing), nameof(Loader.LoadConfig))]
        public void LoadConfigWithNonExistingFileButOptional_Passing(string file, bool optional, bool reload)
        {
            DemoConfig demo = Loader.LoadConfig<DemoConfig>(file, optional, reload);
            Assert.Null(demo);
        }

        [Theory]
        [MemberData(nameof(GetConfigurationOptional_Passing), nameof(Loader.LoadConfigSection))]
        public void LoadConfigSectionWithNonExistingFileButOptional_Passing(string file, string section, bool optional, bool reload)
        {
            DemoConfig demo = Loader.LoadConfigSection<DemoConfig>(file, section, optional, reload);

            Assert.Equal(0.ToString(), demo.Value.ToString());
            Assert.Null(demo.Array);
        }

        private static IEnumerable<(bool, bool)> optionalDisabledOptions = new List<(bool, bool)>()
        {
            (false, false),
            (false, true)
        };

        public static IEnumerable<object[]> GetConfigurationOptional_Failing(string function)
        {
            foreach ((bool optional, bool reload) item in optionalDisabledOptions)
            {
                switch (function)
                {
                    case nameof(Loader.LoadConfig):
                        yield return new object[] { "not.existing.json", item.optional, item.reload };
                        yield return new object[] { Path.Combine(Directory.GetCurrentDirectory(), "not.existing.json"), item.optional, item.reload };
                        break;
                    case nameof(Loader.LoadConfigSection):
                        yield return new object[] { "not.existing.json", null, item.optional, item.reload };
                        yield return new object[] { "not.existing.json", nameof(DemoConfig), item.optional, item.reload };
                        yield return new object[] { Path.Combine(Directory.GetCurrentDirectory(), "not.existing.json"), null, item.optional, item.reload };
                        yield return new object[] { Path.Combine(Directory.GetCurrentDirectory(), "not.existing.json"), nameof(DemoConfig), item.optional, item.reload };
                        break;
                    default:
                        throw new XunitException("TILT: should not be reached!");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetConfigurationOptional_Failing), nameof(Loader.LoadConfig))]
        public void LoadConfigWithNonExistingFile_Failing(string file, bool optional, bool reload)
        {
            DemoConfig demo = null;

            Exception ex = Assert.Throws<FileNotFoundException>(() => demo = Loader.LoadConfig<DemoConfig>(file, optional, reload));

            Assert.Null(demo);
            Assert.Null(ex.InnerException);

            if (Path.IsPathRooted(file))
                Assert.Equal($"The configuration file '{Path.GetFileName(file)}' was not found and is not optional. The expected physical path was '{file}'.", ex.Message);
            else
                Assert.Equal($"The configuration file '{file}' was not found and is not optional. The expected physical path was '{Path.Combine(Directory.GetCurrentDirectory(), file)}'.", ex.Message);
        }

        [Theory]
        [MemberData(nameof(GetConfigurationOptional_Failing), nameof(Loader.LoadConfigSection))]
        public void LoadConfigSectionWithNonExistingFile_Failing(string file, string section, bool optional, bool reload)
        {
            DemoConfig demo = null;

            Exception ex = Assert.Throws<FileNotFoundException>(() => demo = Loader.LoadConfigSection<DemoConfig>(file, section, optional, reload));

            Assert.Null(demo);
            Assert.Null(ex.InnerException);

            if (Path.IsPathRooted(file))
                Assert.Equal($"The configuration file '{Path.GetFileName(file)}' was not found and is not optional. The expected physical path was '{file}'.", ex.Message);
            else
                Assert.Equal($"The configuration file '{file}' was not found and is not optional. The expected physical path was '{Path.Combine(Directory.GetCurrentDirectory(), file)}'.", ex.Message);
        }
    }
}
