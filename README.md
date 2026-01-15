# Genies SDK VR Example (Meta Quest 3)

This example Unity project demonstrates the integration of the Genies’ and Meta’s SDK’s, and is buildable to Meta Quest 3.

## Key Unlocks for Devs

- Leverages Genies’ login systems, with UI configured to work in VR.
- Features pre-configured character retargeting, set up to work especially for Genies avatars.
- Hides face and head geometry from the VR camera to prevent clipping artifacts.
- Works with hand tracking as well as controllers.

## Requirements

- Unity **2022.3.62f2**
- Android build support installed via Unity Hub
- A Genies developer app (Client ID / Client Secret)
- Network access on first open (some packages restore from GitHub)

## What’s included

- Rendering: URP (`com.unity.render-pipelines.universal`)
- XR: OpenXR + XR Management (`com.unity.xr.openxr`, `com.unity.xr.management`)
- Meta: Meta XR SDK **83.0.1** (`com.meta.xr.sdk.core`, `com.meta.xr.sdk.interaction.ovr`)
- Movement sample package pulled via Git URL: `com.meta.xr.sdk.movement`
- Genies Avatar SDK: vendored under `Packages/com.genies.avatar-sdk.client/`

## Getting started

1. Open the project in Unity 2022.3.62f2 or later.
2. Wait for packages to resolve.
   - If you’re on a fresh clone, Unity may need to fetch `com.meta.xr.sdk.movement` from GitHub.
3. Run the Genies bootstrap wizard:
   - **Tools > Genies > SDK Bootstrap Wizard**

For Genies SDK configuration details (IL2CPP, .NET 4.8, ARM64, min API level 31, Vulkan, TMP essentials, Input System), see:
- `Packages/com.genies.avatar-sdk.client/README.md`

## Scenes

- `Assets/Scenes/Main.unity`: Core scene for testing.
- `Assets/Scenes/Clementine.unity`: Useful for character configuration.

## Runtime flow (high level)

- `Assets/Genies VR Example/LoginCanvasVR.cs`: initializes the Genies SDK and drives email-OTP login with a VR-friendly UI.
- `Assets/Genies VR Example/LoadAndBecomeMyAvatar.cs`: loads the logged-in user’s avatar.
- `Assets/Genies VR Example/GeniesAvatarControllerVR.cs`: hooks the loaded avatar into Meta retargeting and applies VR-specific rendering tweaks (including head/face hiding).
- `Assets/Genies VR Example/GeniesCharacterRetargeterForMeta.cs`: name-based bone mapping and retarget setup for Genies avatars.

## Building to Meta Quest 3

This repo is intended to be buildable to Quest 3.

Typical build checklist:

1. Switch platform to **Android** in **Build Settings**.
2. Ensure **ARM64** + **IL2CPP** are selected (the Genies bootstrap wizard will guide this).
3. Verify XR setup (OpenXR/XR Management + Meta XR packages are installed).
4. Build & Run to a Quest 3 device.

## Notes / gotchas

- Some avatar scaling and stabilization behaviors are device-only (Editor vs on-device behavior can differ).
- If VS Code search seems to “miss” Unity assets, check `.vscode/settings.json` workspace excludes.

## Third-party code redistribution

This repository redistributes some third-party code, specifically **Meta’s XR Samples** (and related Meta XR SDK content).

- Review the applicable third-party licenses and notices included with the Meta packages and any bundled sample content.
- The Genies Avatar SDK package also contains third-party notices under `Packages/com.genies.avatar-sdk.client/Internal/**/ThirdPartyNotices`.
