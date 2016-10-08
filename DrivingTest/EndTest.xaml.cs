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
    /// EndTest.xaml 的交互逻辑
    /// </summary>
    public partial class EndTest : Window
    {
        public EndTest()
        {
            InitializeComponent();
        }

        public static T FindChild<T>(DependencyObject parent, string childName)//查找控件
where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // 如果子控件不是需查找的控件类型 
                T childType = child as T;
                if (childType == null)
                {
                    // 在下一级控件中递归查找 
                    foundChild = FindChild<T>(child, childName);
                    // 找到控件就可以中断递归操作  
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // 如果控件名称符合参数条件 
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // 查找到了控件 
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }
    
        private void end_button_Click(object sender, RoutedEventArgs e)
        {
            C1.WPF.C1Window ma = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "c1");
            if (ma != null)
            {
                ma.Close();
            }
            this.Close();

            //IntPtr hwndCalc = FindWindow(null, "计算器");
            //foreach (var item in Application.OpenForms)
            //{
            //    if (item is Form1) item.Close();
            //}
        }

    }
}
