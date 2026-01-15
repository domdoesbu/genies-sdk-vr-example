# Genies SDK VR Example (Meta Quest 3)

This example Unity project demonstrates the integration of the Genies’ and Meta’s SDKs, and is buildable to Meta Quest 3.

## Key Unlocks for Devs

- Leverages Genies’ login systems, with UI configured to work in VR.
- Features pre-configured character retargeting, set up to work especially for Genies avatars.
- Hides face and head geometry from the VR camera to prevent clipping artifacts.
- Works with hand tracking as well as controllers.

## Requirements

- Unity **2022.3.62f2**
- Android build support installed via Unity Hub
- Git installed and accessible to Unity/UPM (required to fetch the Meta Movement SDK Git dependency)
- Network access on first open (some packages restore from GitHub)

## What’s included

- Unity packages (remote UPM dependencies, auto-downloaded; not redistributed):
   - URP (`com.unity.render-pipelines.universal`)
   - OpenXR (`com.unity.xr.openxr`)
   - XR Management (`com.unity.xr.management`)
- Meta XR SDKs (remote UPM dependencies, auto-downloaded; not redistributed):
   - Core (`com.meta.xr.sdk.core`) **83.0.1**
   - Interaction (`com.meta.xr.sdk.interaction.ovr`) **83.0.1**
   - Movement (`com.meta.xr.sdk.movement`, UPM Git dependency)
- Genies Avatar SDK **1.3.1** (vendored UPM package; redistributed): `Packages/com.genies.avatar-sdk.client/`

## Getting started

1. Open the project in Unity 2022.3.62f2 or later.
2. Wait for packages to resolve.
   - If you’re on a fresh clone, Unity will need to fetch `com.meta.xr.sdk.movement` from GitHub (requires Git).
3. Run the Genies bootstrap wizard:
   - **Tools > Genies > SDK Bootstrap Wizard**
4. If you haven't done so already, create an account and customize a Genie in the AvatarStarter unity scene in the Genies SDK.

For Genies SDK configuration details (IL2CPP, .NET 4.8, ARM64, min API level 31, Vulkan, TMP essentials, Input System), see:
- `Packages/com.genies.avatar-sdk.client/README.md`

## Account & avatar prerequisites (recommended)

- A VR-friendly avatar editor is **not** included in this example project. (A developer should be able to port/adapt the avatar editor experience from the Genies SDK, if needed.)

## Scenes

- `Assets/Scenes/Main.unity`: Core scene for testing. Use the UI to login, and wait for your Genie to spawn.
- `Assets/Scenes/StagingAndConfiguration.unity`: Useful for authoring and testing avatar retargeting.

## Runtime flow (high level)

- `Assets/Genies VR Example/LoginCanvasVR.cs`: initializes the Genies SDK and drives email-OTP login with a VR-friendly UI.
- `Assets/Genies VR Example/LoadAndBecomeMyAvatar.cs`: loads the logged-in user’s avatar.
- `Assets/Genies VR Example/GeniesAvatarControllerVR.cs`: hooks the loaded avatar into Meta retargeting and applies VR-specific rendering tweaks (including head/face hiding).
- `Assets/Genies VR Example/GeniesCharacterRetargeterForMeta.cs`: name-based bone mapping and retarget setup for Genies avatars.

## Building to Meta Quest 3

This repo is intended to be buildable to Quest 3.

To build to Quest 3, follow the guidelines from the Meta Quest documentation: https://developers.meta.com/horizon/documentation/unity/unity-build/

## Notes / gotchas

- Some avatar scaling and stabilization behaviors are device-only (Editor vs on-device behavior can differ).
- If VS Code search seems to “miss” Unity assets, check `.vscode/settings.json` workspace excludes.
- Presently, the Genies SDK is not yet compatible with the Genies Party iOS app. Accounts and Genies cannot transfer between Genies Party and apps made with the Genies SDK. Genies plans to address this in a future update.

## Third-party code redistribution

This repository does **not** redistribute any of the Meta XR SDK packages themselves.
Core, Interaction, and Movement are remote dependencies that Unity’s Package Manager will auto-download when you open the project.

The one Package Manager dependency that *is* redistributed is the **Genies Avatar SDK** package, which is directly included under `Packages/com.genies.avatar-sdk.client/`.

It **does** include and redistribute various **Meta XR Samples**, sourced from:

- Meta XR Core SDK
- Meta XR Interaction SDK
- Meta XR Movement SDK

Notes:

- Review the applicable third-party licenses and notices included with the Meta packages and any bundled sample content.
- The Genies Avatar SDK package also contains third-party notices under `Packages/com.genies.avatar-sdk.client/Internal/**/ThirdPartyNotices`.
