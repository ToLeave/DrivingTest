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

        private string[] questions = new string[100];
        private int currentquestion = 0;

        public MainExam()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_Loaded);
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

            create_question_num();

            random_question();

            int i = 0;
            //while (dr.Read())
            //{

            //    questions[i] = " " + dr.GetString(1) + "\n" +
            //                    "\n" + "  A." + dr.GetString(2) +
            //                    "\n" + "  B." + dr.GetString(3) +
            //                    "\n" + "  C." + dr.GetString(4) +
            //                    "\n" + "  D." + dr.GetString(5);
            //    i++;
            //}



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

        //随机抽题
        List<int> qutstion_pd_list = new List<int>();

        private void random_question()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            int question_pd_count = (from c in jiakaoDataSet.question where c.is_judge ==0 select c).Count();
            var question_pd = from c in jiakaoDataSet.question where c.is_judge == 0 select c;
            for (int i = 0; i < 10; i++)
            {
                
                Random random = new Random(Guid.NewGuid().GetHashCode());
                int ran = random.Next(0, question_pd_count);
                int eachcount = 0;
                foreach (var qu in question_pd)
                {
                    if (eachcount == ran)
                    {
                        qutstion_pd_list.Add(qu.question_id);
                        int tem_list_count = (from c in qutstion_pd_list where c == qu.question_id select c).Count();
                        if (tem_list_count > 1)
                        {
                            qutstion_pd_list.Remove(qu.question_id);
                            i--;
                        }
                        break;
                    }
                    eachcount++;
                }
                  
                
                
                //qutstion_pd_list.Add(  random.Next(0,question_pd_count+1) );
            }
            int cl;
            


        }

        //生成题号
        private void create_question_num()
        {
            for (int i = 0; i < 100; i++)
            {
                int x = i / 10;
                int y = i % 10;
                x *= 31;
                y *= 26;
                QuestionNum qu = new QuestionNum();

                qu.Margin = new Thickness(y, x, 0, 0);
                qu.label1.Content = i + 1;
                qu.Name = "qu" + i.ToString();
                qu.MouseDown += new MouseButtonEventHandler(OK);
                dati_canvas.Children.Add(qu);
            }
        }


        void OK(object sender, MouseButtonEventArgs e)
        {
            foreach (var i in dati_canvas.Children)
            {
                QuestionNum myqu = i as QuestionNum;
                if (myqu != null)
                {
                    myqu.canvas1.Background = Brushes.White;
                }

                QuestionNum qu = sender as QuestionNum;
                qu.canvas1.Background = Brushes.SkyBlue;


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







