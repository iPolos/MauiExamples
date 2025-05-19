using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Diagnostics;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MauiExamples.Examples.Components.Maps;

public partial class MapsPage : ContentPage
{
    // Demo locations
    private readonly Location _seattleLocation = new(47.6062, -122.3321); // Seattle
    private readonly Location _newYorkLocation = new(40.7128, -74.0060);  // New York
    private readonly Location _londonLocation = new(51.5074, -0.1278);    // London
    private readonly Location _tokyoLocation = new(35.6762, 139.6503);    // Tokyo
    private readonly Location _sydneyLocation = new(-33.8688, 151.2093);  // Sydney

    // Default map span (zoom level)
    private readonly double _defaultMapSpanInKm = 5.0;
    
    public MapsPage()
    {
        InitializeComponent();
        
        // Initialize maps with default view (Seattle)
        InitializeMap(BasicMap, _seattleLocation);
        InitializeMap(LocationMap, _seattleLocation);
        InitializeMap(PinsMap, _seattleLocation);
        InitializeMap(GeocodingMap, _seattleLocation);
        
        // Add demo pins to the pins map
        AddDemoPins();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Show coordinates input for reverse geocoding after first load
        CoordinatesGrid.IsVisible = true;
    }

    #region Map Initialization

    private void InitializeMap(Map map, Location centerLocation)
    {
        map.MoveToRegion(MapSpan.FromCenterAndRadius(
            centerLocation,
            Distance.FromKilometers(_defaultMapSpanInKm)));
    }

    private void AddDemoPins()
    {
        // Add some demo pins
        AddPin(PinsMap, "Seattle", "The Emerald City", _seattleLocation);
        AddPin(PinsMap, "New York", "The Big Apple", _newYorkLocation);
        AddPin(PinsMap, "London", "The Big Smoke", _londonLocation);
        AddPin(PinsMap, "Tokyo", "The Eastern Capital", _tokyoLocation);
        AddPin(PinsMap, "Sydney", "The Harbour City", _sydneyLocation);
    }

    private void AddPin(Map map, string label, string address, Location location)
    {
        var pin = new Pin
        {
            Label = label,
            Address = address,
            Location = location,
            Type = PinType.Place
        };
        
        // Add pin selected handler
        pin.MarkerClicked += OnPinMarkerClicked;
        pin.InfoWindowClicked += OnPinInfoWindowClicked;
        
        map.Pins.Add(pin);
    }

    #endregion

    #region Tab Navigation

    private void ResetTabButtons()
    {
        BasicMapButton.BackgroundColor = Colors.LightGray;
        UserLocationButton.BackgroundColor = Colors.LightGray;
        PinsButton.BackgroundColor = Colors.LightGray;
        GeocodingButton.BackgroundColor = Colors.LightGray;
        
        BasicMapView.IsVisible = false;
        UserLocationView.IsVisible = false;
        PinsView.IsVisible = false;
        GeocodingView.IsVisible = false;
    }

    private void OnBasicMapClicked(object sender, EventArgs e)
    {
        ResetTabButtons();
        BasicMapButton.BackgroundColor = (Color)Application.Current.Resources["Primary"];
        BasicMapView.IsVisible = true;
    }

    private void OnUserLocationClicked(object sender, EventArgs e)
    {
        ResetTabButtons();
        UserLocationButton.BackgroundColor = (Color)Application.Current.Resources["Primary"];
        UserLocationView.IsVisible = true;
    }

    private void OnPinsClicked(object sender, EventArgs e)
    {
        ResetTabButtons();
        PinsButton.BackgroundColor = (Color)Application.Current.Resources["Primary"];
        PinsView.IsVisible = true;
    }

    private void OnGeocodingClicked(object sender, EventArgs e)
    {
        ResetTabButtons();
        GeocodingButton.BackgroundColor = (Color)Application.Current.Resources["Primary"];
        GeocodingView.IsVisible = true;
    }

    #endregion

    #region Basic Map Functions

    private void OnMapTypeChanged(object sender, EventArgs e)
    {
        if (MapTypePicker.SelectedIndex == -1)
            return;

        MapType mapType = MapTypePicker.SelectedIndex switch
        {
            0 => MapType.Street,
            1 => MapType.Satellite,
            2 => MapType.Hybrid,
            _ => MapType.Street
        };

        BasicMap.MapType = mapType;
    }

    #endregion

