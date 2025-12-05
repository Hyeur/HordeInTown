# Horde In Town

A Unity game project optimized for iOS development.

## Project Overview

Horde In Town is a Unity-based game project with assets and resources for iOS deployment.

## Project Structure

```
HordeInTown/
├── Assets/              # Unity project assets (scripts, scenes, prefabs)
├── ProjectSettings/     # Unity project settings
├── RawAssets/           # Raw game assets (images, sounds, etc.)
└── Packages/            # Unity package dependencies
```

## Setup Instructions

### Prerequisites

- Unity Hub installed
- Unity Editor 6000.3.0f1 (required)
- Xcode (for iOS builds)
- macOS (required for iOS builds)

### Initial Setup

1. Open Unity Hub
2. Click "Add" and select this project folder
3. Unity will detect the project and allow you to open it
4. If prompted, select Unity version 6000.3.0f1

### iOS Build Configuration

1. **Switch Build Platform:**
   - File → Build Settings
   - Select iOS platform
   - Click "Switch Platform"

2. **Player Settings (iOS):**
   - Edit → Project Settings → Player
   - Configure iOS-specific settings:
     - Bundle Identifier: `com.yourcompany.hordeintown`
     - Target minimum iOS Version: iOS 12.0 or later
     - Architecture: ARM64
     - Scripting Backend: IL2CPP (recommended for iOS)

3. **Build Requirements:**
   - macOS with Xcode installed
   - Apple Developer account (for device testing and App Store distribution)
   - Provisioning profiles and certificates configured in Xcode

### Importing Raw Assets

The `RawAssets/` folder contains:
- 2D sprites and UI elements
- Sound effects and background music
- Mock-up images

To import:
1. Open Unity
2. Drag and drop assets from `RawAssets/` into the `Assets/` folder
3. Or use the `.unitypackage` file: Assets → Import Package → Custom Package

## Development

### Git Workflow

This project uses Git for version control. Before pushing to remote:

1. Ensure all changes are committed:
   ```bash
   git add .
   git commit -m "Your commit message"
   ```

2. Create a remote repository on GitHub (if not already created)

3. Add remote and push:
   ```bash
   git remote add origin <your-repo-url>
   git branch -M main
   git push -u origin main
   ```

### Recommended Unity Settings

- **Version Control:** Visible Meta Files
  - Edit → Project Settings → Editor → Version Control Mode
- **Asset Serialization:** Force Text
  - Edit → Project Settings → Editor → Asset Serialization

## iOS-Specific Notes

- **Build Location:** Unity builds to Xcode project, then build in Xcode
- **Signing:** Configure code signing in Xcode after Unity build
- **Performance:** Use IL2CPP scripting backend for better iOS performance
- **Textures:** Optimize textures for iOS (use ASTC compression)
- **Audio:** Use compressed audio formats (AAC, MP3) for iOS

## Resources

- [Unity iOS Documentation](https://docs.unity3d.com/Manual/iphone.html)
- [Unity Build Settings](https://docs.unity3d.com/Manual/BuildSettings.html)

## License

[Add your license information here]

