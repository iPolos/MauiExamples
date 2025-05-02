# .NET MAUI MVVM Examples

This project demonstrates three different ways to implement the MVVM (Model-View-ViewModel) pattern in .NET MAUI applications. Each implementation shows a simple product catalog app with a list and details view.

## Implementations

1. **Standard XAML with Code-Behind**
   - Traditional approach without MVVM
   - Uses direct code-behind for event handling
   - Located in `Examples/Standard/`

2. **Vanilla MVVM**
   - Custom implementation of MVVM without external libraries
   - Includes a custom `BaseViewModel` class with `INotifyPropertyChanged` implementation
   - Includes a custom `EventToCommandBehavior` to handle events
   - Located in `Examples/VanillaMvvm/`

3. **MVVM Toolkit**
   - Uses the Microsoft Community Toolkit.MVVM package
   - Takes advantage of source generators and attributes
   - Simplifies MVVM implementation with `ObservableObject`, `ObservableProperty`, and `RelayCommand`
   - Located in `Examples/MvvmToolkit/`

## Project Structure

- `Models/` - Contains the shared Product model
- `Services/` - Contains the ProductService for data access
- `Examples/Standard/` - Standard implementation
- `Examples/VanillaMvvm/` - Custom MVVM implementation
- `Examples/MvvmToolkit/` - MVVM Toolkit implementation

## Key Features Demonstrated

- Data binding
- Command pattern
- Property change notification
- Navigation between pages
- Collection views
- Dependency injection

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio 2022 or JetBrains Rider
3. Run the application
4. Use the main menu to navigate between the different implementations

## Requirements

- .NET 9.0 or later
- MAUI SDK