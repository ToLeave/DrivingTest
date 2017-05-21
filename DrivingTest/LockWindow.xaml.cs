using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace DrivingTest
{
    /// <summary>
    /// LockWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LockWindow : Window
    {
        public LockWindow()
        {
            InitializeComponent();
            //this.Closing += new System.ComponentModel.CancelEventHandler(LockWindow_Closing);
        }

        //void LockWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (!bo)
        //    {
        //        e.Cancel = true;
        //    }
        //    else
        //    {
        //        C1.WPF.C1Window me = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "驾考");
        //        if (me != null)
        //        {
        //            me.Visibility = System.Windows.Visibility.Visible;
        //        }
        //        C1.WPF.C1Window ms = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "章节选择");
        //        if (ms != null)
        //        {
        //            ms.Visibility = System.Windows.Visibility.Visible;
        //        }
        //        e.Cancel = false;
        //    }
        //}

        private DispatcherTimer timer = new DispatcherTimer();

        //private const int GWL_STYLE = -16;
        //private const int WS_SYSMENU = 0x80000;
        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        //[DllImport("user32.dll")]
        //private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        bool bo = true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var hwnd = new WindowInteropHelper(this).Handle;
            //SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            this.ResizeMode = System.Windows.ResizeMode.CanMinimize;

            //设置定时器
            timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime nowtime = DateTime.Parse(time_textBlock.Text);

                nowtime = nowtime.Subtract(TimeSpan.FromSeconds(1));
                if (time_textBlock.Text == "00:00:00")
                {
                    timer.Stop();
                    bo = true;
                    this.Close();
                }
                else
                {
                    time_textBlock.Text = nowtime.ToLongTimeString();
                }

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!bo)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        } 
    }
}
