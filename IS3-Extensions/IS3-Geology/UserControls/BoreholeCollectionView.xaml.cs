using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using IS3.Core;

namespace IS3.Geology.UserControls
{
    public partial class BoreholeCollectionView : UserControl
    {
        public BoreholeCollectionView()
        {
            InitializeComponent();
            ScaleX = 1.0;
            ScaleY = 1.0;
        }

        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double ViewerHeight  { get; set;}
        public double Top { get; set; }
        public double Base { get; set; }
        public double TotalLength { get { return Top - Base; } }
        public DGObjectsCollection Strata { get; set; }
        public List<Borehole> Boreholes  { get; set;}

        double x_space = 70;
        double y_margin = 20;
        double bh_width = 10;

        public void RefreshView()
        {
            if (Boreholes == null || Boreholes.Count == 0)
                return;

            SortBoreholesByMileage();
            SearchBoreholesTopAndBase();
            ScaleY = (ViewerHeight - 80) / TotalLength;

            LayoutRoot.Children.Clear();
            LayoutRoot.Width = 0.0;
            LayoutRoot.Height = ViewerHeight;

            int i = 0;
            Brush blackBrush = new SolidColorBrush(Colors.Black);
            Polyline pline = new Polyline();
            pline.Stroke = blackBrush;

            foreach (Borehole bh in Boreholes)
            {
                BoreholeView bhView = new BoreholeView();
                bhView.BH_Width = bh_width;
                bhView.ScaleY = ScaleY;
                bhView.Strata = Strata;
                bhView.Borehole = bh;
                bhView.RefreshView();
                if (bhView.IsEmpty)
                    continue;

                double bhTop = bh.Top;
                double y = (Top - bhTop) * ScaleY;
                TranslateTransform translate = new TranslateTransform();
                translate.X = i * x_space * ScaleX;
                translate.Y = y + y_margin;
                bhView.RenderTransform = translate;

                LayoutRoot.Children.Add(bhView);
                LayoutRoot.Width += x_space * ScaleX;

                if (bh.Mileage != null)
                {
                    TextBlock tbMileage = new TextBlock();
                    tbMileage.Foreground = blackBrush;
                    tbMileage.Text = bh.Mileage.Value.ToString("0.0");
                    Canvas.SetTop(tbMileage, ViewerHeight - 40);
                    Canvas.SetLeft(tbMileage, translate.X);
                    LayoutRoot.Children.Add(tbMileage);

                    Line tickMileageLine = new Line();
                    tickMileageLine.Stroke = blackBrush;
                    tickMileageLine.StrokeThickness = 1;
                    tickMileageLine.X1 = translate.X + bh_width/2;
                    tickMileageLine.Y1 = ViewerHeight - 50;
                    tickMileageLine.X2 = tickMileageLine.X1;
                    tickMileageLine.Y2 = tickMileageLine.Y1 + 10;
                    LayoutRoot.Children.Add(tickMileageLine);

                    pline.Points.Add(new Point(tickMileageLine.X2, tickMileageLine.Y2));
                }
                i++;
            }

            if (pline.Points.Count >= 2)
                LayoutRoot.Children.Add(pline);
        }

        void SearchBoreholesTopAndBase()
        {
            Top = Boreholes[0].Top;
            Base = Boreholes[0].Base;

            foreach (Borehole bh in Boreholes)
            {
                if (bh.Top > Top)
                    Top = bh.Top;
                if (bh.Base < Base)
                    Base = bh.Base;
            }
        }

        void SortBoreholesByMileage()
        {
            //Boreholes.Sort((x, y) => x.Mileage.Value.CompareTo(y.Mileage.Value));
            try
            {
                Boreholes.Sort(
                    (x, y) =>
                    {
                        if (x == null || y == null)
                            return 1;
                        if (x.Mileage == null || Double.IsNaN(x.Mileage.Value))
                            return 1;
                        if (y.Mileage == null || Double.IsNaN(y.Mileage.Value))
                            return 1;
                        return x.Mileage.Value.CompareTo(y.Mileage.Value);
                    }
                    );
            }
            catch (Exception e)
            {
                string str = e.Message;
            }
        }
    }
}
