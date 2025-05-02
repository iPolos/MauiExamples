using System.Reflection;
using System.Windows.Input;

namespace MauiExamples.Examples.VanillaMvvm.Behaviors;

/// <summary>
/// A behavior that executes a command when an event is raised on a VisualElement.
/// This enables event-to-command binding in XAML for MVVM implementation.
/// </summary>
public class EventToCommandBehavior : Behavior<VisualElement>
{
    /// <summary>
    /// Bindable property for the name of the event to listen for
    /// Example values: "Appearing", "Loaded", "Clicked", etc.
    /// </summary>
    public static readonly BindableProperty EventNameProperty =
        BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior), null);

    /// <summary>
    /// Bindable property for the command to execute when the event is raised
    /// This is typically bound to a command in the ViewModel
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior), null);

    /// <summary>
    /// Bindable property for an optional parameter to pass to the command
    /// If not specified, the event arguments will be passed instead
    /// </summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior), null);

    /// <summary>
    /// Gets or sets the name of the event to listen for
    /// </summary>
    public string EventName
    {
        get => (string)GetValue(EventNameProperty);
        set => SetValue(EventNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when the event is raised
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the command
    /// </summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    // Store references to event info and handler for later cleanup
    private EventInfo _eventInfo;
    private Delegate _eventHandler;

    /// <summary>
    /// Called when the behavior is attached to a VisualElement
    /// This method uses reflection to find the specified event and hook up a handler
    /// </summary>
    /// <param name="bindable">The VisualElement this behavior is attached to</param>
    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);

        // If no event name is specified, don't do anything
        if (string.IsNullOrEmpty(EventName))
            return;

        // Find the event on the bindable object using reflection
        // This looks for an event with the name specified in EventName
        _eventInfo = bindable.GetType().GetRuntimeEvent(EventName) 
                    ?? throw new ArgumentException($"Event '{EventName}' not found on {bindable.GetType().Name}");

        // Get a reference to our OnEvent method that will be called when the event is raised
        var methodInfo = GetType().GetTypeInfo().GetDeclaredMethod(nameof(OnEvent)) 
                        ?? throw new ArgumentException("OnEvent method not found");

        // Create a delegate of the correct type to handle the event
        // This delegate will call our OnEvent method when the event is raised
        _eventHandler = methodInfo.CreateDelegate(_eventInfo.EventHandlerType, this);
        
        // Add our event handler to the event
        _eventInfo.AddEventHandler(bindable, _eventHandler);
    }

    /// <summary>
    /// Called when the behavior is detached from the VisualElement
    /// This method removes the event handler to prevent memory leaks
    /// </summary>
    /// <param name="bindable">The VisualElement this behavior is detached from</param>
    protected override void OnDetachingFrom(VisualElement bindable)
    {
        // Remove our event handler if it was previously added
        if (_eventInfo != null && _eventHandler != null)
            _eventInfo.RemoveEventHandler(bindable, _eventHandler);

        base.OnDetachingFrom(bindable);
    }

    /// <summary>
    /// This method is called when the event is raised
    /// It executes the command if it's available and can be executed
    /// </summary>
    /// <param name="sender">The object that raised the event</param>
    /// <param name="eventArgs">The event arguments</param>
    private void OnEvent(object sender, object eventArgs)
    {
        // If no command is specified, don't do anything
        if (Command == null)
            return;

        // Use the command parameter if specified, otherwise use the event args
        var parameter = CommandParameter ?? eventArgs;
        
        // Execute the command if it can be executed
        if (Command.CanExecute(parameter))
            Command.Execute(parameter);
    }
} 