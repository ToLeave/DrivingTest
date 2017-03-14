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
using System.Threading;

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

        List<Question> question_list = new List<Question>();
        List<int> question = new List<int>();

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

            var que = from c in jiakaoDataSet.question where c.subject_id == subject.First().subject_id && c.driverlicense_type.Contains(PublicClass.cartype) && user.First().user_id == PublicClass.user_id select c;

            foreach (var qu in que)
            {
                question.Add(qu.question_id);
            }

            for (int i = 0; i < question.Count(); i++)
            {
                ThreadPool.QueueUserWorkItem(Thread_Question, question[i]);
            }


            dataGrid1.ItemsSource = question_list;
            //dataGrid1
        }

        private void Thread_Question(object quesid)
        {
            //var q = from c in PublicClass.question_data where c.question_id.ToString() == quesid.ToString() select c;
            //Question local_question = new Question();
            //local_question.question_id = question.First().question_id;
            //local_question.check_answer = true;
            //local_question.select_answer = "";
            //local_question.question_type = question.First().question_type;
            //local_question.sz = true;
            //local_question.rept_do = 0;
            //if (PublicClass.create_method == 1)
            //{
            //    local_question.answer = random_answer(question.First().question_id);
            //}
            //else
            //{
            //    local_question.answer = order_answer(question.First().question_id);
            //}
            ////ThreadPool.QueueUserWorkItem(local_question.answer= question.First().question_id);
            //question_list.Add(local_question);
        }
    }

    public class Question
    {
        public int id { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public string frequency { get; set; }
        public string chapter { get; set; }

    }
}
