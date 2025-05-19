using System.Diagnostics;
using System.Text;

namespace MauiExamples.Examples.Components.FileSystem;

public partial class FileSystemPage : ContentPage
{
    // Selected file path for media operations
    private string _currentImagePath;
    
    // Colors for tab selection
    private readonly Color _selectedTabColor;
    private readonly Color _unselectedTabColor = Colors.LightGray;
    
    public FileSystemPage()
    {
        InitializeComponent();
        
        // Get primary color from resources
        if (Application.Current?.Resources.TryGetValue("Primary", out var primaryColor) && 
            primaryColor is Color color)
        {
            _selectedTabColor = color;
        }
        else
        {
            _selectedTabColor = Colors.Blue; // Fallback color
        }
        
        // Default to the first tab
        OnBasicOperationsClicked(BasicOperationsButton, EventArgs.Empty);
        
        // Show basic filesystem info on load
        OnShowFileSystemInfoClicked(null, null);
    }
    
    #region Tab Navigation
    
    private void ResetTabButtons()
    {
        if (BasicOperationsButton == null || AppDataButton == null || 
            MediaButton == null || PickerButton == null ||
            BasicOperationsView == null || AppDataView == null || 
            MediaView == null || PickerView == null)
            return;
            
        BasicOperationsButton.BackgroundColor = _unselectedTabColor;
        AppDataButton.BackgroundColor = _unselectedTabColor;
        MediaButton.BackgroundColor = _unselectedTabColor;
        PickerButton.BackgroundColor = _unselectedTabColor;
        
        BasicOperationsView.IsVisible = false;
        AppDataView.IsVisible = false;
        MediaView.IsVisible = false;
        PickerView.IsVisible = false;
    }
    