    #region User Location Functions

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        try
        {
            // Check for location permissions
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", 
                        "Location permission is required to get your current location.", 
                        "OK");
                    return;
                }
            }

            // Show loading indicator
            IsBusy = true;
            
            // Get the current location
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            var location = await Geolocation.GetLocationAsync(request);
            
            if (location != null)
            {
                // Update the UI
                LatitudeLabel.Text = $"Latitude: {location.Latitude}";
                LongitudeLabel.Text = $"Longitude: {location.Longitude}";
                AccuracyLabel.Text = $"Accuracy: {location.Accuracy} meters";
                
                // Move the map to the current location
                var mapLocation = new Location(location.Latitude, location.Longitude);
                LocationMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    mapLocation,
                    Distance.FromKilometers(1)));
            }
            else
            {
                await DisplayAlert("Error", "Unable to get location.", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting location: {ex.Message}");
            await DisplayAlert("Error", $"Failed to get location: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Pins Functions

    private async void OnAddPinClicked(object sender, EventArgs e)
    {
        try
        {
            string pinName = PinNameEntry.Text;
            string pinAddress = PinAddressEntry.Text;
            
            if (string.IsNullOrWhiteSpace(pinName) || string.IsNullOrWhiteSpace(pinAddress))
            {
                await DisplayAlert("Invalid Input", "Please enter both pin name and address.", "OK");
                return;
            }
            
            // Geocode the address to get coordinates
            IsBusy = true;
            var locations = await Geocoding.GetLocationsAsync(pinAddress);
            var location = locations?.FirstOrDefault();
            
            if (location != null)
            {
                var mapLocation = new Location(location.Latitude, location.Longitude);
                
                // Add pin to the map
                AddPin(PinsMap, pinName, pinAddress, mapLocation);
                
                // Move to the new pin
                PinsMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    mapLocation,
                    Distance.FromKilometers(1)));
                
                // Clear input fields
                PinNameEntry.Text = string.Empty;
                PinAddressEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Geocoding Failed", 
                    "Could not find the location for the provided address.", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding pin: {ex.Message}");
            await DisplayAlert("Error", $"Failed to add pin: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPinMarkerClicked(object sender, PinClickedEventArgs e)
    {
        // Returning true will keep the popup open
        e.HideInfoWindow = false;
    }

    private async void OnPinInfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        if (sender is Pin pin)
        {
            // Show details when info window is clicked
            await DisplayAlert(pin.Label, $"Address: {pin.Address}\nLocation: {pin.Location.Latitude}, {pin.Location.Longitude}", "OK");
        }
    }

    #endregion

    #region Geocoding Functions

    private async void OnFindAddressClicked(object sender, EventArgs e)
    {
        try
        {
            string address = AddressEntry.Text;
            
            if (string.IsNullOrWhiteSpace(address))
            {
                await DisplayAlert("Invalid Input", "Please enter an address to geocode.", "OK");
                return;
            }
            
            // Geocode the address
            IsBusy = true;
            var locations = await Geocoding.GetLocationsAsync(address);
            var location = locations?.FirstOrDefault();
            
            if (location != null)
            {
                // Display result
                GeocodingResultLabel.Text = $"Found: {location.Latitude}, {location.Longitude}";
                
                // Update the coordinate entries
                LatitudeEntry.Text = location.Latitude.ToString();
                LongitudeEntry.Text = location.Longitude.ToString();
                
                // Add pin to the map
                GeocodingMap.Pins.Clear();
                var mapLocation = new Location(location.Latitude, location.Longitude);
                AddPin(GeocodingMap, "Found Location", address, mapLocation);
                
                // Move map to the location
                GeocodingMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    mapLocation,
                    Distance.FromKilometers(1)));
            }
            else
            {
                GeocodingResultLabel.Text = "No locations found for this address.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error geocoding: {ex.Message}");
            GeocodingResultLabel.Text = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnReverseGeocodeClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate input
            if (!double.TryParse(LatitudeEntry.Text, out double latitude) ||
                !double.TryParse(LongitudeEntry.Text, out double longitude))
            {
                await DisplayAlert("Invalid Input", "Please enter valid latitude and longitude values.", "OK");
                return;
            }
            
            // Perform reverse geocoding
            IsBusy = true;
            var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
            var placemark = placemarks?.FirstOrDefault();
            
            if (placemark != null)
            {
                // Format the address
                string address = FormatAddress(placemark);
                
                // Display result
                GeocodingResultLabel.Text = $"Address: {address}";
                
                // Update the address entry
                AddressEntry.Text = address;
                
                // Add pin to the map
                GeocodingMap.Pins.Clear();
                var location = new Location(latitude, longitude);
                AddPin(GeocodingMap, "Coordinates", address, location);
                
                // Move map to the location
                GeocodingMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    location,
                    Distance.FromKilometers(1)));
            }
            else
            {
                GeocodingResultLabel.Text = "No address found for these coordinates.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reverse geocoding: {ex.Message}");
            GeocodingResultLabel.Text = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string FormatAddress(Placemark placemark)
    {
        return string.Format("{0} {1}, {2}, {3} {4}, {5}",
            placemark.Thoroughfare ?? string.Empty,
            placemark.SubThoroughfare ?? string.Empty,
            placemark.Locality ?? string.Empty,
            placemark.AdminArea ?? string.Empty,
            placemark.PostalCode ?? string.Empty,
            placemark.CountryName ?? string.Empty).Trim().TrimStart(',').Trim();
    }

    #endregion
} 