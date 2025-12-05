# iOS Build Setup Guide

This guide provides detailed instructions for building and deploying Horde In Town to iOS devices.

## Prerequisites

### Required Software
- **Unity Editor 6000.3.0f1** (required)
- **macOS** (required - iOS builds cannot be done on Windows)
- **Xcode** (latest version from Mac App Store)
- **Apple Developer Account** ($99/year for App Store distribution, free for personal device testing)

### System Requirements
- macOS 12.0 or later
- Xcode 14.0 or later
- iOS deployment target: iOS 12.0 or later (recommended)

## Unity Configuration

### 1. Switch Build Platform

1. Open Unity Editor
2. Go to **File → Build Settings**
3. Select **iOS** from the platform list
4. Click **Switch Platform** (this may take a few minutes)

### 2. Player Settings for iOS

Navigate to **Edit → Project Settings → Player**

#### General Settings
- **Company Name:** Your company name
- **Product Name:** Horde In Town
- **Default Icon:** Set your app icon

#### iOS Settings Tab

**Other Settings:**
- **Bundle Identifier:** `com.yourcompany.hordeintown` (must be unique)
- **Target minimum iOS Version:** iOS 12.0 (or later)
- **Target Device Family:** iPhone (or iPhone + iPad if supporting both)
- **Requires ARKit support:** Uncheck unless using AR features

**Configuration:**
- **Scripting Backend:** IL2CPP (recommended for iOS performance)
- **Api Compatibility Level:** .NET Standard 2.1
- **Architecture:** ARM64 (required for App Store)

**Optimization:**
- **Strip Engine Code:** Enabled (reduces build size)
- **Managed Stripping Level:** Low/Medium (test thoroughly)

**Signing:**
- **Automatically Sign:** Can be enabled, but manual signing in Xcode is recommended for first builds

### 3. Texture Compression

For iOS, use ASTC compression for best performance:

1. Select textures in Project window
2. In Inspector, set:
   - **Texture Type:** Sprite (2D and UI) or Default
   - **Compression:** ASTC 4x4 block (or 6x6 for better quality)
   - **Max Size:** Appropriate for your use case (1024, 2048, etc.)

### 4. Audio Settings

1. Select audio clips
2. In Inspector:
   - **Load Type:** Compressed In Memory (for music) or Streaming (for large files)
   - **Compression Format:** Vorbis (recommended) or MP3
   - **Quality:** 70-90% (balance size vs quality)

## Building the Project

### Step 1: Build from Unity

1. **File → Build Settings**
2. Ensure **iOS** is selected
3. Click **Build** (or **Build and Run** if Xcode is on same machine)
4. Choose a folder for the Xcode project (e.g., `Builds/iOS`)
5. Unity will generate an Xcode project

### Step 2: Configure in Xcode

1. Open the generated `.xcodeproj` file in Xcode
2. Select the project in the navigator
3. Go to **Signing & Capabilities** tab

#### Code Signing
- **Team:** Select your Apple Developer team
- **Bundle Identifier:** Should match Unity settings
- **Automatically manage signing:** Enable (or manually configure provisioning profiles)

#### Capabilities (if needed)
- Push Notifications
- Game Center
- In-App Purchase
- etc.

### Step 3: Build Settings in Xcode

1. Select your target
2. Go to **Build Settings** tab
3. Important settings:
   - **iOS Deployment Target:** Match Unity's minimum iOS version
   - **Code Signing Identity:** Your developer certificate
   - **Provisioning Profile:** Your app's provisioning profile

### Step 4: Build and Run

#### For Simulator
1. Select a simulator from the device dropdown
2. Click **Play** button or press `Cmd + R`

#### For Physical Device
1. Connect your iOS device via USB
2. Trust the computer on your device
3. Select your device from the device dropdown
4. Click **Play** button
5. On device: Settings → General → VPN & Device Management → Trust Developer

## Common Issues and Solutions

### Issue: "No signing certificate found"
**Solution:** 
- Ensure you're logged into Xcode with your Apple ID
- Go to Xcode → Preferences → Accounts
- Add your Apple ID and download certificates

### Issue: Build fails with code signing errors
**Solution:**
- Check Bundle Identifier matches in both Unity and Xcode
- Ensure provisioning profile is valid
- Try cleaning build folder: Product → Clean Build Folder

### Issue: App crashes on launch
**Solution:**
- Check Xcode console for errors
- Verify all required capabilities are enabled
- Check Info.plist for required permissions

### Issue: Large build size
**Solution:**
- Enable "Strip Engine Code" in Unity
- Use appropriate texture compression
- Remove unused assets
- Use Asset Bundles for optional content

## Performance Optimization for iOS

1. **Use IL2CPP:** Better performance than Mono
2. **Texture Compression:** ASTC 4x4 or 6x6
3. **Audio Compression:** Vorbis or MP3
4. **Frame Rate:** Target 60 FPS, use Quality Settings
5. **Batching:** Enable Static Batching and Dynamic Batching
6. **Occlusion Culling:** Enable for 3D scenes
7. **LOD Groups:** Use for complex 3D models

## Testing Checklist

Before submitting to App Store:

- [ ] App launches without crashes
- [ ] All UI elements display correctly on different screen sizes
- [ ] Audio plays correctly
- [ ] Touch controls work properly
- [ ] Performance is acceptable (60 FPS target)
- [ ] No memory leaks (test with Instruments)
- [ ] App handles interruptions (calls, notifications)
- [ ] App resumes correctly after backgrounding
- [ ] Tested on multiple iOS versions
- [ ] Tested on different device models (iPhone SE, iPhone 14 Pro, etc.)

## App Store Submission

1. **Archive the build:**
   - Product → Archive in Xcode
   - Wait for archive to complete

2. **Distribute:**
   - Window → Organizer
   - Select your archive
   - Click "Distribute App"
   - Follow the wizard

3. **App Store Connect:**
   - Create app listing
   - Upload build
   - Submit for review

## Additional Resources

- [Unity iOS Documentation](https://docs.unity3d.com/Manual/iphone.html)
- [Apple Developer Documentation](https://developer.apple.com/documentation/)
- [Xcode Help](https://help.apple.com/xcode/)

