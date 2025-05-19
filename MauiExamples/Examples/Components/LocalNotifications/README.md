# Local Notifications in .NET MAUI

This component demonstrates how to implement local notifications in a .NET MAUI application using the Plugin.LocalNotification package.

## Features

- **Basic Notifications**: Send immediate notifications with a title and message
- **Scheduled Notifications**: Schedule notifications to appear after a specific delay
- **Notification Management**: Cancel all pending notifications

## Implementation Details

The implementation uses the [Plugin.LocalNotification](https://github.com/thudugala/Plugin.LocalNotification) NuGet package which provides a cross-platform API for local notifications.

### Key Components

1. **Initialization**: The notification plugin is initialized in `MauiProgram.cs` using the `.UseLocalNotification()` method
2. **Basic Notification**: Creates and shows a notification immediately
3. **Scheduled Notification**: Creates a notification with a delayed delivery time
4. **Cancellation**: Provides functionality to cancel all pending notifications

### Usage

```csharp
// Send a basic notification
var notification = new NotificationRequest
{
    NotificationId = 100,
    Title = "Notification Title",
    Description = "Notification message",
    ReturningData = "Optional data to return",
    Android = new AndroidOptions
    {
        ChannelId = "general",
        Priority = AndroidPriority.High
    },
    iOS = new iOSOptions
    {
        PresentationOption = iOSPresentationOption.Alert | iOSPresentationOption.Sound
    }
};
await LocalNotificationCenter.Current.Show(notification);

// Schedule a notification
var scheduledNotification = new NotificationRequest
{
    NotificationId = 200,
    Title = "Scheduled Notification",
    Description = "This will appear later",
    Schedule = new NotificationRequestSchedule
    {
        NotifyTime = DateTime.Now.AddSeconds(5) // 5 seconds delay
    },
    Android = new AndroidOptions
    {
        ChannelId = "general",
        Priority = AndroidPriority.High
    },
    iOS = new iOSOptions
    {
        PresentationOption = iOSPresentationOption.Alert | iOSPresentationOption.Sound
    }
};
await LocalNotificationCenter.Current.Show(scheduledNotification);

// Cancel all notifications
LocalNotificationCenter.Current.CancelAll();
```

## Platform Considerations

### Android
- Notifications appear in the system tray
- User must grant notification permissions
- Requires proper notification channel setup

### iOS
- Notifications appear in the notification center
- User must grant notification permissions explicitly
- Requires additional configuration in Info.plist:
  ```xml
  <key>UIBackgroundModes</key>
  <array>
      <string>remote-notification</string>
  </array>
  ```
- For notifications to appear when the app is in the foreground, you must use a custom delegate and set the iOSPresentationOption

### iOS Troubleshooting
If notifications aren't appearing in the iOS simulator:

1. **Check permission**: Make sure you've called `await LocalNotificationCenter.Current.RequestNotificationPermission()` and the user approved
2. **Verify Info.plist**: Ensure UIBackgroundModes includes remote-notification
3. **Custom notification delegate**: Implement a custom iOSNotificationDelegate with WillPresentNotification method
4. **Simulator limitations**: Some versions of the iOS simulator may not show notifications correctly. In this case:
   - Try testing on a physical device
   - Check Debug console for any errors
   - Ensure the app is set to allow notifications in Settings
5. **Foreground notifications**: If the app is in the foreground, notifications won't appear unless you explicitly set iOSPresentationOption.Alert in your notification and delegate

### Windows
- Notifications appear in the Windows notification center

## Additional Resources

- [Plugin.LocalNotification Documentation](https://github.com/thudugala/Plugin.LocalNotification)
- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/) 