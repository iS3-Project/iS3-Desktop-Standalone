using System;
using System.Windows;
using System.Windows.Controls;

using IS3.Core;

namespace IS3.Desktop.UserControls
{
    public class DrawToolClickEventArgs : EventArgs
    {
        public DrawShapeType drawShapeType { get; set; }
        public bool stopDraw { get; set; }
    }

    /// <summary>
    /// Interaction logic for DrawToolControl.xaml
    /// </summary>
    public partial class DrawToolControl : UserControl
    {
        public EventHandler<DrawToolClickEventArgs> drawToolClickEventHandler;

        public DrawToolControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (drawToolClickEventHandler != null)
            {
                DrawToolClickEventArgs args = new DrawToolClickEventArgs();
                Button btn = sender as Button;
                if (btn.Name == "Btn_Point")
                    args.drawShapeType = DrawShapeType.Point;
                else if (btn.Name == "Btn_Polyline")
                    args.drawShapeType = DrawShapeType.Polyline;
                else if (btn.Name == "Btn_Polygon")
                    args.drawShapeType = DrawShapeType.Polygon;
                else if (btn.Name == "Btn_Rectangle")
                    args.drawShapeType = DrawShapeType.Rectangle;
                else if (btn.Name == "Btn_Circle")
                    args.drawShapeType = DrawShapeType.Circle;
                else if (btn.Name == "Btn_Ellipse")
                    args.drawShapeType = DrawShapeType.Ellipse;
                else if (btn.Name == "Btn_Freehand")
                    args.drawShapeType = DrawShapeType.Freehand;
                else if (btn.Name == "Btn_Arrow")
                    args.drawShapeType = DrawShapeType.Arrow;
                else if (btn.Name == "Btn_Stop")
                {
                    args.stopDraw = true;
                }
                drawToolClickEventHandler(this, args);
            }
        }
    }
}
