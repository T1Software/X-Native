# Xbox host

`src/XNative.XboxHost` is the UWP application head of X-Native. It is the only
project allowed to call the Windows Runtime.

## Configuration

| Setting | Value |
| --- | --- |
| Project type | Legacy UWP (`AppContainerExe`) |
| Target platform version | `10.0.19041.0` |
| Target platform minimum version | `10.0.17763.0` |
| Platforms | `x86`, `x64` |
| Device families | `Windows.Universal`, `Windows.Xbox` |

The Xbox operating system tracks the `19041` era rather than the newest desktop
SDK, so `10.0.19041.0` is targeted even though `10.0.26100.0` is installed.
Xbox deploys the `x64` configuration.

## NuGet packages

Only two, both unavoidable:

- `Microsoft.NETCore.UniversalWindowsPlatform`, mandatory for any UWP project.
- `Microsoft.Extensions.DependencyInjection`, the container already used by the
  rest of the solution.

## Dependency direction

The host references `Core`, `Platform`, `UI`, `Graphics`, `Wine` and `Plugins`.
It deliberately does **not** reference `XNative.App`, which is a `net8.0`
executable; depending on it would point a dependency outward. Instead the host
calls the same `AddXNativeX()` extension methods, so both heads share the
registration pattern without sharing a project.

```
XNative.XboxHost ──> UI, Platform, Graphics, Wine, Plugins ──> Core
```

`UwpDeviceInfoService` lives in the host, not in `XNative.Platform`, because
the module libraries target .NET Standard and cannot reference the Windows
Runtime. It implements `IDeviceInfoService`, which is declared in the core, so
the dependency still points inward.

## Startup sequence

1. `App.OnLaunched` sets the view bounds mode and calls `Bootstrapper.Start`.
2. `Bootstrapper` registers every module, builds the container with
   `ValidateOnBuild`, then resolves every `IModuleDescriptor` and the device
   service so that a wiring mistake fails at startup rather than later.
3. `MainPage.OnNavigatedTo` resolves `StartupViewModel`, calls `Load` and
   updates the bindings.
4. If the startup failed, the page shows the failure instead of the report.

The startup screen lists the expected modules in a fixed order and marks each
one as loaded or missing, so a module that fails to register is visible on the
console screen.

## Deploying to a console in Developer Mode

1. Put the console into Developer Mode and note its IP address.
2. Build the package:
   `msbuild src\XNative.XboxHost\XNative.XboxHost.csproj /p:Configuration=Debug /p:Platform=x64`
3. Open the Device Portal at `https://<console-ip>:11443`.
4. Under **Home > Add**, upload the `.msix` from
   `src\XNative.XboxHost\AppPackages\`, together with the dependency `.appx`
   files from the `Dependencies\x64` folder and the `.cer` file.
5. Start **X-Native** from the Dev Home.

From Visual Studio, select `Debug | x64`, choose **Remote Machine**, enter the
console address and deploy.

## Signing

`XNative.XboxHost_TemporaryKey.pfx` is a development certificate created for
local sideloading. It is ignored by git and must not be used for anything
beyond Developer Mode testing.
