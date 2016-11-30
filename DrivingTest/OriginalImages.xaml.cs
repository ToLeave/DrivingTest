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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace DrivingTest
{
    /// <summary>
    /// OriginalImages.xaml 的交互逻辑
    /// </summary>
    public partial class OriginalImages : UserControl
    {
        public OriginalImages()
        {
            InitializeComponent();
        }

        private void image_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = ((MediaElement)sender).Position.Add(TimeSpan.FromMilliseconds(1));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (PublicClass.question_image != "")
            {
                try
                {
                    string path = System.Windows.Forms.Application.StartupPath + "\\Image\\" + PublicClass.question_image;

                    FileInfo fd = new FileInfo(path);
                    int Length = (int)fd.Length;
                    if (Length > 0)
                    {
                        image.Source = new Uri(path, UriKind.Absolute);
                    }
                    else
                    {
                        image.Source = null;
                    }
                }
                catch
                {
                    MessageBox.Show("图片损坏或被不存在,请重启软件并更新!", "提示");
                }
            }
            else
            {
                image.Source = null;
            }
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "查看原图");
            if (cp != null)
            {
                cp.Close();
            }
        }

    }
}
