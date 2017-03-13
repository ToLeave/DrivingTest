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
    /// LookError.xaml 的交互逻辑
    /// </summary>
    public partial class LookError : UserControl
    {
        public LookError()
        {
            InitializeComponent();
        }

        List<int> question_list = new List<int>();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            var subject = from c in jiakaoDataSet.subject where c.subject.Contains(PublicClass.subjection) select c;

            var user = from c in jiakaoDataSet.errquest where c.user_id == PublicClass.user_id select c;

            var question = from c in jiakaoDataSet.question where c.subject_id == subject.First().subject_id && c.driverlicense_type.Contains(PublicClass.cartype) && user.First().user_id == PublicClass.user_id select c;

            foreach (var qu in question)
            {
                question_list.Add(qu.question_id);
            }

            dataGrid1.ItemsSource = question_list;
        }
    }

    public class Question
    {
        public string question_id { get; set; }
        public string check_answer { get; set; }
        public string select_answer { get; set; }
        public string question_type { get; set; }

    }
}
