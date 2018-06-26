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
    public partial class BoreholeView : UserControl
    {
        public BoreholeView()
        {
            InitializeComponent();
            ScaleY = 1.0;
            BH_Width = 20.0;
        }

        public double ScaleY { get; set; }
        public DGObjectsCollection Strata { get; set; }
        public Borehole Borehole { get; set; }
        public bool IsEmpty { get; set; }
        public double BH_Width { get; set; }

        public void RefreshView()
        {
            LayoutRoot.Children.Clear();
            if (Borehole == null || Borehole.Geologies == null
                || Borehole.Geologies.Count == 0)
            {
                IsEmpty = true;
                return;
            }
            IsEmpty = false;

            double width = BH_Width;
            Brush whiteBrush = new SolidColorBrush(Colors.White);
            Brush blueBrush = new SolidColorBrush(Colors.Blue);
            Brush blackBrush = new SolidColorBrush(Colors.Black);
            Brush redBrush = new SolidColorBrush(Colors.Red);

            // Borehole Name
            //
            TextBlock tbName = new TextBlock();
            tbName.Foreground = redBrush;
            tbName.Text = Borehole.name;
            tbName.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(tbName, 0);
            Canvas.SetTop(tbName, -20);
            LayoutRoot.Children.Add(tbName);

            BoreholeGeology bhGeo0 = Borehole.Geologies[0];
            foreach (BoreholeGeology bhGeo in Borehole.Geologies)
            {
                double top = (Borehole.Top - bhGeo.Top) * ScaleY;
                double height = (bhGeo.Top - bhGeo.Base) * ScaleY;
                top = Math.Abs(top);
                height = Math.Abs(height);

                // Stratum rectangle
                //
                Rectangle rec = new Rectangle();
                rec.Fill = whiteBrush;
                rec.Stroke = blueBrush;
                rec.Width = width;
                rec.Height = height;
                Canvas.SetTop(rec, top);
                Canvas.SetLeft(rec, 0);

                // Stratum name
                //
                TextBlock tbStratumName = new TextBlock();
                tbStratumName.Foreground = blueBrush;
                if (Strata != null)
                {
                    Stratum stratum = Strata[bhGeo.StratumID] as Stratum;
                    tbStratumName.Text = stratum.name;
                }
                else
                    tbStratumName.Text = bhGeo.StratumID.ToString();
                Canvas.SetLeft(tbStratumName, width);
                Canvas.SetTop(tbStratumName, top + height / 2 - 8.0);

                // Stratum base elevation
                //
                TextBlock tbBaseElevation = new TextBlock();
                tbBaseElevation.Foreground = blackBrush;
                tbBaseElevation.Text = bhGeo.Base.ToString("0.00");
                Canvas.SetLeft(tbBaseElevation, width);
                Canvas.SetTop(tbBaseElevation, top + height - 8.0);

                LayoutRoot.Children.Add(rec);
                if (height >= 10.0)
                {
                    LayoutRoot.Children.Add(tbStratumName);
                    LayoutRoot.Children.Add(tbBaseElevation);
                }

                // Stratum top elevation
                //
                if (bhGeo == bhGeo0)
                {
                    TextBlock tbTopElevation = new TextBlock();
                    tbTopElevation.Foreground = blackBrush;
                    tbTopElevation.Text = bhGeo.Top.ToString("0.00");
                    Canvas.SetLeft(tbTopElevation, width);
                    Canvas.SetTop(tbTopElevation, top - 8.0);
                    LayoutRoot.Children.Add(tbTopElevation);
                }
            }
        }
    }
}
