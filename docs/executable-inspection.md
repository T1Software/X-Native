# Executable inspection: first fork milestone

`XNative.Platform.Games.PeExecutableInspector.Inspect(Stream)` provides bounded,
read-only PE header diagnostics for future catalogue and launch preflight integration.
It does not load or execute code. The existing catalogue and game runner are unchanged.

## Implemented

- Checks MZ/PE signatures, file offsets, optional-header size and PE32/PE32+ magic.
- Reports x86, x64, ARM, ARM64 or unknown COFF machine values.
- Rejects known machine/format mismatches and malformed section-table boundaries.
- Bounds data-directory counts against the declared optional-header size.
- Reports DLL and executable-image characteristics independently.
- Restores the input position and leaves the stream open, including on invalid input.
- Uses fixed-size primitive reads, not allocations controlled by untrusted header sizes.

`HasValidHeaders` means only that these structural checks passed. It does not
validate section data, imports, relocations, entry points, signatures or code.
Unknown architectures are reported, not silently treated as x64. I/O errors
propagate; callers must handle inaccessible or concurrently changed files.
The stream must be readable and seekable and must not be concurrently mutated.

## Tests and CI

Run `dotnet test tests/XNative.Platform.Tests/XNative.Platform.Tests.csproj -c Release`.
The portable workflow tests each existing test project on Windows and Linux and
builds the development application. It deliberately excludes the legacy UWP host.
No Xbox package, console execution or successful CI result is claimed by this patch.

## Next milestones

1. Wire inspection into catalogue/preflight diagnostics through the filesystem abstraction.
2. Add Xbox x64 packaging and persistent on-console diagnostics.
3. Define and implement a minimal execution backend with a controlled test binary.

PC-game execution, Wine/Proton, graphics translation and arbitrary EXE launching
are not implemented by this milestone.

Format reference: https://learn.microsoft.com/en-us/windows/win32/debug/pe-format
