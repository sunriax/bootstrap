[![Version: 1.0 Release](https://img.shields.io/badge/Version-1.0%20Release-green.svg)](https://github.com/sunriax) [![NuGet](https://img.shields.io/nuget/dt/ragae.bootstrap.svg)](https://www.nuget.org/packages/ragae.bootstrap) [![Build Status](https://www.travis-ci.com/sunriax/bootstrap.svg?branch=main)](https://www.travis-ci.com/sunriax/bootstrap) [![codecov](https://codecov.io/gh/sunriax/bootstrap/branch/main/graph/badge.svg)](https://codecov.io/gh/sunriax/bootstrap) [![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

# BootstrapLib

## Description:

Base class for bootstrapping applications. The library will be used in whole RaGae namespace (if necessary!).

---

## Installation

To install BootstrapLib it is possible to download library [[zip](https://github.com/sunriax/bootstrap/releases/latest/download/Bootstrap.zip) | [gzip](https://github.com/sunriax/bootstrap/releases/latest/download/Bootstrap.tar.gz)] or install it via nuget.

```
PM> Install-Package RaGae.Bootstrap
```

---

## Usage

After adding/installing the BootstrapLib in a project it can be used to bootstrap the application.

## Structure

### Namespace RaGae.BootstrapLib.Loader

#### Static functions

* `Loader.LoadConfig(file, optional, reload)`
* `Loader.LoadConfigSection(file, section, optional, reload)`

---

## LoadConfig/LoadConfigSection

To bind config from file to a provided class.

### Parameter

#### File

Filename for specific *.json file to load.

``` csharp
T config = Loader.LoadConfig<T>("Filename", "...", "...");
T config = Loader.LoadConfigSection<T>("Filename", "..." "...", "...");
```

#### Section (only LoadConfigSection)

This parameter defines in which section in the json file the configuration can be found. The parameter is not necessary and can be omitted.

``` csharp
T config = Loader.LoadConfigSection<T>("...", "DemoConfig" "...", "...");
```

#### Optional

This parameter defines if the configuration is optional (can be loaded but must not be loaded!). The parameter is not necessary and can be omitted.

``` csharp
T config = Loader.LoadConfig<T>("Filename", false/true, "...");
T config = Loader.LoadConfigSection<T>("...", "...", false/true, "...");
```

#### Reload

This parameter defines if the configuration should be reloaded at runtime if parameters are changed. The parameter is not necessary and can be omitted.

``` csharp
T config = Loader.LoadConfig<T>("Filename", "...", false/true);
T config = Loader.LoadConfigSection<T>("...", "...", "...", false/true);
```

### Example

Information how to handle a project with BootstrapLib can be found in [Wiki](https://github.com/sunriax/bootstrap/wiki).

#### **`appsettings.section.json`** with section

``` json
{
  "DemoConfig": {
    "Value": 1,
    "Array": [
      { "Value": 1 }
    ]
  }
}
```

#### **`appsettings.nosection.json`** without section

``` json
{
  "Value": 1,
  "Array": [
    { "Value": 1 }
  ]
}
```

#### **`Application.cs`**
``` csharp
using System;
using RaGae.BootstrapLib.Loader;

namespace Project
{
    public LoadConfig()
    {
        try
        {
            // Appsettings with no configuration section
            DemoConfig demo = Loader.LoadConfig<DemoConfig>("appsettings.nosection.json", false, false);

            // Appsettings with configuration section
            DemoConfig demo = Loader.LoadConfigSection<DemoConfig>("appsettings.section.json", nameof(DemoConfig), false, false);

            string demoValue = demo.Value;
            string demoArrayValue = demo.ElementAt(0).Value;
        }
        catch(Exception ex)
        {
            // ...
        }
    }
}
```

#### **`DemoConfig.cs`**
``` csharp
namespace Project
{
    public enum ErrorCode
    {
        OK,
        ERROR,
        TEST
    }

    public class DemoConfig
    {
        private IEnumerable<DemoArrayConfig> array;
        private int value;

        public int Value
        {
            get => this.value;
            set
            {
                if (value < 1)
                    throw new ArgumentException(nameof(DemoConfig));
                this.value = value;
            }
        }

        public IEnumerable<DemoArrayConfig> Array
        {
            get => this.array;
            set
            {
                value.ToList().ForEach(e =>
                {
                    if (e.Error != ErrorCode.OK)
                        throw new ArgumentException(nameof(DemoArrayConfig));
                });

                this.array = value;
            }
        }
    }

    public class DemoArrayConfig
    {
        private int value;

        public int Value
        {
            get => this.value;
            set
            {
                this.Error = ErrorCode.OK;

                if (value < 1)
                    this.Error = ErrorCode.ERROR;

                this.value = value;
            }
        }

        public ErrorCode Error { get; private set; }
    }
}
```

---

R. GÄCHTER
