# X-Native

X-Native is an Xbox Dev Mode focused compatibility platform. The long term goal
is a modular environment able to manage and launch PC style applications on
Xbox, designed around the constraints of UWP.

This repository currently contains the project skeleton only. No compatibility
layer, no graphics translation and no launching logic is implemented yet.

## Repository layout

```
X-Native/
  .github/       repository automation
  assets/        icons, images and other static content
  docs/          architecture and design documents
  src/           application and library projects
  tests/         test projects
  tools/         development and build helper scripts
  third_party/   vendored external sources
```

## Projects

| Project | Target | References | Responsibility |
| --- | --- | --- | --- |
| `XNative.Core` | netstandard2.0 | none | Domain model and abstractions. Platform independent and dependency free. |
| `XNative.Platform` | netstandard2.0 | Core | Implementations of the core abstractions for the running platform. |
| `XNative.Graphics` | netstandard2.0 | Core | Graphics backend module boundary. |
| `XNative.Wine` | netstandard2.0 | Core | Compatibility layer module boundary. |
| `XNative.Plugins` | netstandard2.0 | Core | Plugin contracts and discovery boundary. |
| `XNative.UI` | netstandard2.0 | Core | View framework agnostic MVVM primitives. |
| `XNative.App` | net8.0 | all of the above | Composition root and development host. |
| `XNative.XboxHost` | UAP 10.0.19041.0 | all modules | UWP application head for Xbox Developer Mode. |
| `XNative.Core.Tests` | net8.0 | Core | Tests for the core. |
| `XNative.Platform.Tests` | net8.0 | Platform | Tests for the platform module. |
| `XNative.UI.Tests` | net8.0 | UI | Tests for the view models. |

The libraries target `netstandard2.0` because that is the surface a UWP
application can consume. A UWP application head will be added later as a
separate project without any change to the libraries.

## Build

```
dotnet build X-Native.sln
dotnet test X-Native.sln
dotnet run --project src/XNative.App
```

Running the development host prints the modules resolved from the dependency
injection container, which verifies that the composition root is wired
correctly.

`XNative.XboxHost` is a legacy UWP project, which the .NET SDK cannot evaluate.
It is intentionally excluded from the `Any CPU` solution configuration so that
`dotnet build` and `dotnet test` keep working. Build it with MSBuild:

```
msbuild src\XNative.XboxHost\XNative.XboxHost.csproj /t:Restore /p:Platform=x64
msbuild src\XNative.XboxHost\XNative.XboxHost.csproj /p:Configuration=Debug /p:Platform=x64
```

The build produces a signed package under
`src\XNative.XboxHost\AppPackages\`. See
[docs/xbox-host.md](docs/xbox-host.md) for deployment to a console in
Developer Mode.

## Contributing rules

- `XNative.Core` must never reference another project or a NuGet package.
- Interfaces live in `XNative.Core`, implementations live in the modules.
- No placeholder code for features that are not being implemented yet.

See [docs/architecture.md](docs/architecture.md) for details.
