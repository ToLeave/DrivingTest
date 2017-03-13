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
    /// MyError.xaml 的交互逻辑
    /// </summary>
    public partial class MyError : UserControl
    {
        public MyError()
        {
            InitializeComponent();
        }

        List<int> myerr_list = new List<int>();
        List<int> err_list = new List<int>();
        List<int> question_list = new List<int>();
        int err_count = 0;


        private void shunxu_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);


            if (cuo_radioButton1.IsChecked == true)
            {
                err_count = 1;
            }
            if (cuo_radioButton2.IsChecked == true)
            {
                err_count = 2;
            }
            if (cuo_radioButton3.IsChecked == true)
            {
                err_count = 3;
            }
            //是否删除错题
            if (quchu_checkBox.IsChecked == true)
            {
                PublicClass.delerr = true;
            }
            else
            {
                PublicClass.delerr = false;
            }



            var subject = from c in jiakaoDataSet.subject where c.subject.Contains(PublicClass.subjection) select c;

            var user = from c in jiakaoDataSet.errquest where c.user_id == PublicClass.user_id select c;

            if (err_count != 0 && err_count <= 2)//错一次和错两次
            {
                var errquestion = from c in jiakaoDataSet.errquest where c.amount == err_count select c;
                foreach (var err in errquestion)
                {
                    myerr_list.Add(err.question_id);
                }
            }
            else if (err_count == 3)//错三次及以上
            {
                var errquestion = from c in jiakaoDataSet.errquest where c.amount >= 3 select c;
                foreach (var err in errquestion)
                {
                    myerr_list.Add(err.question_id);
                }
            }

            var question = from c in jiakaoDataSet.question where myerr_list.Contains(c.question_id) && c.subject_id == subject.First().subject_id && c.driverlicense_type.Contains(PublicClass.cartype) && user.First().user_id == PublicClass.user_id select c;

            foreach (var qu in question)
            {
                question_list.Add(qu.question_id);
            }

            if (question_list.Count() > 0)
            {
                MainExam ma = new MainExam();
                ma.create_question(0, 0, PublicClass.cartype, PublicClass.subjection, question_list);


                C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "错题");
                if (cp != null)
                {
                    cp.Close();
                }

                C1.WPF.C1Window cwin = new C1.WPF.C1Window();
                cwin.Content = ma;
                cwin.Name = "驾考";
                cwin.Header = "错题练习";
                cwin.ShowModal();
                cwin.IsActive = true;
                cwin.WindowState = C1.WPF.C1WindowState.Maximized;
                cwin.Closing += new System.ComponentModel.CancelEventHandler(cwin_Closing);
            }
            else
            {
                if (err_count != 3)
                {
                    MessageBox.Show("做错次数为" + err_count + "的错题为0题", "提示");
                }
                else
                {
                    MessageBox.Show("做错次数为" + err_count + "及其以上的错题为0题", "提示");
                }
            }







        }


        void cwin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PublicClass.question_list = new List<PublicClass.Question>();
        }

        private void suiji_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            if (cuo_radioButton1.IsChecked == true)
            {
                err_count = 1;
            }
            if (cuo_radioButton2.IsChecked == true)
            {
                err_count = 2;
            }
            if (cuo_radioButton3.IsChecked == true)
            {
                err_count = 3;
            }
            //是否删除错题
            if (quchu_checkBox.IsChecked == true)
            {
                PublicClass.delerr = true;
            }
            else
            {
                PublicClass.delerr = false;
            }

            var subject = from c in jiakaoDataSet.subject where c.subject.Contains(PublicClass.subjection) select c;

            var user = from c in jiakaoDataSet.errquest where c.user_id == PublicClass.user_id select c;

            if (err_count != 0 && err_count <= 2)//错一次和错两次
            {
                var errquestion = from c in jiakaoDataSet.errquest where c.amount == err_count && c.amount >= err_count select c;

                foreach (var err in errquestion)
                {
                    myerr_list.Add(err.question_id);
                }
            }
            else if (err_count == 3)//错三次及以上
            {
                var errquestion = from c in jiakaoDataSet.errquest where c.amount >= 3 select c;
                foreach (var err in errquestion)
                {
                    myerr_list.Add(err.question_id);
                }
            }

            var question = from c in jiakaoDataSet.question where myerr_list.Contains(c.question_id) && c.subject_id == subject.First().subject_id && c.driverlicense_type.Contains(PublicClass.cartype) && user.First().user_id == PublicClass.user_id select c;
            foreach (var qu in question)
            {
                question_list.Add(qu.question_id);
            }

            if (question_list.Count() > 0)
            {
                MainExam ma = new MainExam();
                ma.create_question(1, 0, PublicClass.cartype, PublicClass.subjection, question_list);

                C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "错题");
                if (cp != null)
                {
                    cp.Close();
                }

                C1.WPF.C1Window cwin = new C1.WPF.C1Window();
                cwin.Content = ma;
                cwin.Name = "驾考";
                cwin.Header = "错题练习";
                cwin.ShowModal();
                cwin.IsActive = true;
                cwin.WindowState = C1.WPF.C1WindowState.Maximized;
                cwin.Closing += new System.ComponentModel.CancelEventHandler(cwin_Closing);
            }
            else
            {
                if (err_count != 3)
                {
                    MessageBox.Show("做错次数为" + err_count + "的错题为0题", "提示");
                }
                else
                {
                    MessageBox.Show("做错次数为" + err_count + "及其以上的错题为0题", "提示");
                }
            }






        }



        //删除错题
        private void shanchu_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            int[] err_c = new int[3]{10000,10000,10000};//本地用 需删错题次数
            int[] err_a = new int[3];//后台用 需删错题次数
            int m = 0;

            if (shanchu_button.Content.ToString() == "删除错题")
            {
                checkBox1.Visibility = System.Windows.Visibility.Visible;
                checkBox2.Visibility = System.Windows.Visibility.Visible;
                checkBox3.Visibility = System.Windows.Visibility.Visible;

                rectangle1.Visibility = System.Windows.Visibility.Visible;
                shunxu_button.Visibility = System.Windows.Visibility.Hidden;
                suiji_button.Visibility = System.Windows.Visibility.Hidden;
                quchu_checkBox.Visibility = System.Windows.Visibility.Hidden;
                shanchu_button.Content = "确认删除";
                MessageBox.Show("请勾选您要删除的错题库并点击确认删除按钮!", "提示");
            }
            else if (shanchu_button.Content.ToString() == "确认删除")
            {

                MessageBoxResult result = MessageBox.Show("确定删除吗？", "询问", MessageBoxButton.OKCancel);

                //确认删除
                if (result == MessageBoxResult.OK)
                {
                    if (checkBox1.IsChecked == true)
                    {
                        err_a[0] = err_c[0] = 1;                       
                    }
                    if (checkBox2.IsChecked == true)
                    {
                        err_a[1] = err_c[1] = 2;
                    }
                    if (checkBox3.IsChecked == true)
                    {
                        err_a[2] = err_c[2] = 3;
                    }


                    var errquestion = from c in jiakaoDataSet.errquest where c.amount == err_c[0] || c.amount == err_c[1] || c.amount >= err_c[2] select c;
                    int o = errquestion.Count();
                    foreach (var err in errquestion)
                    {
                        err_list.Add(err.errquest_id);
                    }

                    for (int i = 0; i < err_list.Count(); i++)
                    {
                        jiakaoDataSet.errquest.FindByerrquest_id(err_list[i]).Delete();//删除错题
                        m = 1;
                    }

                    try
                    {
                        string loginstr = null;
                        HttpWebResponse response = null;
                        StreamReader reader = null;

                        string amount = string.Join(",", err_a);

                        string url = PublicClass.http + @"/returnjsons/del_errquests?user_id=" + PublicClass.user_id + "&amount=" + amount;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//注册 url
                        request.Method = "GET";
                        request.Timeout = 10000;
                        response = (HttpWebResponse)request.GetResponse();
                        reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                        loginstr = reader.ReadToEnd();

                        response.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    if (m == 1)
                    {
                        MessageBox.Show("错题删除成功");
                    }

                    jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                    jiakaoDataSet.errquest.AcceptChanges();

                    checkBox1.Visibility = System.Windows.Visibility.Hidden;
                    checkBox2.Visibility = System.Windows.Visibility.Hidden;
                    checkBox3.Visibility = System.Windows.Visibility.Hidden;

                    rectangle1.Visibility = System.Windows.Visibility.Hidden;
                    shunxu_button.Visibility = System.Windows.Visibility.Visible;
                    suiji_button.Visibility = System.Windows.Visibility.Visible;
                    quchu_checkBox.Visibility = System.Windows.Visibility.Visible;
                    shanchu_button.Content = "删除错题";
                }

                //取消删除
                if (result == MessageBoxResult.Cancel)
                {

                }


            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cuo_radioButton1.IsChecked = true;

            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DrivingTest;component/Images/窗体背景.png"));
            b.Stretch = Stretch.Fill;
            this.Background = b;
        }


        private void chakan_button_Click(object sender, RoutedEventArgs e)
        {
            LookError lo = new LookError();
            C1.WPF.C1Window clo = new C1.WPF.C1Window();
            clo.Content = lo;
            clo.Name = "查看错题";
            clo.Header = "我的错题";
            clo.ShowModal();
            clo.IsActive = true;
            clo.WindowState = C1.WPF.C1WindowState.Maximized;
            clo.Closing += new System.ComponentModel.CancelEventHandler(cwin_Closing);

            //ma.create_question(1, 0, PublicClass.cartype, PublicClass.subjection, question_list);

            //C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "错题");
            //if (cp != null)
            //{
            //    cp.Close();
            //}
        }
    }
}
