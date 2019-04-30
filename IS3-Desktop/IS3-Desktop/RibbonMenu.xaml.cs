using iS3.Core;
using Microsoft.Windows.Controls.Ribbon;
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

namespace iS3.Desktop
{
    /// <summary>
    /// RibbonMenu.xaml 的交互逻辑
    /// </summary>
    public partial class RibbonMenu : UserControl
    {
        public iS3MenuRibbon menuRibbon = new iS3MenuRibbon();
        public Dictionary<RibbonButton, iS3MenuButton> menuDict = new Dictionary<RibbonButton, iS3MenuButton>();
        public RibbonMenu()
        {
            InitializeComponent();
        }

        public void ShowMenu()
        {
            foreach (iS3MenuTab tab in menuRibbon.MenuTabs)
            {
                RibbonTab _tab = new RibbonTab() { Header = tab.Header };
                RibbonHolder.Items.Add(_tab);
                foreach (iS3MenuGroup group in tab.MenuGroups)
                {
                    RibbonGroup _group = new RibbonGroup() { Header = group.Header };
                    _tab.Items.Add(_group);
                    foreach (iS3MenuButton button in group.MenuButtons)
                    {
                        RibbonButton _button = new RibbonButton() { Label = button.Label };
                        if (button.IsLargeImage)
                        {
                            _button.LargeImageSource = new BitmapImage(new Uri(Runtime.rootPath + @"\tools\Icon\" + button.ImageSource));
                        }
                        else
                        {
                            _button.SmallImageSource = new BitmapImage(new Uri(Runtime.rootPath + @"\tools\Icon\" + button.ImageSource));
                        }
                        menuDict[_button] = button;
                        _group.Items.Add(_button);
                        _button.Click += _button_Click;
                    }
                }
            }
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            RibbonButton button = sender as RibbonButton;
            iS3MenuButton model = menuDict[button];
            model.DFunc.func();
        }

        private void IntroductionME(object sender, MouseEventArgs e)
        {
            contentCP.Content = null;
            string introduction = "*iS3 is short for infrastructure Smart Service System." + "\r\n" + "*iS3 provides smart operation system service for \n tunnels, roads, bridges, pipelines, deep excavation \n and other" +
                " infrastructures to enhance infrasturcture  \n informatization to a brand new level.";
            contentCP.Content = introduction;
        }

        private void ContentME(object sender, MouseEventArgs e)
        {
            contentCP.Content = null;
            string contact = "*Address: Rm.1002, Building 8, 63 Chifeng Road, \nShanghai, P.R.China" + "\r\n" + "*Postcode: 200092" + "\r\n" + "*Email: office@iS3.org.cn";
            contentCP.Content = contact;
        }
    }
}
