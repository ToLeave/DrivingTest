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
using System.Net;
using System.IO;

namespace DrivingTest
{
    /// <summary>
    /// test.xaml 的交互逻辑
    /// </summary>
    public partial class test : UserControl
    {
        public test()
        {
            InitializeComponent();
        }

        private void ss_Click(object sender, RoutedEventArgs e)
        {
            HttpWebResponse response = null;
            StreamReader reader = null;
            for (int i = 0; i < 1000; i++)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PublicClass.http + @"/returnjsons/t_errquests?login=" + i);//
                show.Text = i.ToString();
                request.Method = "GET";
                request.Timeout = 10000;
                response = (HttpWebResponse)request.GetResponse();
                response.Close();
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
}
