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
            ServicePointManager.DefaultConnectionLimit = 1000;
            HttpWebResponse response = null;
            StreamReader reader = null;
            //string url = PublicClass.http + @"/returnjsons/t_errquests?";

            List<int> q_list = new List<int>();
            List<int> a_list = new List<int>();
            Random ran = new Random();
            for (int i = 0; i < 10000; i++)
            {
                q_list.Add(i);
                a_list.Add(i);
            }

            int arr_count = q_list.Count / 50;
            arr_count++;



            for (int cou = 0; cou < arr_count; cou++)
            {
                string url = PublicClass.http + @"/returnjsons/t_errquests?user_id=1&";
                for (int i = cou * 50; i < (cou + 1) * 50; i++)
                {
                    if (i < a_list.Count)
                    {
                        url += "q[]=" + q_list[i] + "&a[]=" + a_list[i] + "&";
                    }
                }
                url = url.Substring(0, url.Length - 1);
                int a = url.Length;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//
                request.Method = "GET";
                
                request.Timeout = 10000;
                response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }


            //int tem = 1000;
            //for (int i = 100; i < 200; i++)
            //{
            //    url += "q[]=" + i + "&" + "a[]=" + i + "&";
            //}
            //if (tem != 0)
            //{

            //    url = url.Substring(0, url.Length - 1);
            //    int a = url.Length;
            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//
            //    request.Method = "GET";
            //    request.Timeout = 10000;
            //    response = (HttpWebResponse)request.GetResponse();
            //    response.Close();
            //}
            
        }
    }
}
