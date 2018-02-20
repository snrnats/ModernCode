using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace ModernCode.Uwp.Behavior
{
    /// <summary>
    /// Behavior allows binding of active visual state with backing state property.
    /// </summary>
    public class VisualStateBehavior : Behavior<Control>
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            nameof(State),
            typeof(string),
            typeof(VisualStateBehavior),
            new PropertyMetadata(string.Empty, PropertyChangedCallback));

        public bool IgnoreCallback { get; set; }

        public string State
        {
            get => (string) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public bool UseTransitions { get; set; }

        public string Group { get; set; }

        public string StatePrefix { get; set; }

        protected override void OnAttached()
        {
            var visualGroup = VisualStateUtilities.GetVisualStateGroups(AssociatedObject).First(group => group.Name == Group);
            visualGroup.CurrentStateChanged += OnVisualStateChanged;
            UpdateState(this, State);
        }

        protected override void OnDetaching()
        {
            var visualGroup = VisualStateUtilities.GetVisualStateGroups(AssociatedObject).First(group => group.Name == Group);
            visualGroup.CurrentStateChanged -= OnVisualStateChanged;
        }

        private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            IgnoreCallback = true;
            var stateName = e.NewState.Name;
            if (!string.IsNullOrEmpty(StatePrefix))
            {
                stateName = stateName.Replace(StatePrefix, string.Empty);
            }

            State = stateName;
            IgnoreCallback = false;
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UpdateState((VisualStateBehavior) dependencyObject, (string) dependencyPropertyChangedEventArgs.NewValue);
        }

        private static void UpdateState(VisualStateBehavior behaviour, string value)
        {
            if (behaviour.IgnoreCallback)
            {
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var element = behaviour.AssociatedObject;
            if (element == null)
            {
                // There are not any stateful elements
                return;
            }

            var state = behaviour.StatePrefix + value;
            VisualStateManager.GoToState(element, state, behaviour.UseTransitions);
        }
    }
}
