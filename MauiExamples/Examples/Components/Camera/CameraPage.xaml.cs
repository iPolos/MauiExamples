using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace MauiExamples.Examples.Components.Camera;

public partial class CameraPage : ContentPage
{
    private ObservableCollection<PhotoItem> _photos;
    private readonly string _photoFolderPath;
    private bool IsSimulator => 
#if __IOS__
        DeviceInfo.Current.DeviceType == DeviceType.Virtual;
#else
        false;
#endif
    
    public CameraPage()
    {
        InitializeComponent();
        
        // Create folder for storing photos
        _photoFolderPath = Path.Combine(FileSystem.AppDataDirectory, "Photos");
        if (!Directory.Exists(_photoFolderPath))
        {
            Directory.CreateDirectory(_photoFolderPath);
        }
        
        // Initialize photo collection
        _photos = new ObservableCollection<PhotoItem>();
        PhotoCollection.ItemsSource = _photos;
        
        // Load existing photos
        LoadSavedPhotos();
        
        // Show a message if running in iOS simulator
        if (IsSimulator)
        {
            DisplayAlert("iOS Simulator", "Camera hardware is not available in the iOS simulator. A placeholder image will be used when taking photos.", "OK");
        }
        
        // Check if camera is supported
        CheckCameraSupport();
    }

    private async void CheckCameraSupport()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Required", 
                        "Camera permission is required to take photos. Please enable it in the app settings.", 
                        "OK");
                }
            }
            
            // Check if camera capture is supported
            if (!MediaPicker.IsCaptureSupported)
            {
                await DisplayAlert("Not Supported", 
                    "Camera is not supported on this device or the required manifest configuration is missing.", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking camera permissions: {ex.Message}");
        }
    }
    
    private void LoadSavedPhotos()
    {
        try
        {
            if (Directory.Exists(_photoFolderPath))
            {
                // Get all image files from the directory
                var imageFiles = Directory.GetFiles(_photoFolderPath, "*.jpg")
                    .OrderByDescending(f => new FileInfo(f).CreationTime);
                
                // Load each image into the collection
                foreach (var imageFile in imageFiles)
                {
                    var fileInfo = new FileInfo(imageFile);
                    _photos.Add(new PhotoItem
                    {
                        FilePath = imageFile,
                        DateTaken = fileInfo.CreationTime
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading photos: {ex.Message}");
        }
    }
    
    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        if (IsSimulator)
        {
            await HandleSimulatorPhoto();
            return;
        }

        if (!MediaPicker.IsCaptureSupported)
        {
            await DisplayAlert("Not Supported", 
                "Camera is not supported on this device. This could be due to:\n\n" +
                "1. No camera hardware detected\n" +
                "2. Missing <queries> element in AndroidManifest.xml\n" +
                "3. Missing camera permissions", 
                "OK");
            return;
        }
        
        try
        {
            // First ensure we have camera permission
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", 
                    "Camera permission is required to take photos.", 
                    "OK");
                return;
            }
            
            // Take photo
            var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Take Photo"
            });
            
            if (photo != null)
            {
                // Create unique filename
                string fileName = $"photo_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                string filePath = Path.Combine(_photoFolderPath, fileName);
                
                // Save the photo to our app's directory
                using (var sourceStream = await photo.OpenReadAsync())
                using (var destinationStream = File.Create(filePath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
                
                // Add to collection
                var photoItem = new PhotoItem
                {
                    FilePath = filePath,
                    DateTaken = DateTime.Now
                };
                
                _photos.Insert(0, photoItem);
            }
        }
        catch (Exception ex)
        {
            string errorMessage = ex.Message;
            
            // Check for specific error message about missing manifest entry
            if (errorMessage.Contains("android.media.action.IMAGE_CAPTURE") || 
                errorMessage.Contains("no camera") ||
                errorMessage.Contains("manifest"))
            {
                errorMessage = "Camera access error: The Android manifest file is missing required configuration. " +
                              "Please ensure IMAGE_CAPTURE action is properly defined in the <queries> element.";
            }
            
            Debug.WriteLine($"Camera error: {ex}");
            await DisplayAlert("Camera Error", errorMessage, "OK");
        }
    }

    private async Task HandleSimulatorPhoto()
    {
        try
        {
            // Use a simulated photo for testing in the simulator
            string fileName = $"simulator_photo_{DateTime.Now:yyyyMMddHHmmss}.jpg";
            string filePath = Path.Combine(_photoFolderPath, fileName);
            
            // Create a simulated photo using a bundled image
            // First try to copy an existing image from the app's resources
            var assembly = GetType().Assembly;
            using (var destinationStream = File.Create(filePath))
            {
                try 
                {
                    // Try to use the phone.png image from the app resources
                    using (var sourceStream = await FileSystem.OpenAppPackageFileAsync("phone.png"))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                }
                catch 
                {
                    try 
                    {
                        // Fallback to dotnet_bot.png if phone.png isn't available
                        using (var sourceStream = await FileSystem.OpenAppPackageFileAsync("dotnet_bot.png"))
                        {
                            await sourceStream.CopyToAsync(destinationStream);
                        }
                    }
                    catch 
                    {
                        // If all else fails, create a colored square as a placeholder
                        // Creating a simple colored square bitmap (100x100 pixels) in memory
                        byte[] buffer = new byte[100 * 100 * 3]; // RGB format, 3 bytes per pixel
                        for (int i = 0; i < buffer.Length; i += 3)
                        {
                            // Set RGB values for a blue color
                            buffer[i] = 0;     // R
                            buffer[i + 1] = 0; // G
                            buffer[i + 2] = 255; // B
                        }
                        await destinationStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
            }
            
            // Add to collection
            var photoItem = new PhotoItem
            {
                FilePath = filePath,
                DateTaken = DateTime.Now
            };
            
            _photos.Insert(0, photoItem);
            
            await DisplayAlert("Simulator", "Using a placeholder image in iOS simulator (camera not available in simulator)", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Simulator photo error: {ex}");
            await DisplayAlert("Simulator Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
    
    private async void OnPhotoSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is PhotoItem selectedPhoto)
        {
            // Clear selection
            PhotoCollection.SelectedItem = null;
            
            // Create a callback for when a photo is deleted
            Action deleteCallback = () => {
                _photos.Remove(selectedPhoto);
            };
            
            // Navigate to detail page
            var detailPage = new PhotoDetailPage(
                selectedPhoto.FilePath, 
                selectedPhoto.DateTaken,
                deleteCallback);
                
            await Navigation.PushAsync(detailPage);
        }
    }
}

public class PhotoItem
{
    public string FilePath { get; set; }
    public DateTime DateTaken { get; set; }
} 