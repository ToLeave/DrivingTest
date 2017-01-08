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
using System.Net;
using System.IO;

namespace DrivingTest
{
    /// <summary>
    /// PreparePage.xaml 的交互逻辑
    /// </summary>
    public partial class PreparePage : UserControl
    {
        public PreparePage()
        {
            InitializeComponent();
        }

        private void kaikao_Click(object sender, RoutedEventArgs e)
        {
            MainExam ma = new MainExam();         
            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            ma.create_question(1, 1, PublicClass.cartype, PublicClass.subjection, null);
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.WindowState = C1.WPF.C1WindowState.Maximized;
            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "驾驶理论考试系统");
            if (cp != null)
            {
                cp.Close();
            }
            cwin.Show();
            cwin.Focus();
        
            cwin.Closing += new System.ComponentModel.CancelEventHandler(cwin_Closing);

            
        }

        //关闭时上传错题
        void cwin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

            var errquestion = (from c in jiakaoDataSet.errquest where c.user_id == PublicClass.user_id && c.user_id > 0 && c.user_id == PublicClass.user_id select c).ToArray();

            PublicClass.question_list = new List<PublicClass.Question>();

            try
            {
                ServicePointManager.DefaultConnectionLimit = 1000;
                HttpWebResponse response = null;

                string url = PublicClass.http + @"/returnjsons/t_errquests?" + "command=clear&user_id=" + PublicClass.user_id ;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//提交请求
                request.Method = "GET";

                request.Timeout = 10000;
                response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                ServicePointManager.DefaultConnectionLimit = 1000;
                HttpWebResponse response = null;
                //StreamReader reader = null;

                int arr_count = errquestion.Count() / 60;
                arr_count++;

                for (int cou = 0; cou < arr_count; cou++)
                {
                    string url = PublicClass.http + @"/returnjsons/t_errquests?user_id=" + PublicClass.user_id + "&";
                    for (int i = cou * 60; i < (cou + 1) * 60; i++)
                    {
                        if (i < errquestion.Count())
                        {
                            url += "q[]=" + errquestion[i].question_id + "&a[]=" + errquestion[i].amount + "&";
                        }
                    }
                    url = url.Substring(0, url.Length - 1);
                    int a = url.Length;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//上传题目
                    request.Method = "GET";

                    request.Timeout = 10000;
                    response = (HttpWebResponse)request.GetResponse();
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void preparepage_Loaded(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter jiakaoDataSetuserTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter();
            jiakaoDataSetuserTableAdapter.Fill(jiakaoDataSet.user);

            var setting = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            var user = from c in jiakaoDataSet.user where c.user_id == PublicClass.user_id select c;
            foreach (var s in setting)
            {
                textBlock4.Text = s.testbench_number.ToString();//考台
            }
            foreach (var u in user)
            {
                if (u.idcard != "")
                {
                    textBlock1.Text = u.idcard;//身份证号
                }
                if (u.name != "")
                {
                    textBlock2.Text = u.name;//名字
                }
            }
        }
    }
}
