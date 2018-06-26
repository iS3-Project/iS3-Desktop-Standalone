using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace IS3.Desktop.UserControls
{
    public enum SelectionButtonEnum
    {
        Point = 0,
        Polyline = 1,
        Polygon = 2,
        Rectangle = 3,
        Stop = 4,
        Clear = 5,
    }

    public class SelectionToolbarItemArgs : EventArgs
    {
        public SelectionButtonEnum Index { get; set; }
    }

    /// <summary>
    /// Interaction logic for SelectionToolbar.xaml
    /// </summary>
    public partial class SelectionToolbar : UserControl
    {
        public SelectionToolbar()
        {
            InitializeComponent();
        }

        public string Title 
        {
            get { return TB_Title.Text; } 
            set { TB_Title.Text = value; } 
        }
        public string Status 
        { 
            get { return TB_Status.Text; } 
            set { TB_Status.Text = value; }
        }

        public event EventHandler<SelectionToolbarItemArgs> ToolbarClicked;
        public event EventHandler<EventArgs> OKClicked;

        public Button GetButton(SelectionButtonEnum index)
        {
            if (index == SelectionButtonEnum.Point)
                return Btn_Point;
            else if (index == SelectionButtonEnum.Polyline)
                return Btn_Polyline;
            else if (index == SelectionButtonEnum.Polygon)
                return Btn_Polygon;
            else if (index == SelectionButtonEnum.Rectangle)
                return Btn_Rectangle;
            else if (index == SelectionButtonEnum.Stop)
                return Btn_Stop;
            else if (index == SelectionButtonEnum.Clear)
                return Btn_Clear;
            else
                return null;
        }

        public SelectionButtonEnum GetButtonEnum(Button btn)
        {
            if (btn.Name == "Btn_Point")
                return SelectionButtonEnum.Point;
            else if (btn.Name == "Btn_Polyline")
                return SelectionButtonEnum.Polyline;
            else if (btn.Name == "Btn_Polygon")
                return SelectionButtonEnum.Polygon;
            else if (btn.Name == "Btn_Rectangle")
                return SelectionButtonEnum.Rectangle;
            else if (btn.Name == "Btn_Stop")
                return SelectionButtonEnum.Stop;
            else
                return SelectionButtonEnum.Clear;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn;
            foreach (UIElement element in ButtonsHolder.Children)
            {
                btn = element as Button;
                if (btn != null)
                {
                    VisualStateManager.GoToState(btn, "UnSelected", false);
                }
            }

            btn = sender as Button;
            VisualStateManager.GoToState(btn, "Selected", false);

            SelectionButtonEnum index = GetButtonEnum(btn);
            string tooltip = ToolTipService.GetToolTip(btn) as string;
            Status = tooltip;

            if (ToolbarClicked != null)
            {
                SelectionToolbarItemArgs args = new SelectionToolbarItemArgs();
                args.Index = index;
                ToolbarClicked(this, args);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClicked != null)
                OKClicked(this, e);
        }

        public void ShowAllButton(System.Windows.Visibility vis)
        {
            for (int i = 0; i <= 5; i++)
            {
                SelectionButtonEnum index = (SelectionButtonEnum)i;
                Button btn = GetButton(index);
                btn.Visibility = vis;
            }
        }

        public void ShowButton(SelectionButtonEnum index, System.Windows.Visibility vis)
        {
            Button btn = GetButton(index);
            btn.Visibility = vis;
        }

        public void SetLayers(IEnumerable layers)
        {
            LayerList.ItemsSource = layers;
        }

        public void SetCurrentLayerIndex(int index)
        {
            LayerList.SelectedIndex = index;
        }

        public object GetCurrentLayer()
        {
            return LayerList.SelectedItem;
        }
    }
}
