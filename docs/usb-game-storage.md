# USB game storage: ZIP browse and local import

## What this adds

The Xbox host has a USB game storage section below the existing library. It uses
KnownFolders.RemovableDevices and StorageFolder/StorageFile APIs, not guessed drive
letters or broadFileSystemAccess. Local game scanning remains available.

This is USB archive access and import, NOT direct-from-USB EXE/DLL access or execution.
The removableStorage capability is filtered by declared file types. The package
requests .zip access only; it does not attempt reserved .exe/.dll associations.
Opening ZIPs from another app is not implemented; use the in-app USB controls.

## Prepare and use

1. On your PC, create XNative/Games on a USB drive your Xbox can expose as removable media.
2. ZIP your game's files and place the ZIP in that folder (one package per game).
   Put game.exe and optionally game.json at the ZIP root, or within one enclosing folder.
3. Rebuild and redeploy the updated UWP package; an old package will not have USB capability.
4. Connect the USB drive, launch X-Native, and select Scan USB storage.
5. Choose a package and select Import selected ZIP. Imports copy to a unique app-local
   Games/usb-<id> folder; source USB files are never changed.
6. The catalogue refreshes after import. Discovery does not imply launch compatibility.

The browser lists ZIPs immediately inside XNative/Games on each exposed drive, not
nested archives or arbitrary files. It lists at most 500 packages. Rescan after
plugging in or replacing a drive. Missing folders and access errors are reported.
If a drive is unplugged during import, the error is shown and staging cleanup is attempted.
There is no automatic device watcher or background import. Leaving the page cancels
work at the next check; an in-flight WinRT request may finish first.

## Extraction safety and limits

- Source is read-only. Extraction happens off the UI thread into app-local Imports staging.
- Completed staging is moved into Games before the catalogue refresh.
- No overwrite of an existing destination; imports use unique IDs.
- Rejects traversal, absolute paths, alternate streams, unsafe filename characters,
  trailing dots/spaces, Windows device names, symbolic/reparse links and duplicate case-insensitive file paths.
- Limits: 4 GiB expanded data, 10000 ZIP entries and 32 path components per entry.
- Enforces the byte limit while copying, not just from declared entry sizes.
- Exceptions attempt to remove partial staging. Cleanup failures are reported where possible.
- Imports require enough free internal app storage. Out-of-space errors are surfaced.
- Files are untrusted data: extraction is not malware scanning or a compatibility guarantee.

Portable tests cover valid import, traversal variants, limits, duplicate names,
existing destinations, symbolic links, cancellation and stream ownership.
Run: dotnet test tests/XNative.Platform.Tests/XNative.Platform.Tests.csproj -c Release

## Verification boundary

The USB UI and WinRT enumeration require an Xbox UWP build and console testing.
Portable CI does not compile the UWP host. No passing build, console USB access,
PC-game execution or direct USB execution is claimed by this patch.

References:
https://learn.microsoft.com/en-us/uwp/api/windows.storage.knownfolders.removabledevices
https://learn.microsoft.com/en-us/windows/apps/develop/launch/reserved-uri-scheme-names
