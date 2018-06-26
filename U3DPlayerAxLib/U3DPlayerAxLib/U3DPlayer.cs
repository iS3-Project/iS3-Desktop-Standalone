using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Threading;

namespace U3DPlayerAxLib
{
    /// <summary>      
    /// 封装U3d WebPlayer控件，屏蔽ocx中的鼠标右键显示菜单功能       
    /// </summary>        

    public class U3DPlayer : AxUnityWebPlayerAXLib.AxUnityWebPlayer
    {
        #region 常量定义，鼠标信息
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_RBUTTONBLCLK = 0x206;
        #endregion
        /// <summary>      
        /// 
        /// 屏蔽鼠标右键消息，解决鼠标右键下，会出现菜单的问题       
        /// 
        /// 
        /// </summary>        
        /// 
        /// <param name="msg"></param>       
        /// 
        /// <returns></returns>        
        //public override bool PreProcessMessage(ref Message msg)
        //{
        //    switch (msg.Msg)
        //    {
        //        case 0x204://鼠标右键按下消息                  
        //            this.SendMessage("ThiredViewCamera", "RightMouseButtonDown", null);
        //            this.SendMessage("FirstViewCamera", "RightMouseButtonDown", null);
        //            this.SendMessage("Main Camera", "RightMouseButtonDown", null);
        //            this.Focus();
        //            return true;
        //        case 0x205://鼠标右键抬起消息                  
        //            this.SendMessage("ThiredViewCamera", "RightMouseButtonUp", null);
        //            this.SendMessage("FirstViewCamera", "RightMouseButtonUp", null);
        //            this.SendMessage("Main Camera", "RightMouseButtonUp", null);
        //            return true;
        //        case 0x206://鼠标右键点击消息                  
        //            return true;
        //    }
        //    return base.PreProcessMessage(ref msg);
        //}
        #region new mwthod

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int WM_PARENTNOTIFY = 0x210;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PARENTNOTIFY:
                    HideContextMenu(m);
                    break;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 是否隐藏右键菜单，默认还是要显示的，new出来后要设置一下
        /// </summary>
        public bool DisableContextMenu = true;

        private bool _menuHided = false;

        private void HideContextMenu(Message m)
        {
            if (_menuHided) return;
            if (DisableContextMenu == false) return;
            if (m.ToString().Contains("WM_RBUTTONDOWN"))
            {
                if (_menuHided == false)
                {
                    new Thread(() =>
                    {
                        while (_menuHided == false)
                        {
                            HideContextMenu();
                        }
                    }).Start();
                }
            }
        }

        private void HideContextMenu()
        {
            IntPtr handle = FindWindow("Unity.ContextSubmenu", null);
            if (handle != IntPtr.Zero)
            {
                MoveWindow(handle, 0, 0, 10, 10, true); //这个必须要，不然会有阴影，而且怎么Refresh(),Inalidate()都没法去掉
                MoveWindow(handle, 0, 0, 0, 0, true);
                _menuHided = true;
            }
        }
        #endregion
    }
}