    private void OnBasicOperationsClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (BasicOperationsButton != null) BasicOperationsButton.BackgroundColor = _selectedTabColor;
            if (BasicOperationsView != null) BasicOperationsView.IsVisible = true;
        }
        catch (Exception ex)
        {
            DisplayError($"Error switching to Basic tab: {ex.Message}");
        }
    }
    
    private void OnAppDataClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (AppDataButton != null) AppDataButton.BackgroundColor = _selectedTabColor;
            if (AppDataView != null) AppDataView.IsVisible = true;
        }
        catch (Exception ex)
        {
            DisplayError($"Error switching to App Storage tab: {ex.Message}");
        }
    }
    
    private void OnMediaClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (MediaButton != null) MediaButton.BackgroundColor = _selectedTabColor;
            if (MediaView != null) MediaView.IsVisible = true;
        }
        catch (Exception ex)
        {
            DisplayError($"Error switching to Media tab: {ex.Message}");
        }
    }
    
    private void OnPickerClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (PickerButton != null) PickerButton.BackgroundColor = _selectedTabColor;
            if (PickerView != null) PickerView.IsVisible = true;
        }
        catch (Exception ex)
        {
            DisplayError($"Error switching to Pickers tab: {ex.Message}");
        }
    }
    
    #endregion
    
    #region Basic File Operations
    
    private async void OnWriteFileClicked(object sender, EventArgs e)
    {
        try
        {
            if (FileNameEntry == null || FileContentEditor == null || OutputLabel == null)
                return;
                
            string fileName = FileNameEntry.Text?.Trim();
            string content = FileContentEditor.Text?.Trim();
            
            if (string.IsNullOrEmpty(fileName))
            {
                await DisplayAlert("Error", "Please enter a file name", "OK");
                return;
            }
            
            // Get the app data directory
            string targetDirectory = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(targetDirectory, fileName);
            
            // Write the file
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            await Task.Run(() => File.WriteAllText(filePath, content ?? string.Empty));
            
            OutputLabel.Text = $"File successfully written to: {filePath}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error writing file: {ex.Message}");
        }
    }
    
    private async void OnReadFileClicked(object sender, EventArgs e)
    {
        try
        {
            if (FileNameEntry == null || FileContentEditor == null || OutputLabel == null)
                return;
                
            string fileName = FileNameEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(fileName))
            {
                await DisplayAlert("Error", "Please enter a file name to read", "OK");
                return;
            }
            
            // Get file path
            string targetDirectory = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(targetDirectory, fileName);
            
            if (!File.Exists(filePath))
            {
                OutputLabel.Text = $"File not found: {filePath}";
                return;
            }
            
            // Read the file
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            string content = await Task.Run(() => File.ReadAllText(filePath));
            
            FileContentEditor.Text = content;
            OutputLabel.Text = $"File read successfully from: {filePath}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error reading file: {ex.Message}");
        }
    }
    
    private async void OnAppendFileClicked(object sender, EventArgs e)
    {
        try
        {
            if (FileNameEntry == null || FileContentEditor == null || OutputLabel == null)
                return;
                
            string fileName = FileNameEntry.Text?.Trim();
            string content = FileContentEditor.Text?.Trim();
            
            if (string.IsNullOrEmpty(fileName))
            {
                await DisplayAlert("Error", "Please enter a file name", "OK");
                return;
            }
            
            // Get file path
            string targetDirectory = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(targetDirectory, fileName);
            
            // Append to the file
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            await Task.Run(() => {
                // Create a newline before appending if file exists and doesn't end with a newline
                string appendContent = content ?? string.Empty;
                if (File.Exists(filePath) && File.ReadAllText(filePath).Length > 0)
                {
                    appendContent = Environment.NewLine + appendContent;
                }
                
                File.AppendAllText(filePath, appendContent);
            });
            
            OutputLabel.Text = $"Content successfully appended to: {filePath}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error appending to file: {ex.Message}");
        }
    }
    
    private async void OnDeleteFileClicked(object sender, EventArgs e)
    {
        try
        {
            if (FileNameEntry == null || OutputLabel == null)
                return;
                
            string fileName = FileNameEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(fileName))
            {
                await DisplayAlert("Error", "Please enter a file name to delete", "OK");
                return;
            }
            
            // Get file path
            string targetDirectory = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(targetDirectory, fileName);
            
            if (!File.Exists(filePath))
            {
                OutputLabel.Text = $"File not found: {filePath}";
                return;
            }
            
            // Confirm deletion
            bool confirm = await DisplayAlert("Confirm Delete", 
                $"Are you sure you want to delete {fileName}?", "Yes", "No");
            
            if (confirm)
            {
                // Delete the file
                IsBusy = true;
                LoadingIndicator.IsRunning = true;
                
                await Task.Run(() => File.Delete(filePath));
                
                OutputLabel.Text = $"File deleted: {filePath}";
                FileContentEditor.Text = string.Empty;
                
                IsBusy = false;
                LoadingIndicator.IsRunning = false;
            }
        }
        catch (Exception ex)
        {
            DisplayError($"Error deleting file: {ex.Message}");
        }
    }
    
    private void OnShowFileSystemInfoClicked(object sender, EventArgs e)
    {
        try
        {
            if (FilesystemInfoLabel == null)
                return;
                
            StringBuilder info = new StringBuilder();
            
            // Display information about the app's file system paths
            info.AppendLine($"App Data Directory: {FileSystem.AppDataDirectory}");
            info.AppendLine($"Cache Directory: {FileSystem.CacheDirectory}");
            
            // Show platform-specific directories if available
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                string externalDir = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                info.AppendLine($"External Storage: {externalDir}");
                info.AppendLine($"Pictures Directory: {Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath}");
                info.AppendLine($"Documents Directory: {Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath}");
                info.AppendLine($"Downloads Directory: {Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath}");
            }
            
            FilesystemInfoLabel.Text = info.ToString();
        }
        catch (Exception ex)
        {
            DisplayError($"Error displaying file system info: {ex.Message}");
        }
    }
    
    private async void OnListFilesClicked(object sender, EventArgs e)
    {
        try
        {
            if (OutputLabel == null)
                return;
                
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            // Get app data directory
            string directory = FileSystem.AppDataDirectory;
            
            StringBuilder filesList = new StringBuilder();
            filesList.AppendLine($"Files in {directory}:");
            
            // List all files
            await Task.Run(() => {
                string[] files = Directory.GetFiles(directory);
                
                if (files.Length == 0)
                {
                    filesList.AppendLine("No files found.");
                }
                else
                {
                    foreach (string file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        filesList.AppendLine($"• {Path.GetFileName(file)} ({fileInfo.Length} bytes)");
                    }
                }
            });
            
            OutputLabel.Text = filesList.ToString();
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error listing files: {ex.Message}");
        }
    }
    
    #endregion
    
    #region App-Specific Storage
    
    private async void OnSaveSettingClicked(object sender, EventArgs e)
    {
        try
        {
            if (SettingKeyEntry == null || SettingValueEntry == null || AppDataOutputLabel == null)
                return;
                
            string key = SettingKeyEntry.Text?.Trim();
            string value = SettingValueEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(key))
            {
                await DisplayAlert("Error", "Please enter a setting key", "OK");
                return;
            }
            
            // Save the setting
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            // Use the Preferences API to store settings
            Preferences.Default.Set(key, value ?? string.Empty);
            
            AppDataOutputLabel.Text = $"Setting saved: {key} = {value}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error saving setting: {ex.Message}");
        }
    }
    
    private void OnGetSettingClicked(object sender, EventArgs e)
    {
        try
        {
            if (SettingKeyEntry == null || SettingValueEntry == null || AppDataOutputLabel == null)
                return;
                
            string key = SettingKeyEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(key))
            {
                DisplayAlert("Error", "Please enter a setting key to retrieve", "OK");
                return;
            }
            
            // Retrieve the setting
            string value = Preferences.Default.Get(key, string.Empty);
            
            SettingValueEntry.Text = value;
            AppDataOutputLabel.Text = string.IsNullOrEmpty(value) 
                ? $"No setting found for key: {key}" 
                : $"Setting retrieved: {key} = {value}";
        }
        catch (Exception ex)
        {
            DisplayError($"Error retrieving setting: {ex.Message}");
        }
    }
    
    private async void OnSaveSecureClicked(object sender, EventArgs e)
    {
        try
        {
            if (SecureKeyEntry == null || SecureValueEntry == null || AppDataOutputLabel == null)
                return;
                
            string key = SecureKeyEntry.Text?.Trim();
            string value = SecureValueEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(key))
            {
                await DisplayAlert("Error", "Please enter a secure storage key", "OK");
                return;
            }
            
            // Save to secure storage
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            await SecureStorage.Default.SetAsync(key, value ?? string.Empty);
            
            AppDataOutputLabel.Text = $"Secure value saved for key: {key}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error saving to secure storage: {ex.Message}");
        }
    }
    
    private async void OnGetSecureClicked(object sender, EventArgs e)
    {
        try
        {
            if (SecureKeyEntry == null || SecureValueEntry == null || AppDataOutputLabel == null)
                return;
                
            string key = SecureKeyEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(key))
            {
                await DisplayAlert("Error", "Please enter a secure storage key to retrieve", "OK");
                return;
            }
            
            // Retrieve from secure storage
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            string value = await SecureStorage.Default.GetAsync(key);
            
            SecureValueEntry.Text = value;
            AppDataOutputLabel.Text = string.IsNullOrEmpty(value) 
                ? $"No secure value found for key: {key}" 
                : $"Secure value retrieved for key: {key}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error retrieving from secure storage: {ex.Message}");
        }
    }
    
    private async void OnRemoveSecureClicked(object sender, EventArgs e)
    {
        try
        {
            if (SecureKeyEntry == null || AppDataOutputLabel == null)
                return;
                
            string key = SecureKeyEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(key))
            {
                await DisplayAlert("Error", "Please enter a secure storage key to remove", "OK");
                return;
            }
            
            // Remove from secure storage
            SecureStorage.Default.Remove(key);
            
            SecureValueEntry.Text = string.Empty;
            AppDataOutputLabel.Text = $"Secure value removed for key: {key}";
        }
        catch (Exception ex)
        {
            DisplayError($"Error removing from secure storage: {ex.Message}");
        }
    }
    
    private async void OnWriteCacheClicked(object sender, EventArgs e)
    {
        try
        {
            if (CacheFileNameEntry == null || CacheContentEntry == null || AppDataOutputLabel == null)
                return;
                
            string fileName = CacheFileNameEntry.Text?.Trim();
            string content = CacheContentEntry.Text?.Trim();
            
            if (string.IsNullOrEmpty(fileName))
            {
                await DisplayAlert("Error", "Please enter a cache file name", "OK");
                return;
            }
            
            // Write to cache directory
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            string cachePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            await Task.Run(() => File.WriteAllText(cachePath, content ?? string.Empty));
            
            AppDataOutputLabel.Text = $"Cache file written: {cachePath}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error writing to cache: {ex.Message}");
        }
    }
    
    private async void OnClearCacheClicked(object sender, EventArgs e)
    {
        try
        {
            if (AppDataOutputLabel == null)
                return;
                
            // Confirm cache clear
            bool confirm = await DisplayAlert("Confirm Clear Cache", 
                "Are you sure you want to clear all files in the cache directory?", 
                "Yes", "No");
            
            if (!confirm) return;
            
            // Clear all files in cache directory
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            await Task.Run(() => {
                string[] files = Directory.GetFiles(FileSystem.CacheDirectory);
                int count = 0;
                
                foreach (string file in files)
                {
                    File.Delete(file);
                    count++;
                }
                
                MainThread.BeginInvokeOnMainThread(() => {
                    AppDataOutputLabel.Text = $"Cleared {count} file(s) from cache";
                    CacheFileNameEntry.Text = string.Empty;
                    CacheContentEntry.Text = string.Empty;
                });
            });
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error clearing cache: {ex.Message}");
        }
    }
    
    #endregion
    
    #region Media Storage
    
    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            if (MediaImage == null || ImagePathLabel == null || MediaOutputLabel == null)
                return;
                
            // Check for camera permission
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", 
                        "Camera permission is required to take photos.", 
                        "OK");
                    return;
                }
            }
            
            // Take a photo
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            
            if (photo != null)
            {
                // Save the captured photo to app's data directory
                string localFilePath = await SavePhotoToAppDataDirectory(photo);
                
                // Display the photo
                _currentImagePath = localFilePath;
                MediaImage.Source = ImageSource.FromFile(localFilePath);
                ImagePathLabel.Text = localFilePath;
                MediaOutputLabel.Text = $"Photo saved to: {localFilePath}";
            }
            else
            {
                MediaOutputLabel.Text = "No photo was captured.";
            }
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error taking photo: {ex.Message}");
        }
    }
    
    private async Task<string> SavePhotoToAppDataDirectory(FileResult photo)
    {
        // Create a unique filename
        string targetFilename = $"{Path.GetFileNameWithoutExtension(photo.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(photo.FileName)}";
        string targetPath = Path.Combine(FileSystem.AppDataDirectory, targetFilename);
        
        // Copy the file
        using (var stream = await photo.OpenReadAsync())
        using (var newStream = File.OpenWrite(targetPath))
        {
            await stream.CopyToAsync(newStream);
        }
        
        return targetPath;
    }
    
    private async void OnSaveToGalleryClicked(object sender, EventArgs e)
    {
        try
        {
            if (MediaOutputLabel == null)
                return;
                
            if (string.IsNullOrEmpty(_currentImagePath) || !File.Exists(_currentImagePath))
            {
                await DisplayAlert("No Image", "Please take a photo first.", "OK");
                return;
            }
            
            // Check for storage permission on Android
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permission Denied", 
                            "Storage permission is required to save to gallery.", 
                            "OK");
                        return;
                    }
                }
            }
            
            // Copy the file to the device's gallery
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            string filename = Path.GetFileName(_currentImagePath);
            string destinationPath = string.Empty;
            
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android-specific implementation
                var picturePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                destinationPath = Path.Combine(picturePath, $"MauiExamples_{filename}");
                
                // Copy the file
                File.Copy(_currentImagePath, destinationPath, true);
                
                // Notify the media scanner to index the new file
                var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(destinationPath)));
                Android.App.Application.Context.SendBroadcast(mediaScanIntent);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // iOS-specific implementation using UIKit
                using (var sourceData = NSData.FromFile(_currentImagePath))
                {
                    var image = UIKit.UIImage.LoadFromData(sourceData);
                    image.SaveToPhotosAlbum((img, error) =>
                    {
                        if (error != null)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert("Error", $"Could not save to photos: {error.LocalizedDescription}", "OK");
                            });
                        }
                    });
                }
                
                destinationPath = "Photos Album";
            }
            
            // Update the UI
            MediaOutputLabel.Text = $"Image saved to gallery: {destinationPath}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error saving to gallery: {ex.Message}");
        }
    }
    
    private async void OnCreateFileInDownloadsClicked(object sender, EventArgs e)
    {
        try
        {
            if (MediaOutputLabel == null)
                return;
                
            // Check for storage permissions on Android
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permission Denied", 
                            "Storage permission is required to write to Downloads.", 
                            "OK");
                        return;
                    }
                }
            }
            
            // Create a text file in the Downloads directory
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            string filename = $"maui_example_{DateTime.Now:yyyyMMddHHmmss}.txt";
            string content = $"This file was created by the MAUI Examples app on {DateTime.Now}";
            string destinationPath = string.Empty;
            
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android-specific implementation
                var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                destinationPath = Path.Combine(downloadsPath, filename);
                
                // Write the file
                File.WriteAllText(destinationPath, content);
                
                // Notify the media scanner to index the new file
                var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(destinationPath)));
                Android.App.Application.Context.SendBroadcast(mediaScanIntent);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // For iOS, we'll use the Documents directory since direct access to Downloads is restricted
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                destinationPath = Path.Combine(documents, filename);
                
                // Write the file
                File.WriteAllText(destinationPath, content);
            }
            
            // Update the UI
            MediaOutputLabel.Text = $"Text file created: {destinationPath}";
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error creating file in downloads: {ex.Message}");
        }
    }
    
    #endregion
    
    #region File Pickers
    
    private async void OnPickFileClicked(object sender, EventArgs e)
    {
        try
        {
            if (PickerOutputLabel == null)
                return;
                
            // Use the file picker to select a single file
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            var options = new PickOptions
            {
                PickerTitle = "Select a text file",
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.text", "public.plain-text" } },
                        { DevicePlatform.Android, new[] { "text/plain" } },
                        { DevicePlatform.WinUI, new[] { ".txt" } },
                        { DevicePlatform.MacCatalyst, new[] { "public.text", "public.plain-text" } }
                    })
            };
            
            var result = await FilePicker.Default.PickAsync(options);
            
            if (result != null)
            {
                // Read and display the file content
                string fileContent = await ReadFileContent(result);
                
                StringBuilder output = new StringBuilder();
                output.AppendLine($"Selected file: {result.FileName}");
                output.AppendLine($"Full path: {result.FullPath}");
                output.AppendLine($"Content type: {result.ContentType}");
                output.AppendLine("\nFile Content:");
                output.AppendLine(fileContent);
                
                PickerOutputLabel.Text = output.ToString();
            }
            else
            {
                PickerOutputLabel.Text = "No file selected.";
            }
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error picking file: {ex.Message}");
        }
    }
    
    private async void OnPickMultipleFilesClicked(object sender, EventArgs e)
    {
        try
        {
            if (PickerOutputLabel == null)
                return;
                
            // Use the file picker to select multiple files
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            var options = new PickOptions
            {
                PickerTitle = "Select multiple files",
                FileTypes = FilePickerFileType.Images // Allow image files
            };
            
            var results = await FilePicker.Default.PickMultipleAsync(options);
            
            if (results != null && results.Count > 0)
            {
                // Display information about selected files
                StringBuilder output = new StringBuilder();
                output.AppendLine($"Selected {results.Count} file(s):");
                
                foreach (var file in results)
                {
                    output.AppendLine($"\n• {file.FileName}");
                    output.AppendLine($"  Path: {file.FullPath}");
                    output.AppendLine($"  Type: {file.ContentType}");
                }
                
                PickerOutputLabel.Text = output.ToString();
            }
            else
            {
                PickerOutputLabel.Text = "No files selected.";
            }
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error picking multiple files: {ex.Message}");
        }
    }
    
    private async void OnPickFolderClicked(object sender, EventArgs e)
    {
        try
        {
            if (PickerOutputLabel == null)
                return;
                
            // Check if folder picking is supported on this platform
            if (!FolderPicker.Default.IsFolderPickerSupported)
            {
                await DisplayAlert("Not Supported", 
                    "Folder picking is not supported on this device.", 
                    "OK");
                return;
            }
            
            // Use the folder picker
            IsBusy = true;
            LoadingIndicator.IsRunning = true;
            
            var result = await FolderPicker.Default.PickAsync(default);
            
            if (!string.IsNullOrEmpty(result.Folder.Path))
            {
                // List files in the selected folder
                var files = Directory.GetFiles(result.Folder.Path);
                
                StringBuilder output = new StringBuilder();
                output.AppendLine($"Selected folder: {result.Folder.Path}");
                output.AppendLine($"Contains {files.Length} file(s):");
                
                foreach (var file in files.Take(20)) // Only show up to 20 files
                {
                    output.AppendLine($"\n• {Path.GetFileName(file)}");
                    var fileInfo = new FileInfo(file);
                    output.AppendLine($"  Size: {FormatFileSize(fileInfo.Length)}");
                }
                
                if (files.Length > 20)
                {
                    output.AppendLine("\n...and more files not shown");
                }
                
                PickerOutputLabel.Text = output.ToString();
            }
            else
            {
                PickerOutputLabel.Text = "No folder selected.";
            }
            
            IsBusy = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Error picking folder: {ex.Message}");
        }
    }
    
    private async Task<string> ReadFileContent(FileResult file)
    {
        try
        {
            using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            
            // Read up to 10KB to avoid very large files
            char[] buffer = new char[10240];
            int bytesRead = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
            
            string content = new string(buffer, 0, bytesRead);
            
            // If we didn't read the full file
            if (!reader.EndOfStream)
            {
                content += "\n\n... (file content truncated) ...";
            }
            
            return content;
        }
        catch (Exception ex)
        {
            return $"Error reading file: {ex.Message}";
        }
    }
    
    #endregion
    
    #region Utilities
    
    private void DisplayError(string message)
    {
        Debug.WriteLine(message);
        
        try
        {
            IsBusy = false;
            if (LoadingIndicator != null) LoadingIndicator.IsRunning = false;
            
            MainThread.BeginInvokeOnMainThread(async () => {
                await DisplayAlert("Error", message, "OK");
            });
        }
        catch
        {
            // Fallback if DisplayAlert fails
        }
    }
    
    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        
        return $"{len:0.##} {sizes[order]}";
    }
    
    #endregion
} 