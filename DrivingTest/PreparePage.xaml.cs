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
            
            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "驾驶理论考试系统");
            if (cp != null)
            {
                cp.Close();
            }

            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            ma.create_question(1, 1, "C1", "科目一", null);
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.Show();
            cwin.Closing += new System.ComponentModel.CancelEventHandler(cwin_Closing);

            //this.Content = ma;
            //cwin.WindowState = C1.WPF.C1WindowState.Maximized;
            //cwin.CenterOnScreen();
            //cwin.Margin = new Thickness(0);
            //cwin.Width = SystemParameters.PrimaryScreenWidth;
            //cwin.Height = SystemParameters.WorkArea.Height;
            cwin.WindowState = C1.WPF.C1WindowState.Maximized;
        }

        void cwin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

            //List<int> q_list = new List<int>();
            //List<int> a_list = new List<int>();

            //var login = from c in jiakaoDataSet.user where c.login == PublicClass.login select c;
            //int loginid = 0;
            //foreach (var id in login)
            //{
            //    loginid = id.user_id;
            //}

            var errquestion = (from c in jiakaoDataSet.errquest where c.user_id == PublicClass.user_id && c.user_id > 0 select c).ToArray();
            //var err_list = errquestion.ToArray();
            //int errquestion_id = 0;
            //foreach (var id in errquestion)
            //{
            //        q_list.Add(id.question_id);
            //        a_list.Add(id.amount);
            //}
   
            try
            {
                ServicePointManager.DefaultConnectionLimit = 1000;
                HttpWebResponse response = null;
                //StreamReader reader = null;

                int arr_count = errquestion.Count() / 60;
                arr_count++;

                for (int cou = 0; cou < arr_count; cou++)
                {
                    string url = PublicClass.http + @"/returnjsons/t_errquests?" + PublicClass.user_id + "&";
                    for (int i = cou * 60; i < (cou + 1) * 60; i++)
                    {
                        if (i < errquestion.Count())
                        {
                            url += "q[]=" + errquestion[i].question_id + "&a[]=" + errquestion[i].amount + "&";
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
            //jiakaoDataSet.errquest.AcceptChanges();

        }

        private void preparepage_Loaded(object sender, RoutedEventArgs e)
        {

            // 不要在设计时加载数据。
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//在此处加载数据并将结果指派给 CollectionViewSource。
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }
    }
}
