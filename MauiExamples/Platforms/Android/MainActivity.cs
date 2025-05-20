using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Plugin.LocalNotification;

namespace MauiExamples;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Handle notification when app is launched via notification
        LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
    }

    private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
    {
        // Handle notification tap action here
        System.Diagnostics.Debug.WriteLine($"Notification tapped: ID={e.Request.NotificationId}, Title={e.Request.Title}");
    }
    
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}