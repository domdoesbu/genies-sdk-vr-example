# Copilot instructions (genies-sdk-vr-example)

## Project at a glance
- Unity project (URP + XR): Unity **2022.3.62f2** (see `ProjectSettings/ProjectVersion.txt`).
- VR stack: **Meta XR SDK 83.0.1** + OpenXR/XR Management (see `Packages/manifest.json`).
- `com.meta.xr.sdk.movement` is pulled from a GitHub URL via UPM; fresh clones need network access for package restore.
- Genies Avatar SDK is vendored as a UPM package under `Packages/com.genies.avatar-sdk.client/`.

## Where to start reading
- VR example entry points live in `Assets/Genies VR Example/`:
  - `LoginCanvasVR.cs`: initializes `AvatarSdk`, drives email-OTP login via `AvatarSdk.Events.*`.
  - `LoadAndBecomeMyAvatar.cs`: loads the logged-in user’s `ManagedAvatar` via `AvatarSdk.LoadUserAvatarAsync(...)`.
  - `GeniesAvatarControllerVR.cs`: wires the loaded avatar into Meta retargeting + VR-specific rendering tweaks.
- Scenes: `Assets/Scenes/Main.unity` and `Assets/Scenes/Clementine.unity`.

## Runtime flow (important data/control paths)
- App boot (scene): `LoginCanvasVR.Awake()` → `await AvatarSdk.InitializeAsync()` → `TryInstantLoginAsync()`.
- Login is event-driven: button handlers call `AvatarSdk.StartLoginEmailOtpAsync(...)` / `SubmitEmailOtpCodeAsync(...)`, UI transitions happen in `AvatarSdk.Events.*` callbacks.
- After login: `LoadAndBecomeMyAvatar.LoadAvatar()` → `AvatarSdk.LoadUserAvatarAsync(parent: ...)` → `GeniesAvatarControllerVR.InitializeWithLoadedAvatar(...)`.
- Retargeting: controller enables `GeniesCharacterRetargeterForMeta` (extends Meta `CharacterRetargeter`) + `MetaSourceDataProvider`.
  - Bone mapping is name-based and manually assembled; if avatar skeleton names change, update `GeniesCharacterRetargeterForMeta`.

## VR-specific conventions / gotchas
- Don’t assume parenting works for the avatar root: the sample manually syncs avatar root position/rotation to the retargeter every `Update()` (see `GeniesAvatarControllerVR`).
- Device vs Editor differences matter:
  - Avatar scaling is applied only on-device (`if (Application.isEditor) return;`), and only when changes exceed a threshold.
  - When scale changes, the sample re-stabilizes physics/hair via `DynamicsStructure.RequestPrewarmOnNextFrame()`.
- Shader stripping on Android/Quest is a real concern: prefer serialized `Shader` references for runtime swaps (see `_skinShaderWithInvisibleHeadSupport` in `GeniesAvatarControllerVR`).

## Genies SDK setup workflow (source of truth)
- Use the Bootstrap Wizard: **Tools > Genies > SDK Bootstrap Wizard** (documented in `Packages/com.genies.avatar-sdk.client/README.md`).
- Common required player settings (especially for Quest/Android) are called out in that README: IL2CPP, .NET 4.8, ARM64, min API 31, Vulkan, TMP essentials, Input System.
- Credentials live in **Project Settings > Genies > Auth Settings**.

## Debugging / editor workflow
- VS Code is set up to attach to Unity via `visualstudiotoolsforunity.vstuc` (see `.vscode/launch.json`).
- Note: this repo’s `.vscode/settings.json` hides many Unity asset types (`*.unity`, `*.asset`, `ProjectSettings/`, etc.). If you need to inspect those, temporarily disable excludes or use tools that search ignored files.

## IntelliSense / assemblies note
- Unity is responsible for generating assemblies and the C# project files. When new scripts are added or moved, VS Code IntelliSense may be temporarily out of date until Unity recompiles.
- If types/files aren’t being recognized after script changes, ask the developer to bring the Unity Editor into focus so it can compile and regenerate assemblies/project files.

## Coding guidelines (for agents)
- Keep changes minimal and targeted; avoid over-engineering or excessive edge-case guarding unless the surrounding code already does it.
- Avoid in-line `if` statements. Prefer brace-wrapped blocks even for early returns.
  - Avoid:
    - `if (something) return;`
  - Prefer:
    - `if (something)`
    - `{`
    - `    return;`
    - `}`
