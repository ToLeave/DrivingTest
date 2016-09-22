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
using System.Windows.Threading;
using System.Data.OleDb;

namespace DrivingTest
{
    /// <summary>
    /// MainExam.xaml 的交互逻辑
    /// </summary>
    public partial class MainExam : Window
    {
        private DispatcherTimer timer;
        private ProcessCount processCount;

        List<string> question_pd_list = new List<string>();//随机题号列表
        List<PublicClass.Question> question_list = new List<PublicClass.Question>();

        int question_c = 0;
        int lab_index = 0;

        public MainExam()
        {
            InitializeComponent();
            //this.Loaded += new RoutedEventHandler(Window_Loaded);
        }



        public static T FindChild<T>(DependencyObject parent, string childName)
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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);



            random_question();//随机抽题

            create_question_num();//生成题号

            questionindex();//初始化第一题



            #region 启动定时器
            //设置定时器
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
            timer.Tick += new EventHandler(timer_Tick);

            //转换成秒数
            Int32 hour = Convert.ToInt32(HourArea.Text);
            Int32 minute = Convert.ToInt32(MinuteArea.Text);
            Int32 second = Convert.ToInt32(SecondArea.Text);

            //处理倒计时的类
            processCount = new ProcessCount(hour * 3600 + minute * 60 + second);
            CountDown += new CountDownHandler(processCount.ProcessCountDown);

