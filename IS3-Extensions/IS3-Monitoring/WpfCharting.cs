using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

// WPFToolkit need to be installed at first.
// Or System.Windows.Controls.DataVisualization.Charting.Toolkit.dll must exist.
//
//using System.Windows.Controls.DataVisualization.Charting;

using IS3.Core;

namespace IS3.Monitoring
{
    // Summary:
    //     Charting using System.Windows.Controls.DataVisualization.Charting
    // Remarks:
    //     There are two versions of charting from Microsoft:
    //     System.Windows.Forms.DataVisualization.Charting in .Net 4.0+
    //     System.Windows.Controls.DataVisualization.Charting in WPFToolkit
    //
    /*
    public class WpfCharting
    {
        public static FrameworkElement getChart(IEnumerable<DGObject> objs,
            double width, double height)
        {
            Chart chart = new Chart();
            chart.Name = "Curves";
            chart.Width = width;
            chart.Height = height;

            foreach (DGObject obj in objs)
            {
                MonPoint monPnt = obj as MonPoint;
                if (monPnt == null)
                    continue;
                List<ISeries> series = monPnt2Series(monPnt);
                foreach (var item in series)
                    chart.Series.Add(item);
            }
            return chart;
        }

        static List<ISeries> monPnt2Series(MonPoint monPnt)
        {
            List<ISeries> series = new List<ISeries>();
            foreach (string key in monPnt.readingsDict.Keys)
            {
                LineSeries lineSeries = new LineSeries();
                lineSeries.Title = monPnt.name + ":" + key;
                List<MonReading> readings = monPnt.readingsDict[key];
                lineSeries.ItemsSource = readings;
                lineSeries.IndependentValueBinding = new System.Windows.Data.Binding("time");
                lineSeries.DependentValueBinding = new System.Windows.Data.Binding("value");
                lineSeries.DataPointStyle = getDataPointStyle();

                series.Add(lineSeries);
            }
            return series;
        }

        static Style getDataPointStyle()
        {
            //<Style TargetType="LineDataPoint">
            //    <Setter Property="Template">
            //        <Setter.Value>
            //            <ControlTemplate TargetType="LineDataPoint">
            //                <Canvas Width="0.0" Height="0.0">
            //                </Canvas>
            //            </ControlTemplate>
            //        </Setter.Value>
            //    </Setter>
            //</Style>
            // The above XAML in c# code:
            ControlTemplate template = new ControlTemplate(typeof(LineDataPoint));
            var rec = new FrameworkElementFactory(typeof(Canvas));
            rec.SetValue(Canvas.WidthProperty, 0.0D);
            rec.SetValue(Canvas.HeightProperty, 0.0D);
            template.VisualTree = rec;

            var style = new Style(typeof(LineDataPoint));
            style.Setters.Add(new Setter(Control.TemplateProperty, template));

            return style;
        }
    }*/
}
