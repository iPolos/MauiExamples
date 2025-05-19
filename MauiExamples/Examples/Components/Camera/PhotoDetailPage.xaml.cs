namespace MauiExamples.Examples.Components.Camera;

public partial class PhotoDetailPage : ContentPage
{
    private string _photoPath;
    private Action _deleteCallback;
    
    public PhotoDetailPage(string photoPath, DateTime dateTaken, Action deleteCallback)
    {
        InitializeComponent();
        
        _photoPath = photoPath;
        _deleteCallback = deleteCallback;
        
        // Display the photo
        PhotoImage.Source = ImageSource.FromFile(photoPath);
        
        // Set the date taken
        DateTakenLabel.Text = $"Taken: {dateTaken:g}";
    }
    
    private async void OnDeletePhotoClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete Photo", 
            "Are you sure you want to delete this photo?", 
            "Yes", "No");
        
        if (confirm)
        {
            try
            {
                if (File.Exists(_photoPath))
                {
                    File.Delete(_photoPath);
                }
                
                // Call the delete callback to update the collection
                _deleteCallback?.Invoke();
                
                // Go back to the camera page
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not delete photo: {ex.Message}", "OK");
            }
        }
    }
} 