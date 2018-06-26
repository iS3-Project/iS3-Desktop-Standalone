import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")
from System.Windows import (
    SizeToContent, Thickness, Window
)
from System.Windows.Controls import (
    Button, Label, StackPanel
)
from System.Windows.Media.Effects import DropShadowBitmapEffect

window = Window()
window.Title = 'Welcome to IronPython'
window.SizeToContent = SizeToContent.Height
window.Width = 450

stack = StackPanel()
stack.Margin = Thickness(15)
window.Content = stack

button = Button()
button.Content = 'Push Me'
button.FontSize = 24
button.BitmapEffect = DropShadowBitmapEffect()

def onClick(sender, event):
    message = Label()
    message.FontSize = 36
    message.Content = 'Welcome to IronPython!'
    stack.Children.Add(message)

button.Click += onClick
stack.Children.Add(button)

window.Show()

