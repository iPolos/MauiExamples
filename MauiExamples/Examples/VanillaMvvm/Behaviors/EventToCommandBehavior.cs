using System.Reflection;
using System.Windows.Input;

namespace MauiExamples.Examples.VanillaMvvm.Behaviors;

public class EventToCommandBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty EventNameProperty =
        BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior), null);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior), null);

    public string EventName
    {
        get => (string)GetValue(EventNameProperty);
        set => SetValue(EventNameProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    private EventInfo _eventInfo;
    private Delegate _eventHandler;

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);

        if (string.IsNullOrEmpty(EventName))
            return;

        _eventInfo = bindable.GetType().GetRuntimeEvent(EventName) 
                    ?? throw new ArgumentException($"Event '{EventName}' not found on {bindable.GetType().Name}");

        var methodInfo = GetType().GetTypeInfo().GetDeclaredMethod(nameof(OnEvent)) 
                        ?? throw new ArgumentException("OnEvent method not found");

        _eventHandler = methodInfo.CreateDelegate(_eventInfo.EventHandlerType, this);
        _eventInfo.AddEventHandler(bindable, _eventHandler);
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        if (_eventInfo != null && _eventHandler != null)
            _eventInfo.RemoveEventHandler(bindable, _eventHandler);

        base.OnDetachingFrom(bindable);
    }

    private void OnEvent(object sender, object eventArgs)
    {
        if (Command == null)
            return;

        var parameter = CommandParameter ?? eventArgs;
        if (Command.CanExecute(parameter))
            Command.Execute(parameter);
    }
} 