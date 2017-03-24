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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            C1.WPF.C1Window ms = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "章节选择");
            if (ms != null)
            {
                ms.Visibility = System.Windows.Visibility.Visible;
            }
            C1.WPF.C1Window me = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "驾考");
            if (me != null)
            {
                me.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
