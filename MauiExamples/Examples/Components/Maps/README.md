# Maps & Location in .NET MAUI

This component demonstrates how to implement mapping functionality and location services in a .NET MAUI application.

## Features

- **Interactive Maps**: Display maps with zoom and pan controls
- **User Location**: Track and display the user's current location
- **Custom Map Pins**: Add and customize pins on the map
- **Geocoding**: Convert addresses to coordinates and vice versa
- **Route Directions**: Draw routes between locations

## Implementation Details

The implementation uses MAUI's Map control and location services, which leverage platform-specific map providers (Google Maps on Android, Apple Maps on iOS).

### Key Components

1. **Map Control**: Display interactive maps with various features
2. **Location Services**: Access user's current location
3. **Geocoding**: Convert between addresses and coordinates
4. **Pins & Annotations**: Mark specific locations on the map

### Usage

```csharp
// Create a map
var map = new Microsoft.Maui.Controls.Maps.Map
{
    IsShowingUser = true,  // Show user's location
    MapType = MapType.Street
};

// Add a pin
var pin = new Pin
{
    Label = "Microsoft",
    Address = "One Microsoft Way, Redmond, WA 98052",
    Location = new Location(47.643, -122.131)
};
map.Pins.Add(pin);

// Move to specific location
map.MoveToRegion(MapSpan.FromCenterAndRadius(
    new Location(47.643, -122.131),
    Distance.FromKilometers(1)));
```

## Platform Considerations

### Android
- Requires Google Maps API key in AndroidManifest.xml
- Needs location permissions: ACCESS_FINE_LOCATION and ACCESS_COARSE_LOCATION

```xml
<application ...>
    <meta-data android:name="com.google.android.geo.API_KEY" 
               android:value="YOUR_API_KEY_HERE" />
</application>
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
```

### iOS
- Uses Apple Maps automatically
- Requires location permission descriptions in Info.plist

```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>This app needs access to your location to show your position on the map.</string>
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>This app needs access to your location to show your position on the map and provide location-based services.</string>
```

## Getting a Google Maps API Key

To use Google Maps on Android:

1. Visit the [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the "Maps SDK for Android" API
4. Create credentials (API key)
5. Restrict the API key to your app's package name and the Maps SDK
6. Add the API key to your AndroidManifest.xml

## Additional Features to Consider

- **Custom Map Styles**: Customize the appearance of maps
- **Clustering**: Group pins together when zoomed out
- **Geofencing**: Trigger events when entering or leaving specified areas
- **Offline Maps**: Download map data for offline use
- **Indoor Mapping**: Navigate within buildings
- **3D Maps**: Utilize 3D terrain and building features

## Additional Resources

- [MAUI Maps Documentation](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/map)
- [Geocoding Documentation](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/geocoding)
- [Geolocation Documentation](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/geolocation)
- [Google Maps API Documentation](https://developers.google.com/maps/documentation/android-sdk)
- [Apple MapKit Documentation](https://developer.apple.com/documentation/mapkit) 