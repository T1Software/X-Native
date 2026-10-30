# Filesystem and Xbox catalogue wiring

The platform now registers `PhysicalFileSystemService` for `IFileSystemService`.
Previously `LocalGameCatalog` required that interface but no concrete implementation
was registered by the platform or Xbox host. The new regression test builds the
platform container with dependency validation and resolves the catalogue.

`TryAddSingleton` preserves a filesystem supplied by a host before platform registration.
All access remains subject to the current process permissions and UWP sandbox.
This implementation does not validate manifest path containment or bypass permissions.

The Xbox host overrides the desktop `MyDocuments` default with
`ApplicationData.Current.LocalFolder/Games`. Place game folders under that app-local
Games directory using your development deployment/file-transfer tools. The directory
is not created by registration; an absent directory produces an empty catalogue.
Executable discovery is still extension-based; the PE inspector is not yet wired in.
Discovery does not imply launch compatibility.

Text reads detect BOMs and otherwise use UTF-8. Cancellation is checked before opening
and after the asynchronous read, not during ReadToEndAsync on .NET Standard 2.0.
I/O and access failures propagate. Enumeration is immediate-directory-only; the
catalogue controls recursive scanning.

Portable tests cover DI registration, host overrides, top-level enumeration,
Unicode reads, file-handle release, cancellation and missing-file errors.
Run: `dotnet test tests/XNative.Platform.Tests/XNative.Platform.Tests.csproj -c Release`.
The Xbox-specific configuration still requires a UWP build and console test.
No successful build, test run, Xbox package or PC-game execution is claimed here.
