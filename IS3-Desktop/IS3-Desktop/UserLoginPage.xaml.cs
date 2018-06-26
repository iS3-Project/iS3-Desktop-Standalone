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

using IS3.Core;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace IS3.Desktop
{
    /// <summary>
    /// UserLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class UserLoginPage : UserControl
    {
        private XDocument xml;
        public UserLoginPage()
        {
            InitializeComponent();
        }

        #region 按钮事件

        //登陆验证
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            XDocument xml = XDocument.Load(Runtime.configurationPath);

            if ((LoginNameTB.Text == "") || (LoginPasswordTB.Password == ""))
            {
                MessageBox.Show("账号或密码不为空");
                return;
            }
            if ((LoginNameTB.Text != xml.Root.Element("user").Value) || (LoginPasswordTB.Password != xml.Root.Element("password").Value))
            {
                MessageBox.Show("账号或密码错误");
                return;
            }
            App app = Application.Current as App;
            IS3MainWindow mw = (IS3MainWindow)app.MainWindow;
            mw.SwitchToProjectListPage();
        }

        #endregion
    }
}