# File System Demo

This component demonstrates various file system operations in .NET MAUI applications, highlighting cross-platform file access patterns that work on both iOS and Android.

## Features

The demo is organized into four main sections:

### 1. Basic File Operations

- Create, read, update, and delete text files in the app's data directory
- Display filesystem information and paths
- List files in the app's data directory

### 2. App-Specific Storage

- Preferences API for storing simple key-value pairs
- SecureStorage API for storing sensitive information
- Cache directory for temporary files

### 3. Media Storage

- Capture photos using the device camera
- Save images to the device's gallery
- Create files in the device's public directories (Downloads)

### 4. File Pickers

- Select single files with filtering by type
- Select multiple files
- Select folders and explore their contents

## Platform-Specific Considerations

### Android

- Requires explicit permissions in AndroidManifest.xml:
  - CAMERA permission for photo capture
  - READ_EXTERNAL_STORAGE and WRITE_EXTERNAL_STORAGE for accessing public directories
- Uses Android-specific APIs for accessing public directories
- Requires media scanner notification for files added to public directories

### iOS

- Uses NSData and UIKit for saving images to the photo album
- More restrictive access to the file system; primarily uses app sandbox
- No direct access to system Download directory; uses Documents folder instead

## Usage Notes

1. Ensure proper permissions are requested before accessing the camera or external storage
2. Use platform-specific code only when necessary, with proper conditional compilation
3. Follow iOS sandbox rules for file access
4. Always handle exceptions for file operations, as they may fail for many reasons
5. Consider using .NET MAUI's built-in file APIs before using platform-specific code

## Integration

To use this component in your own MAUI application:

1. Add the required permissions to the platform-specific manifest files
2. Copy the FileSystemPage.xaml and FileSystemPage.xaml.cs files to your project
3. Update namespace references as needed
4. Add navigation to the page in your app 