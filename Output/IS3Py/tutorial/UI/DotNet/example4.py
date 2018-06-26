import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, Form,
    FormBorderStyle, Label
)
from System.Drawing import (
    Color, Font, FontStyle, Point
)

class MainForm(Form):
    def __init__(self):
        self.Text = "Hello World"
        self.FormBorderStyle = FormBorderStyle.Fixed3D
        self.Height = 150
        
        newFont = Font("Verdana", 16, FontStyle.Bold | FontStyle.Italic)

        label = Label()
        label.AutoSize = True
        label.Text = "My Hello World Label"
        label.Location = Point(10, 50)
        label.BackColor = Color.Aquamarine
        label.ForeColor = Color.DarkMagenta
        label.Font = newFont

        self.Controls.Add(label)

mainForm = MainForm()
Application.Run(mainForm)