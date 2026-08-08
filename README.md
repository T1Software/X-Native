# X-Native

> **Run PC games on Xbox Dev Mode.**
>
> An open-source compatibility layer and launcher that aims to bring Steam, Epic Games, and other Windows PC games to Xbox consoles running Developer Mode.

> **Status:** 🚧 Early Development

---

# What is X-Native?

X-Native is an experimental compatibility platform designed to make running Windows PC games on **Xbox Dev Mode (UWP)** possible.

Rather than emulating a PC, X-Native aims to provide a compatibility environment that allows supported games to run through native translation layers and platform abstractions.

The project is inspired by the philosophy behind projects like Wine, Proton, DXVK, and Box86/64—but is built specifically for Xbox Developer Mode.

---

# Vision

Imagine launching:

- 🎮 Steam Games
- 🎮 Epic Games Store Games
- 🎮 DRM-Free Windows Games
- 🎮 Fan Games
- 🎮 Indie Titles

...directly from an Xbox Dev Mode application.

That's the long-term vision.

---

# Goals

- Native UWP launcher
- Windows API compatibility layer
- Graphics abstraction
- Input translation
- Audio translation
- File system virtualization
- Plugin architecture
- Game profiles
- Performance-focused design
- Open-source from day one

---

# Planned Architecture

```
                 Xbox Dev Mode
                       │
                X-Native Launcher
                       │
        ┌──────────────┴──────────────┐
        │                             │
 Compatibility Layer           Plugin System
        │                             │
 ┌──────┴────────┐           ┌────────┴────────┐
 │               │           │                 │
Graphics      Audio       Community Plugins  Tools
 │               │
 Input      File System
 │               │
 └───────────────┴───────────────┐
                                 │
                          Windows PC Games

```

---

# Current Roadmap

## Phase 1 — Foundation

-  Project architecture
-  Core libraries
-  UWP host application
-  Logging system
-  Configuration system
-  Plugin framework

---

## Phase 2 — Compatibility Layer

-  Win32 abstraction
-  Process management
-  Memory management
-  Threading
-  DLL loading
-  File system virtualization

---

## Phase 3 — Graphics

-  Graphics abstraction
-  DirectX translation research
-  Shader pipeline
-  Texture management

---

## Phase 4 — Input & Audio

-  Xbox controller support
-  Keyboard & mouse abstraction
-  Audio backend
-  XAudio compatibility research

---

## Phase 5 — Game Support

-  Game profiles
-  Compatibility database
-  Launcher UI
-  Save management

---

# Current Status

This project is **very early** in development.

Expect:

- Frequent API changes
- Experimental code
- Incomplete features
- Breaking changes

Nothing should currently be considered stable.

---

# Why Open Source?

Projects like this are too large for one developer alone.

By developing in the open, we hope to:

- Share knowledge
- Encourage experimentation
- Welcome contributors
- Build useful tools for the community
- Preserve compatibility research

---

# Contributing

Contributions of all sizes are welcome.

You don't need to be an emulator expert to help.

Ways to contribute include:

- Documentation
- Bug reports
- Testing
- C#
- C++
- UWP development
- Graphics research
- Reverse engineering
- Performance optimization
- Build systems
- CI/CD
- UI/UX improvements

If you're new to open source, beginner-friendly issues will be labeled accordingly.

---

# Principles

X-Native is built around a few core principles:

- 🟢 Open development
- 🟢 Clean architecture
- 🟢 Modular design
- 🟢 Transparency
- 🟢 Community collaboration
- 🟢 Long-term maintainability

---

# Disclaimer

X-Native is an independent open-source project and is **not affiliated with or endorsed by Microsoft, Valve, Epic Games, or any game publisher**.

Users are responsible for obtaining and using legally acquired copies of any games they run with X-Native.

---

# License

This project is licensed under the **MIT License**.

---

# Join the Journey

X-Native is an ambitious, long-term project.

There are no promises about what games will work or when. Progress will happen one milestone at a time, and every improvement—no matter how small—brings the project closer to its goal.

Whether you're here to contribute code, report bugs, test builds, improve documentation, or simply follow development, you're welcome. 🚀
