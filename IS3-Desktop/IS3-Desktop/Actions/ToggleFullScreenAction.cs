using System.Windows;
using System.Windows.Interactivity;

namespace DigitalGeotec.Actions
{
    public class ToggleFullScreenAction : TriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }
    }
}
