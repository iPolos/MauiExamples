using Plugin.LocalNotification;

namespace MauiExamples.Examples.Components.LocalNotifications;

public partial class LocalNotificationPage : ContentPage
{
    public LocalNotificationPage()
    {
        InitializeComponent();
        
        // Request notification permission when page is loaded
        RequestNotificationPermission();
    }
    
    private async void RequestNotificationPermission()
    {
        // Request notification permission
        var result = await LocalNotificationCenter.Current.RequestNotificationPermission();
        if (!result)
        {
            await DisplayAlert("Permission Denied", 
                "Notification permission was denied. Please enable notifications in your device settings to use this feature.", 
                "OK");
        }
    }

    private async void OnBasicNotificationClicked(object sender, EventArgs e)
    {
        var title = string.IsNullOrEmpty(TitleEntry.Text) ? "Basic Notification" : TitleEntry.Text;
        var message = string.IsNullOrEmpty(MessageEntry.Text) ? "This is a notification from the MAUI app" : MessageEntry.Text;
        
        try
        {
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = title,
                Description = message,
                ReturningData = "Basic notification data",
                Android = new AndroidOptions
                {
                    ChannelId = "general_notifications",
                    ChannelName = "General Notifications",
                    Priority = AndroidPriority.High,
                    IconSmallName = "notification_icon",
                    Color = Android.Graphics.Color.Blue,
                    AutoCancel = true,
                    Ongoing = false
                }
            };
            
            await LocalNotificationCenter.Current.Show(notification);
            await DisplayAlert("Success", "Notification sent successfully", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to send notification: {ex.Message}", "OK");
        }
    }

    private async void OnScheduledNotificationClicked(object sender, EventArgs e)
    {
        var title = string.IsNullOrEmpty(TitleEntry.Text) ? "Scheduled Notification" : TitleEntry.Text;
        var message = string.IsNullOrEmpty(MessageEntry.Text) ? "This is a scheduled notification" : MessageEntry.Text;
        
        if (!int.TryParse(DelayEntry.Text, out int seconds))
        {
            await DisplayAlert("Error", "Please enter a valid number for delay", "OK");
            return;
        }
        
        try
        {
            var notification = new NotificationRequest
            {
                NotificationId = 200,
                Title = title,
                Description = message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(seconds)
                },
                ReturningData = "Scheduled notification data",
                Android = new AndroidOptions
                {
                    ChannelId = "scheduled_notifications",
                    ChannelName = "Scheduled Notifications",
                    Priority = AndroidPriority.High,
                    IconSmallName = "notification_icon",
                    Color = Android.Graphics.Color.Red,
                    AutoCancel = true
                }
            };
            
            await LocalNotificationCenter.Current.Show(notification);
            await DisplayAlert("Success", $"Notification scheduled in {seconds} seconds", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to schedule notification: {ex.Message}", "OK");
        }
    }

    private async void OnCancelNotificationsClicked(object sender, EventArgs e)
    {
        try
        {
            LocalNotificationCenter.Current.CancelAll();
            await DisplayAlert("Success", "All notifications have been cancelled", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to cancel notifications: {ex.Message}", "OK");
        }
    }
} 