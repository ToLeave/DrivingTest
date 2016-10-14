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

            var subject = from c in jiakaoDataSet.subject where c.subject.Contains(PublicClass.subjection) select c;
            var errquestion = from c in jiakaoDataSet.errquest where c.amount == err_count select c;

            foreach(var err in errquestion)
            {
                myerr_list.Add(err.question_id);
            }

                var question = from c in jiakaoDataSet.question where myerr_list.Contains(c.question_id) && c.subject_id == subject.First().subject_id && c.driverlicense_type.Contains(PublicClass.cartype) select c;
                foreach(var qu in question)
                {
                    question_list.Add(qu.question_id);
                }
                MainExam ma = new MainExam();
                ma.create_question(0, 0, PublicClass.cartype, PublicClass.subjection, question_list);

                C1.WPF.C1Window cwin = new C1.WPF.C1Window();
                cwin.Content = ma;
                cwin.Name = "驾考";
                cwin.Header = "驾驶理论考试系统";
                cwin.Show();
                cwin.WindowState = C1.WPF.C1WindowState.Maximized;

        }

        private void suiji_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

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

            var subject = from c in jiakaoDataSet.subject where c.subject.Contains(PublicClass.subjection) select c;
            var errquestion = from c in jiakaoDataSet.errquest where c.amount == err_count select c;

            foreach (var err in errquestion)
            {
                myerr_list.Add(err.question_id);
            }

            var question = from c in jiakaoDataSet.question where myerr_list.Contains(c.question_id) && c.subject_id == subject.First().subject_id && c.driverlicense_type.Contains(PublicClass.cartype) select c;
            foreach (var qu in question)
            {
                question_list.Add(qu.question_id);
            }
            MainExam ma = new MainExam();
            ma.create_question(0, 0, PublicClass.cartype, PublicClass.subjection, question_list);

            C1.WPF.C1Window cwin = new C1.WPF.C1Window();
            cwin.Content = ma;
            cwin.Name = "驾考";
            cwin.Header = "驾驶理论考试系统";
            cwin.Show();
            cwin.WindowState = C1.WPF.C1WindowState.Maximized;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
