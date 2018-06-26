import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, Form

form = Form()
form.Text = "Hello World"

Application.Run(form)