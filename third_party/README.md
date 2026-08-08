# third_party

Vendored external sources live here, one directory per dependency.

Rules:

- Every directory must contain the upstream licence file.
- Record the upstream URL and the exact revision that was vendored.
- Do not commit prebuilt binaries; they are ignored by `.gitignore`.
- Do not modify vendored sources in place. Keep local changes as patch files
  next to the source so they can be reapplied after an update.

This directory is currently empty.
