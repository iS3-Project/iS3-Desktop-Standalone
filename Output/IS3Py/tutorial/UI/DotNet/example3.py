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
from System import Random

random = Random()

form = Form()
form.Text = "Hello World"
form.FormBorderStyle = FormBorderStyle.Fixed3D
form.Height = 150

newFont = Font("Verdana", 16, FontStyle.Bold | FontStyle.Italic)

label = Label()
label.AutoSize = True
label.Text = "My Hello World Label"
label.Location = Point(10, 50)
label.BackColor = Color.Aquamarine
label.ForeColor = Color.DarkMagenta
label.Font = newFont

form.Controls.Add(label)

def GetNewColor():
    red = random.Next(256)
    green = random.Next(256)
    blue = random.Next(256)
    return Color.FromArgb(red, green, blue)

def ChangeColor(sender, event):
    print 'X:', event.X, 'Y:', event.Y
    sender.BackColor = GetNewColor()
    sender.ForeColor = GetNewColor()

label.MouseMove += ChangeColor

Application.Run(form)