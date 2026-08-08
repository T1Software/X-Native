# Architecture

## Principles

X-Native follows clean architecture. Dependencies point inward, towards
`XNative.Core`. The core defines what the platform can do, the modules decide
how it is done, and the composition root decides which implementation is used.

1. `XNative.Core` is platform independent and has no dependency of any kind.
2. Interfaces are declared in the core, implementations live in the modules.
3. Modules never reference each other, only the core.
4. The composition root is the only place aware of every module.

## Dependency graph

```
                +---------------+
                | XNative.App   |   composition root
                +-------+-------+
                        |
   +----------+---------+---------+-----------+
   |          |         |         |           |
+--v---+  +---v----+ +--v-----+ +-v-------+ +-v--------+
|  UI  |  |Platform| |Graphics| | Wine    | | Plugins  |
+--+---+  +---+----+ +--+-----+ +-+-------+ +-+--------+
   |          |         |         |           |
   +----------+----+----+---------+-----------+
                   |
              +----v-----+
              |   Core   |
              +----------+
```

No arrow ever leaves the core.

## Projects

### XNative.Core

Contains the domain model and the abstractions of the platform. It targets
`netstandard2.0` and references nothing, not even the dependency injection
abstractions, so that it can be consumed by any head and remains trivially
testable. A unit test enforces this rule by inspecting the referenced
assemblies of the compiled core.

Current abstractions, all of them declared without members until the
requirements of the runtime modules are known:

| Interface | Implemented by |
| --- | --- |
| `ILogService` | `XNative.Platform` |
| `IFileSystemService` | `XNative.Platform` |
| `IStorageService` | `XNative.Platform` |
| `IProcessService` | `XNative.Platform` |
| `IDeviceInfoService` | the application head, see below |
| `IGraphicsBackend` | `XNative.Graphics` |
| `ICompatibilityLayer` | `XNative.Wine` |
| `IPluginHost` | `XNative.Plugins` |
| `IModuleDescriptor` | every module |

`IDeviceInfoService` is the only one with a member so far, `GetDeviceInfo`. It
returns a `DeviceInfo`, an immutable model declared in `XNative.Core.Devices`.
It is implemented by the application head rather than by `XNative.Platform`
because reading it requires the Windows Runtime, which a .NET Standard library
cannot reference.

### XNative.Platform

Holds every implementation that touches the operating system. Isolating the
platform here is what keeps the Xbox and UWP sandbox rules out of the rest of
the codebase.

### XNative.Graphics

Boundary for a future rendering backend. It currently contains only its module
registration.

### XNative.Wine

Boundary for a future compatibility layer. It currently contains only its
module registration.

### XNative.Plugins

Defines `IPlugin` and `IPluginCatalog`. Loading strategies will be supplied by
the platform module so that the contracts stay platform independent.

### XNative.UI

View framework agnostic MVVM primitives: `ObservableObject`, `ViewModelBase`,
`RelayCommand` and `INavigationService`, plus the view models under
`ViewModels`. It contains no views and no XAML, so the same view models can be
reused by a UWP head, by a desktop head or by tests.

`StartupViewModel` compares the module descriptors resolved from the container
against the list of expected modules and reports each one as loaded or missing.

### XNative.App

The development composition root. `CompositionRoot.AddXNative` registers every
module and `Program` prints the resolved modules, which makes the wiring
verifiable from the command line.

### XNative.XboxHost

The UWP application head for Xbox Developer Mode. It owns the Windows Runtime
implementations and its own composition root, `Bootstrapper`, which calls the
same `AddXNativeX()` extensions as `XNative.App`. It does not reference
`XNative.App`, since that is an executable and depending on it would point a
dependency outward. See [xbox-host.md](xbox-host.md).

## Module registration

Each module exposes one extension method on `IServiceCollection`:

```csharp
services.AddSingleton<IModuleDescriptor, CoreModuleDescriptor>();
services
    .AddXNativePlatform()
    .AddXNativeGraphics()
    .AddXNativeWine()
    .AddXNativePlugins()
    .AddXNativeUI();
```

The core cannot register itself, because it has no dependency on a container,
so each composition root registers `CoreModuleDescriptor` explicitly. The Xbox
host adds `AddXboxHost()` on top for its Windows Runtime implementations.

Adding a module means adding a project, an extension method and one line in
each composition root. Nothing else in the codebase changes.

## Target frameworks

Libraries target `netstandard2.0`, which is the highest surface a UWP
application can reference. `XNative.App` and the test projects target `net8.0`
so that the modern SDK tooling can build and run them. `XNative.XboxHost`
targets UAP `10.0.19041.0` and references the same `netstandard2.0` libraries
unchanged.

Because the .NET SDK cannot evaluate a legacy UWP project, `XNative.XboxHost`
is mapped to `Debug|x64` under the `Any CPU` solution configuration but is not
built there. `dotnet build` and `dotnet test` therefore keep working on the
whole solution, while MSBuild builds the host under `x64` or `x86`.

## Not implemented

The following are explicitly out of scope for the current skeleton and no
placeholder code exists for them: compatibility layers, `Box86` and `Box64`,
emulation, graphics translation, application launching and user interface
design.