            //开启定时器
            timer.Start();
            #endregion
        }

        //随机抽答案
        private List<PublicClass.Answer> random_answer(int question_id)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
            List<PublicClass.Answer> newanswer = new List<PublicClass.Answer>();
            var answer = from c in jiakaoDataSet.answer where c.question_id == question_id select c;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < answer.Count(); i++)
            {
                int ran = random.Next(0, answer.Count());
                PublicClass.Answer temanswer = new PublicClass.Answer();
                int temstep = 0;
                foreach (var an in answer)
                {

                    if (ran == temstep)
                    {
                        temanswer.answer_id = an.answer_id;
                        temanswer.isright = int.Parse(an.is_right);
                        newanswer.Add(temanswer);
                        break;
                    }
                    temstep++;
                }

                int an_count = 0;
                an_count = (from c in newanswer where c.answer_id == temanswer.answer_id select c).Count();
                if (an_count > 1)
                {
                    newanswer.Remove(temanswer);
                    i--;
                }


            }

            return newanswer;
        }

        //随机抽题目
        private void random_question()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            int question_pd_count = (from c in jiakaoDataSet.question where c.question_type.Contains("XZ") select c).Count();
            var question_pd = from c in jiakaoDataSet.question where c.question_type.Contains("XZ") select c;
            question_c = question_pd_count;
            int question_count = question_pd_count;
            if (question_count > 100)
            {
                question_count = 100;
            }
            for (int i = 0; i < question_count; i++)
            {

                Random random = new Random(Guid.NewGuid().GetHashCode());
                int ran = random.Next(0, question_pd_count);
                int eachcount = 0;
                foreach (var qu in question_pd)
                {
                    if (eachcount == ran)
                    {
                        PublicClass.Question question = new PublicClass.Question();
                        question.question_id = qu.question_id;
                        question.check_answer = true;
                        question.select_answer = "";
                        question.question_type = qu.question_type;
                        question.answer = random_answer(qu.question_id);
                        question_list.Add(question);
                        int tem_list_count = (from c in question_list where c.question_id == qu.question_id select c).Count();
                        if (tem_list_count > 1)
                        {
                            question_list.Remove(question);
                            i--;
                        }
                        break;

                    }
                    eachcount++;
                }



                //qutstion_pd_list.Add(  random.Next(0,question_pd_count+1) );
            }

            //foreach (var qu in question_list)
            //{
            //    qu.answer = random_answer(qu.question_id);
            //}













        }

        //生成题号
        private void create_question_num()
        {
            int cou = question_c;
            for (int i = 0; i < cou; i++)
            {
                int x = i / 10;
                int y = i % 10;
                x *= 31;
                y *= 26;
                QuestionNum qu = new QuestionNum();

                qu.Margin = new Thickness(y, x, 0, 0);
                qu.label1.Content = i + 1;
                qu.Name = "q" + i.ToString();
                qu.MouseDown += new MouseButtonEventHandler(OK);
                qu.setnum(i + 1, true, "");
                dati_canvas.Children.Add(qu);
            }
            dati_canvas.Height = cou / 10 * 31;
        }

        //初始化第一题
        private void questionindex()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            int question_index = 0;
            var question = from c in jiakaoDataSet.question where c.question_id == question_list[question_index].question_id select c;
            foreach (var temqu in question)
            {
                timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;
                break;
            }

            int step = 0;
            foreach (var an in question_list[question_index].answer)
            {
                PublicClass.Answer myan = an as PublicClass.Answer;
                var teman = from c in jiakaoDataSet.answer where c.answer_id == myan.answer_id select c;
                foreach (var temann in teman)
                {
                    switch (step)
                    {
                        case 0:
                            xuanxiang_textBlock1.Text = "A." + temann.answer;
                            break;
                        case 1:
                            xuanxiang_textBlock2.Text = "B." + temann.answer;
                            break;
                        case 2:
                            xuanxiang_textBlock3.Text = "C." + temann.answer;
                            break;
                        case 3:
                            xuanxiang_textBlock4.Text = "D." + temann.answer;
                            break;
                    }

                }

                step++;
            }

            foreach (var q in dati_canvas.Children)
            {
                QuestionNum temnum = q as QuestionNum;
                if (temnum != null && temnum.Name == "q0")
                {
                    temnum.setbackcolor();
                }
            }

        }

        //题号单击事件
        void OK(object sender, MouseButtonEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            foreach (var i in dati_canvas.Children)
            {
                QuestionNum myqu = i as QuestionNum;
                if (myqu != null)
                {
                    myqu.canvas1.Background = Brushes.White;
                }


            }
            QuestionNum qu = sender as QuestionNum;
            qu.setbackcolor();

            int question_index = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1));
            lab_index = question_index;
            var question = from c in jiakaoDataSet.question where c.question_id == question_list[question_index].question_id select c;
            foreach (var temqu in question)
            {
                timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;
                break;
            }

            int step = 0;
            foreach (var an in question_list[question_index].answer)
            {
                PublicClass.Answer myan = an as PublicClass.Answer;
                var teman = from c in jiakaoDataSet.answer where c.answer_id == myan.answer_id select c;
                foreach (var temann in teman)
                {
                    switch (step)
                    {
                        case 0:
                            xuanxiang_textBlock1.Text = "A." + temann.answer;
                            break;
                        case 1:
                            xuanxiang_textBlock2.Text = "B." + temann.answer;
                            break;
                        case 2:
                            xuanxiang_textBlock3.Text = "C." + temann.answer;
                            break;
                        case 3:
                            xuanxiang_textBlock4.Text = "D." + temann.answer;
                            break;
                    }

                }

                step++;
            }




        }

        string questionnum;
        //上一题
        private void up_button_Click(object sender, RoutedEventArgs e)
        {
            int question_id = 0;
            foreach (var lab in dati_canvas.Children)
            {
                QuestionNum qu = lab as QuestionNum;
                if (qu.canvas1.Background == Brushes.SkyBlue)
                {
                    question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1)) - 1;
                    if (question_id >= 0)
                    {
                        select_question(question_id);
                    }
                    break;
                }
            }
        }

        //下一题
        private void do_button_Click(object sender, RoutedEventArgs e)
        {

            //foreach (var a in dati_canvas.Children)
            //{
            // UIHelper.FindChild<TextBox>(Application.Current.MainWindow, "myTextBoxName");
            //QuestionNum aa = MainExam.FindChild<QuestionNum>(Application.Current.MainWindow, "q1");
            //aa.setbackcolor();
            int question_id = 0;
            foreach (var lab in dati_canvas.Children)
            {
                QuestionNum qu = lab as QuestionNum;
                if (qu.canvas1.Background == Brushes.SkyBlue)
                {
                    question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1)) + 1;
                    if (question_id < question_c)
                    {
                        select_question(question_id);
                    }
                    break;
                }
            }

            //}


        }


        private void select_question(int question_id)//
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            foreach (var i in dati_canvas.Children)
            {
                QuestionNum myqu = i as QuestionNum;
                if (myqu != null)
                {
                    myqu.canvas1.Background = Brushes.White;
                }
                if (myqu.Name.ToString().Substring(1, myqu.Name.ToString().Length - 1) == question_id.ToString())
                {
                    myqu.setbackcolor();
                }


            }
            //QuestionNum qu = sender as QuestionNum;
            //qu.setbackcolor();

            //int question_index = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1));
            //lab_index = question_index;
            var question = from c in jiakaoDataSet.question where c.question_id == question_list[question_id].question_id select c;
            foreach (var temqu in question)
            {
                timu_textBlock.Text = question_id + 1 + "." + temqu.question_name;
                break;
            }

            int step = 0;
            foreach (var an in question_list[question_id].answer)
            {
                PublicClass.Answer myan = an as PublicClass.Answer;
                var teman = from c in jiakaoDataSet.answer where c.answer_id == myan.answer_id select c;
                foreach (var temann in teman)
                {
                    switch (step)
                    {
                        case 0:
                            xuanxiang_textBlock1.Text = "A." + temann.answer;
                            break;
                        case 1:
                            xuanxiang_textBlock2.Text = "B." + temann.answer;
                            break;
                        case 2:
                            xuanxiang_textBlock3.Text = "C." + temann.answer;
                            break;
                        case 3:
                            xuanxiang_textBlock4.Text = "D." + temann.answer;
                            break;
                    }

                }

                step++;
            }
        }

        #region 计时器
        /// <summary>
        /// Timer触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (OnCountDown())
            {
                HourArea.Text = processCount.GetHour();
                MinuteArea.Text = processCount.GetMinute();
                SecondArea.Text = processCount.GetSecond();
            }
            else
                timer.Stop();
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        public event CountDownHandler CountDown;
        public bool OnCountDown()
        {
            if (CountDown != null)
                return CountDown();

            return false;
        }


    }

    /// <summary>
    /// 处理倒计时的委托
    /// </summary>
    /// <returns></returns>
    public delegate bool CountDownHandler();
        #endregion
}







