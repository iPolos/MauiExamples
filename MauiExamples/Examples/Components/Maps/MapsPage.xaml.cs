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
    
    // Colors for tab selection
    private readonly Color _selectedTabColor;
    private readonly Color _unselectedTabColor = Colors.LightGray;
    
    public MapsPage()
    {
        InitializeComponent();
        
        // Safely get the primary color from resources, or use a default color
        if (Application.Current?.Resources != null && 
            Application.Current.Resources.TryGetValue("Primary", out var primaryColor) && 
            primaryColor is Color color)
        {
            _selectedTabColor = color;
        }
        else
        {
            _selectedTabColor = Colors.Blue; // Fallback color
        }
        
        try
        {
            // Initialize maps with default view (Seattle)
            InitializeMap(BasicMap, _seattleLocation);
            InitializeMap(LocationMap, _seattleLocation);
            InitializeMap(PinsMap, _seattleLocation);
            InitializeMap(GeocodingMap, _seattleLocation);
            
            // Add demo pins to the pins map
            AddDemoPins();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in MapsPage constructor: {ex.Message}");
            // Don't throw to avoid crashing the app
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        try
        {
            // Show coordinates input for reverse geocoding after first load
            if (CoordinatesGrid != null)
            {
                CoordinatesGrid.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnAppearing: {ex.Message}");
        }
    }

    #region Map Initialization

    private void InitializeMap(Map map, Location centerLocation)
    {
        if (map == null || centerLocation == null) return;
        
        try
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(
                centerLocation,
                Distance.FromKilometers(_defaultMapSpanInKm)));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing map: {ex.Message}");
        }
    }

    private void AddDemoPins()
    {
        try
        {
            if (PinsMap == null) return;
            
            // Add some demo pins
            AddPin(PinsMap, "Seattle", "The Emerald City", _seattleLocation);
            AddPin(PinsMap, "New York", "The Big Apple", _newYorkLocation);
            AddPin(PinsMap, "London", "The Big Smoke", _londonLocation);
            AddPin(PinsMap, "Tokyo", "The Eastern Capital", _tokyoLocation);
            AddPin(PinsMap, "Sydney", "The Harbour City", _sydneyLocation);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding demo pins: {ex.Message}");
        }
    }

    private void AddPin(Map map, string label, string address, Location location)
    {
        if (map == null || location == null) return;
        
        try
        {
            var pin = new Pin
            {
                Label = label ?? "Unknown",
                Address = address ?? string.Empty,
                Location = location,
                Type = PinType.Place
            };
            
            // Add pin selected handler
            pin.MarkerClicked += OnPinMarkerClicked;
            pin.InfoWindowClicked += OnPinInfoWindowClicked;
            
            map.Pins.Add(pin);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding pin: {ex.Message}");
        }
    }

    #endregion

    #region Tab Navigation

    private void ResetTabButtons()
    {
        try 
        {
            if (BasicMapButton != null) BasicMapButton.BackgroundColor = _unselectedTabColor;
            if (UserLocationButton != null) UserLocationButton.BackgroundColor = _unselectedTabColor;
            if (PinsButton != null) PinsButton.BackgroundColor = _unselectedTabColor;
            if (GeocodingButton != null) GeocodingButton.BackgroundColor = _unselectedTabColor;
            
            if (BasicMapView != null) BasicMapView.IsVisible = false;
            if (UserLocationView != null) UserLocationView.IsVisible = false;
            if (PinsView != null) PinsView.IsVisible = false;
            if (GeocodingView != null) GeocodingView.IsVisible = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error resetting tab buttons: {ex.Message}");
        }
    }

    private void OnBasicMapClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (BasicMapButton != null) BasicMapButton.BackgroundColor = _selectedTabColor;
            if (BasicMapView != null) BasicMapView.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnBasicMapClicked: {ex.Message}");
        }
    }

    private void OnUserLocationClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (UserLocationButton != null) UserLocationButton.BackgroundColor = _selectedTabColor;
            if (UserLocationView != null) UserLocationView.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnUserLocationClicked: {ex.Message}");
        }
    }

    private void OnPinsClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (PinsButton != null) PinsButton.BackgroundColor = _selectedTabColor;
            if (PinsView != null) PinsView.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnPinsClicked: {ex.Message}");
        }
    }

    private void OnGeocodingClicked(object sender, EventArgs e)
    {
        try
        {
            ResetTabButtons();
            if (GeocodingButton != null) GeocodingButton.BackgroundColor = _selectedTabColor;
            if (GeocodingView != null) GeocodingView.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnGeocodingClicked: {ex.Message}");
        }
    }

    #endregion

    #region Basic Map Functions

    private void OnMapTypeChanged(object sender, EventArgs e)
    {
        try
        {
            if (MapTypePicker == null || BasicMap == null || MapTypePicker.SelectedIndex == -1)
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
        catch (Exception ex)
        {
            Debug.WriteLine($"Error changing map type: {ex.Message}");
        }
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
                if (LatitudeLabel != null) LatitudeLabel.Text = $"Latitude: {location.Latitude}";
                if (LongitudeLabel != null) LongitudeLabel.Text = $"Longitude: {location.Longitude}";
                if (AccuracyLabel != null) AccuracyLabel.Text = $"Accuracy: {location.Accuracy} meters";
                
                // Move the map to the current location
                if (LocationMap != null)
                {
                    var mapLocation = new Location(location.Latitude, location.Longitude);
                    LocationMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        mapLocation,
                        Distance.FromKilometers(1)));
                }
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
            if (PinNameEntry == null || PinAddressEntry == null || PinsMap == null)
                return;
                
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
        try
        {
            if (e != null)
            {
                // Returning true will keep the popup open
                e.HideInfoWindow = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in pin marker clicked: {ex.Message}");
        }
    }

    private async void OnPinInfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        try
        {
            if (sender is Pin pin && pin != null)
            {
                // Show details when info window is clicked
                await DisplayAlert(
                    pin.Label ?? "Pin", 
                    $"Address: {pin.Address ?? "Unknown"}\nLocation: {pin.Location?.Latitude.ToString() ?? "?"}, {pin.Location?.Longitude.ToString() ?? "?"}", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in pin info window clicked: {ex.Message}");
        }
    }

    #endregion

    #region Geocoding Functions

    private async void OnFindAddressClicked(object sender, EventArgs e)
    {
        try
        {
            if (AddressEntry == null || GeocodingResultLabel == null || GeocodingMap == null || 
                LatitudeEntry == null || LongitudeEntry == null)
                return;
                
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
            if (GeocodingResultLabel != null)
            {
                GeocodingResultLabel.Text = $"Error: {ex.Message}";
            }
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
            if (LatitudeEntry == null || LongitudeEntry == null || GeocodingResultLabel == null || 
                AddressEntry == null || GeocodingMap == null)
                return;
                
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
            if (GeocodingResultLabel != null)
            {
                GeocodingResultLabel.Text = $"Error: {ex.Message}";
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string FormatAddress(Placemark placemark)
    {
        if (placemark == null) return "Unknown location";
        
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