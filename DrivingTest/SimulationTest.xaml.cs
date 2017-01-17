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
    /// SimulationTest.xaml 的交互逻辑
    /// </summary>
    public partial class SimulationTest : UserControl
    {
        public SimulationTest()
        {
            InitializeComponent();
        }

        private void kaikao_Click(object sender, RoutedEventArgs e)
        {
            PreparePage pr = new PreparePage();
            C1.WPF.C1Window c1pr = new C1.WPF.C1Window();
            c1pr.WindowState = C1.WPF.C1WindowState.Maximized;
            c1pr.Header = "驾驶理论考试系统";
            c1pr.Content = pr;
            C1.WPF.C1Window ma = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "全真模拟");
            if (ma != null)
            {
                ma.Close();
            }
            c1pr.ShowModal();
            //c1pr.ToolTip = "驾驶理论考试系统";
            c1pr.Name = "驾驶理论考试系统";
            c1pr.Focus();
           
        
            //C1.WPF.C1Window ma1 = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "科目一");
            //if (ma != null)
            //{
            //    //ma1.Visibility = System.Windows.Visibility.Collapsed;
            //    ma1.Close();
            //    ma1.Width = 0;
            //    ma1.Height = 0;

            //}
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            this.Background = b;
        }
    }
}
