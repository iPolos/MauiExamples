# Camera in .NET MAUI

This component demonstrates how to implement camera functionality in a .NET MAUI application, allowing users to capture photos and maintain a photo gallery.

## Features

- **Capture Photos**: Take pictures using the device's camera
- **Photo Gallery**: Display captured photos in a grid layout
- **Photo Management**: Easily delete unwanted photos
- **Persistent Storage**: Photos are saved to the app's local storage

## Implementation Details

The implementation uses MAUI's built-in `MediaPicker` API, which provides cross-platform access to the device's camera and photo capabilities.

### Key Components

1. **MediaPicker**: Access to device camera for capturing photos
2. **File System Access**: Saving and retrieving photos from local storage
3. **CollectionView with GridItemsLayout**: Display photos in a grid-based gallery
4. **ObservableCollection**: Maintain a dynamic collection of photos

### Usage

```csharp
// Check if camera is available
if (MediaPicker.IsCaptureSupported)
{
    // Capture photo
    var photo = await MediaPicker.CapturePhotoAsync();
    
    if (photo != null)
    {
        // Get photo stream
        using var stream = await photo.OpenReadAsync();
        
        // Process the photo (e.g., save to file, display in UI)
    }
}
```

## Platform Considerations

### Android
- Requires `CAMERA` permission in the Android Manifest
- Storage permissions are needed for saving photos
- **IMPORTANT**: For Android 11 (API level 30) and higher, you must add the `<queries>` element with the `IMAGE_CAPTURE` intent in the AndroidManifest.xml:

```xml
<!-- Required for Android 11 (API level 30) and higher -->
<queries>
    <intent>
        <action android:name="android.media.action.IMAGE_CAPTURE" />
    </intent>
</queries>
```

- The following permissions are also required:
```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" android:maxSdkVersion="32" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" android:maxSdkVersion="32" />
```

### iOS
- Requires `NSCameraUsageDescription` in Info.plist
- May require photo library permissions for saving photos
- Note: Camera is not available in iOS Simulator, special handling is provided to use a placeholder image instead

## File Storage

Photos are stored in the app's data directory:
- `FileSystem.AppDataDirectory/Photos/`

This ensures that:
1. Photos are private to the application
2. Photos persist between app sessions
3. Photos are removed when the app is uninstalled

## Troubleshooting

### Common Issues

1. **Camera Grayed Out or Not Available**
   - On iOS Simulator: This is expected behavior as the simulator doesn't have camera hardware. The app will use a placeholder image.
   - On Android: Ensure your AndroidManifest.xml has the proper `<queries>` element and camera permissions as shown above.

2. **Permission Denied**
   - Ensure that your app has been granted camera permissions in the device settings.

3. **Android Error: "No camera available"**
   - This usually indicates that the `<queries>` element is missing from your AndroidManifest.xml file.

## Additional Features to Consider

- **Photo Preview**: Display a preview before saving
- **Camera Settings**: Allow users to adjust camera settings
- **Photo Editing**: Basic editing capabilities like cropping or filters
- **Cloud Backup**: Sync photos with cloud storage
- **Sharing**: Share photos via platform sharing APIs

## Additional Resources

- [MediaPicker Documentation](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/media-picker)
- [File System in MAUI](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers)
- [CollectionView Documentation](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/)
- [Android Package Visibility](https://developer.android.com/training/package-visibility) 