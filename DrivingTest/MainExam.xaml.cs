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
using System.Windows.Controls.Primitives;
using System.IO;
using System.Speech.Synthesis;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Xml;



namespace DrivingTest
{
    /// <summary>
    /// MainExam.xaml 的交互逻辑
    /// </summary>
    public partial class MainExam : UserControl
    {
        #region 公共变量
        private DispatcherTimer timer = new DispatcherTimer();
        //private ProcessCount processCount;

        List<string> question_pd_list = new List<string>();//随机题号列表

        int question_c = 0;//总题数
        int question_x = 0;//选择题总题数
        int quesiton_p = 0;//判断题总题数
        int quesiton_d = 0;//多选题总题数
        int lab_index = 0;
        bool is_click_flag = false;//选答案判断
        int kaoshicishu = 1;//考试次数
        string timer_type = "";
        SpeechSynthesizer synth = new SpeechSynthesizer();//语音阅读
        // Configure the audio output. 
        string current_question_type = "S";//S=单选 M=多选 P=判断

        string imagename = "";//图片文件名
        bool playvoice = false;
        int last_question_lab_index = 0;//上一次选中题标签的索引
        int cur_question_lab_index = 0;//当前选中题标签的索引
        bool first_run = true;//首次运行
        bool check_error = false;//查看错题


        System.Timers.Timer imagetimer = new System.Timers.Timer();
        int imgwidth = 0;
        #endregion


        public MainExam()
        {
            InitializeComponent();
            //this.Loaded += new RoutedEventHandler(Window_Loaded);


        }


        #region 查找控件
        public static T FindChild<T>(DependencyObject parent, string childName)//查找控件
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
        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //guanggao_image.Source = new BitmapImage(new Uri(System.Windows.Forms.Application.StartupPath + "\\Image\\Advertise\\car2.jpg"));

            if (first_run)
            {
                //this.Name = "mainW";
                DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                // 将数据加载到表 question 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

                // 将数据加载到表 answer 中。可以根据需要修改此代码。
                DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
                jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

                DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter jiakaoDataSetuserTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.userTableAdapter();
                jiakaoDataSetuserTableAdapter.Fill(jiakaoDataSet.user);


                register_key();//注册快捷键

                //random_question();//随机抽题

                //create_question_num();//生成题号

                //questionindex();//初始化第一题

                var user = from c in jiakaoDataSet.user where c.user_id == PublicClass.user_id select c;

                foreach (var u in user)
                {
                    string image_name = u.login;

                    if (u.name != "")
                    {
                        name_textBlock.Text = u.name;
                    }
                    if (u.sex != "")
                    {
                        sex_textBlock.Text = u.sex;
                    }
                    BitmapImage imagetemp = new BitmapImage(new Uri(System.Windows.Forms.Application.StartupPath + "\\Image\\User\\" + image_name + ".jpg", UriKind.Relative));//用户头像 相对路径
                    //BitmapImage imagemoren = new BitmapImage(new Uri("\\DrivingTest;component\\Images\\学员头像.png"));
                    string ima = "\"" + imagetemp.ToString() + "\"";
                    if (File.Exists(imagetemp.ToString()))//如果路径存在 相对路径
                    {
                        imagetemp = new BitmapImage(new Uri("\\Image\\User\\" + image_name + ".jpg", UriKind.Relative));//绝对路径
                        touxiang_image.Source = imagetemp; //绝对路径
                    }
                    else
                    {

                    }
                    //else
                    //{
                    //    touxiang_image.Source = imagemoren;
                    //}

                }
                chouti_count.Text = question_c.ToString();
                weida.Text = question_c.ToString();

                // Configure the audio output. 
                synth.SetOutputToDefaultAudioDevice();
                synth.SelectVoiceByHints(VoiceGender.Male);
                // Speak a string.
                synth.Volume = 100;
                synth.Rate = 0;

                //设置定时器
                timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();


                //转换成秒数
                //Int32 hour = Convert.ToInt32(HourArea.Text);
                //Int32 minute = Convert.ToInt32(MinuteArea.Text);
                //Int32 second = Convert.ToInt32(SecondArea.Text);

                //处理倒计时的类
                //processCount = new ProcessCount(hour * 3600 + minute * 60 + second);
                //CountDown += new CountDownHandler(processCount.ProcessCountDown);

                first_run = false;





                var imggroup = from c in PublicClass.avatar_list where c.avatar_type == "top" select c;
                imgwidth = imggroup.Count() * 100;
                foreach (var img in imggroup)
                {
                    Image image = new Image();
                    image.Width = 100;
                    image.Height = 300;
                    image.Source = new BitmapImage(new Uri(System.Windows.Forms.Application.StartupPath + "\\Image\\" + "\\Avatar\\" + img.avatarurl));
                    image.MouseUp += new MouseButtonEventHandler(image_MouseUp);
                    img_panel.Children.Add(image);
                }
                img_panel.Margin = new Thickness(-imgwidth + 100, 0, 0, 0);

                imagetimer.Interval = 10000;
                imagetimer.Elapsed += new System.Timers.ElapsedEventHandler(imagetimer_Elapsed);
                imagetimer.Start();





            }

        }

        void imagetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ThicknessAnimation panelani = new ThicknessAnimation();
                if (img_panel.Margin.Left == 0)
                {
                    panelani.To = new Thickness(-imgwidth + 100, 0, 0, 0);
                }
                else
                {
                    panelani.To = new Thickness(img_panel.Margin.Left + 100, 0, 0, 0);
                }
                panelani.Duration = TimeSpan.FromSeconds(1);
                img_panel.BeginAnimation(StackPanel.MarginProperty, panelani);
            }));
        }

        void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img != null)
            {
                var link = from c in PublicClass.avatar_list where c.avatarurl.Contains(img.Source.ToString().Split('/').Last()) select c;
                Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = link.First().link;
                proc.Start();

            }
        }

        //随机题目
        private List<PublicClass.Question> random_question(List<PublicClass.Question> question_all, int create_count)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());

            PublicClass.Question question = new PublicClass.Question();
            for (int i = 0; i < question_all.Count(); i++)
            {
                int ran1 = random.Next(0, question_all.Count());
                int ran2 = random.Next(0, question_all.Count());
                if (ran1 != ran2)
                {
                    question = question_all[ran1];
                    question_all[ran1] = question_all[ran2];
                    question_all[ran2] = question;
                }
                else
                {
                    i--;
                }
            }
            while (question_all.Count() > create_count)
            {
                int ran = random.Next(0, question_all.Count());
                question_all.RemoveAt(ran);
            }
            return question_all;

        }


        /// <summary>
        /// 生成题库
        /// </summary>
        /// <param name="create_method">0顺序,1随机</param>
        /// <param name="question_mode">0练习,1考试,2错题</param>
        /// <param name="cartype">车型</param>
        /// <param name="subject">科目</param>
        /// <param name="questions_id">需生成题库题目ID</param>
        public void create_question(int create_method, int question_mode, string cartype, string subject, List<int> questions_id)
        {
            //存参以留重新考试
            PublicClass.create_method = create_method;
            PublicClass.question_mode = question_mode;
            PublicClass.cartype = cartype;
            PublicClass.subject = subject;
            PublicClass.questions_id = questions_id;
            System.Windows.Forms.Application.DoEvents();
            cart_sub(cartype, subject);//显示车型科目

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

            List<PublicClass.Question> question_pd_list = new List<PublicClass.Question>();
            List<PublicClass.Question> question_xz_list = new List<PublicClass.Question>();
            List<PublicClass.Question> question_hh_list = new List<PublicClass.Question>();
            List<PublicClass.Question> question_dx_list = new List<PublicClass.Question>();
            var local_subject = from c in jiakaoDataSet.subject where c.subject.Contains(subject) select c;


            if (question_mode == 1)//考试
            {
                shezhi_grid.Visibility = System.Windows.Visibility.Hidden;
                xianshi_grid.Visibility = System.Windows.Visibility.Hidden;
                tianjiacuoti.Visibility = System.Windows.Visibility.Hidden;
                shanchucuoti.Visibility = System.Windows.Visibility.Hidden;
                guanggao_textBlock.Visibility = System.Windows.Visibility.Hidden;
                //guanggao_image.Visibility = System.Windows.Visibility.Hidden;
                main_image_panel.Visibility = Visibility.Hidden;

                timer_type = "考试";
                var question_pd = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.question_type.Contains("PD") && c.subject_id == local_subject.First().subject_id select c;

                foreach (var qu in question_pd)
                {
                    PublicClass.Question question = new PublicClass.Question();
                    question.question_id = qu.question_id;
                    question.check_answer = true;
                    question.select_answer = "";
                    question.question_type = qu.question_type;



                    question.sz = true;
                    question.rept_do = 0;
                    //question.answer = random_answer(qu.question_id);
                    question_pd_list.Add(question);
                }
                if (subject == "科目四")
                {
                    question_pd_list = random_question(question_pd_list, 20);
                }
                else 
                {
                    question_pd_list = random_question(question_pd_list, 40);
                }


                var question_xz = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.question_type.Contains("XZ") && !c.question_type.Contains("DX") && c.subject_id == local_subject.First().subject_id select c;

                foreach (var qu in question_xz)
                {
                    PublicClass.Question question = new PublicClass.Question();
                    question.question_id = qu.question_id;
                    question.check_answer = true;
                    question.select_answer = "";
                    question.question_type = qu.question_type;
                    question.sz = true;
                    question.rept_do = 0;
                    //question.answer = random_answer(qu.question_id);
                    question_xz_list.Add(question);
                }
                if (subject == "科目四")
                {
                    question_xz_list = random_question(question_xz_list, 23);
                    //question_zhongzhuan_list = question_xz_list;
                }
                else 
                {
                    question_xz_list = random_question(question_xz_list, 60);
                }

                if (subject == "科目四")
                {
                //抽5题多选


                var question_dx = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.question_type.Contains("DX") && c.subject_id == local_subject.First().subject_id select c;
                foreach (var dx in question_dx)
                {
                    PublicClass.Question question = new PublicClass.Question();
                    question.question_id = dx.question_id;
                    question.check_answer = true;
                    question.select_answer = "";
                    question.question_type = dx.question_type;
                    question.sz = true;
                    question.rept_do = 0;
                    //question.answer = random_answer(qu.question_id);
                    question_dx_list.Add(question);
                }
                question_dx_list = random_question(question_dx_list, 5);
                question_xz_list.AddRange(question_dx_list);
                 List<int> questionid=new List<int>();
                 foreach (var id in question_xz_list)
                {
                    questionid.Add(id.question_id);
                }

                

                //抽2题随机
                var question_hh = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.question_type.Contains("XZ") && !questionid.Contains(c.question_id) && c.subject_id == local_subject.First().subject_id select c;
                foreach (var qu in question_hh)
                {
                    PublicClass.Question question = new PublicClass.Question();
                    question.question_id = qu.question_id;
                    question.check_answer = true;
                    question.select_answer = "";
                    question.question_type = qu.question_type;
                    question.sz = true;
                    question.rept_do = 0;
                    //question.answer = random_answer(qu.question_id);
                    question_hh_list.Add(question);
                }
              
                    question_hh_list = random_question(question_hh_list, 2);

                    question_xz_list.InsertRange(23, question_hh_list);

                }
             


                for (int i = 0; i < question_pd_list.Count(); i++)
                {
                    PublicClass.question_list.Add(question_pd_list[i]);
                }
                for (int i = 0; i < question_xz_list.Count(); i++)
                {
                    PublicClass.question_list.Add(question_xz_list[i]);
                    PublicClass.question_list.Last().answer = random_answer(PublicClass.question_list.Last().question_id);
                }
                question_c = PublicClass.question_list.Count();
                create_question_num();
                questionindex();
            }
            else //练习
            {
                timer_type = "练习";
                SecondArea.Text = "00:00:00";
                display_answers();//判断试都显示正确答案
                if (create_method == 0)
                {
                    for (int i = 0; i < questions_id.Count(); i++)
                    {
                        //var question = from c in PublicClass.question_data where c.question_id == questions_id[i] select c;
                        //PublicClass.Question local_question = new PublicClass.Question();
                        //local_question.question_id = question.First().question_id;
                        //local_question.check_answer = true;
                        //local_question.select_answer = "";
                        //local_question.question_type = question.First().question_type;
                        //local_question.sz = true;
                        //local_question.rept_do = 0;
                        //local_question.answer = order_answer(question.First().question_id);
                        //ThreadPool.QueueUserWorkItem(local_question.answer= question.First().question_id);
                        //PublicClass.question_list.Add(local_question);
                        //ThreadPool.SetMaxThreads(16, 2);
                        ThreadPool.QueueUserWorkItem(Thread_Question, questions_id[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < questions_id.Count(); i++)
                    {
                        var question = from c in PublicClass.question_data where c.question_id == questions_id[i] select c;
                        PublicClass.Question local_question = new PublicClass.Question();
                        local_question.question_id = question.First().question_id;
                        local_question.check_answer = true;
                        local_question.select_answer = "";
                        local_question.question_type = question.First().question_type;
                        local_question.sz = true;
                        local_question.rept_do = 0;
                        local_question.answer = order_answer(question.First().question_id);
                        //PublicClass.question_list.Add(local_question);
                        ThreadPool.QueueUserWorkItem(Thread_Question, questions_id[i]);
                    }
                    PublicClass.question_list = random_question(PublicClass.question_list, questions_id.Count());
                }
                //question_c = PublicClass.question_list.Count();
                //create_question_num();
                //questionindex();

            }
            //question_c = PublicClass.question_list.Count();

            //开启定时器
            //timer.Start();



        }


        private void Thread_Question(object quesid)
        {
            var question = from c in PublicClass.question_data where c.question_id.ToString() == quesid.ToString() select c;
            PublicClass.Question local_question = new PublicClass.Question();
            local_question.question_id = question.First().question_id;
            local_question.check_answer = true;
            local_question.select_answer = "";
            local_question.question_type = question.First().question_type;
            local_question.sz = true;
            local_question.rept_do = 0;
            if (PublicClass.create_method == 1)
            {
                local_question.answer = random_answer(question.First().question_id);
            }
            else
            {
                local_question.answer = order_answer(question.First().question_id);
            }
            //ThreadPool.QueueUserWorkItem(local_question.answer= question.First().question_id);
            PublicClass.question_list.Add(local_question);
            if (PublicClass.question_list.Count == PublicClass.questions_id.Count())
            {
                question_c = PublicClass.question_list.Count();
                create_question_num();
                questionindex();
            }
        }


        //随机抽答案
        private List<PublicClass.Answer> random_answer(int question_id)
        {


            List<PublicClass.Answer> newanswer = new List<PublicClass.Answer>();
            var answer = from c in PublicClass.answer_data where c.question_id == question_id select c;
            Random random = new Random(Guid.NewGuid().GetHashCode());


            foreach (var an in answer)
            {
                PublicClass.Answer temanswer = new PublicClass.Answer();
                temanswer.answer_id = an.answer_id;
                temanswer.isright = int.Parse(an.is_right);
                newanswer.Add(temanswer);
            }
            int temcount = 0;
            for (int i = 0; i < answer.Count(); i++)
            {
                int ran1 = random.Next(0, answer.Count());
                int ran2 = random.Next(0, answer.Count());
                if (ran1 != ran2)
                {
                    PublicClass.Answer temanswer = new PublicClass.Answer();
                    temanswer = newanswer[ran1];
                    newanswer[ran1] = newanswer[ran2];
                    newanswer[ran2] = temanswer;
                }
                else
                {
                    i--;
                }
                temcount++;
                if (temcount > 4)
                {
                    i = 10;
                }
            }
            return newanswer;
        }

        //顺序抽答案
        private List<PublicClass.Answer> order_answer(int question_id)
        {


            List<PublicClass.Answer> newanswer = new List<PublicClass.Answer>();
            var answer = from c in PublicClass.answer_data where c.question_id == question_id select c;
            Random random = new Random(Guid.NewGuid().GetHashCode());


            foreach (var an in answer)
            {
                PublicClass.Answer temanswer = new PublicClass.Answer();
                temanswer.answer_id = an.answer_id;
                temanswer.isright = int.Parse(an.is_right);
                newanswer.Add(temanswer);
            }
            return newanswer;
        }

        //考试随机抽题目
        private void random_question()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            //判断题
            int question_pd_count = (from c in jiakaoDataSet.question where c.question_type.Contains("PD") select c).Count();
            var question_pd = from c in jiakaoDataSet.question where c.question_type.Contains("PD") select c;
            int question_count = question_pd_count;
            if (question_count > 40)
            {
                question_count = 40;
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
                        question.sz = true;
                        question.rept_do = 0;
                        //question.answer = random_answer(qu.question_id);
                        PublicClass.question_list.Add(question);
                        int tem_list_count = (from c in PublicClass.question_list where c.question_id == qu.question_id select c).Count();
                        if (tem_list_count > 1)
                        {
                            PublicClass.question_list.Remove(question);
                            i--;
                        }
                        break;

                    }
                    eachcount++;
                }
            }
            quesiton_p = question_pd_count;//获取判断题总数

            //选择题
            int question_xz_count = (from c in jiakaoDataSet.question where c.question_type.Contains("XZ") select c).Count();
            var question_xz = from c in jiakaoDataSet.question where c.question_type.Contains("XZ") select c;
            question_count = question_xz_count;
            if (question_count > 60)
            {
                question_count = 60;
            }
            for (int i = 0; i < question_count; i++)
            {

                Random random = new Random(Guid.NewGuid().GetHashCode());
                int ran = random.Next(0, question_xz_count);
                int eachcount = 0;
                foreach (var qu in question_xz)
                {
                    if (eachcount == ran)
                    {
                        PublicClass.Question question = new PublicClass.Question();
                        question.question_id = qu.question_id;
                        question.check_answer = true;
                        question.select_answer = "";
                        question.question_type = qu.question_type;
                        question.sz = true;
                        question.rept_do = 0;
                        question.answer = random_answer(qu.question_id);
                        PublicClass.question_list.Add(question);
                        int tem_list_count = (from c in PublicClass.question_list where c.question_id == qu.question_id select c).Count();
                        if (tem_list_count > 1)
                        {
                            PublicClass.question_list.Remove(question);
                            i--;
                        }
                        break;

                    }
                    eachcount++;
                }
            }
            question_x = question_xz_count;//获取选择题总数


            //question_c = question_pd_count + question_xz_count;//根据抽出题数生成题号
            question_c = 100;

        }

        //生成题号
        private void create_question_num()
        {
            Dispatcher.Invoke(new Action(() =>
{
    //dati_canvas.Children.Clear()

    for (int i = 0; i < dati_canvas.Children.Count; i++)
    {
        QuestionNum delqu = dati_canvas.Children[i] as QuestionNum;
        if (delqu != null)
        {
            dati_canvas.UnregisterName(delqu.Name);
            dati_canvas.Children.Remove(delqu);
            i--;
        }
    }

    int cou = question_c;
    for (int i = 0; i < cou; i++)
    {
        int x = i / 10;
        int y = i % 10;
        x *= 30;
        y *= 24;
        QuestionNum qu = new QuestionNum();

        qu.Margin = new Thickness(y, x, 0, 0);
        qu.label1.Content = i + 1;
        if (PublicClass.question_list[i].shownum > -1)
        {
            qu.labelshow.Content = PublicClass.question_list[i].shownum.ToString();
            qu.setnum(i + 1, false, PublicClass.question_list[i].select_answer.ToString());
            //qu.label2.Content = PublicClass.question_list[i].select_answer.ToString();
            //label2.Foreground = Brushes.Red;
        }
        else
        {
            qu.labelshow.Content = qu.label1.Content;
            //qu.label2.Content = "";
            qu.setnum(i + 1, true, "");
        }
        qu.Name = "q" + i.ToString();
        qu.MouseDown += new MouseButtonEventHandler(OK);

        dati_canvas.Children.Add(qu);
        dati_canvas.RegisterName("q" + i, qu);
    }
    if (cou % 10 > 0)
        dati_canvas.Height = (cou / 10 + 1) * 30;
    else
        dati_canvas.Height = cou / 10 * 30;

}));
        }

        //初始化第一题
        private void questionindex()
        {
            Dispatcher.Invoke(new Action(() =>
{
    //DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
    //// 将数据加载到表 question 中。可以根据需要修改此代码。
    //DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
    //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

    //// 将数据加载到表 answer 中。可以根据需要修改此代码。
    //DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
    //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

    int question_index = 0;
    if (PublicClass.question_index == -1)
    {
        question_index = 0;
    }
    else
    {
        question_index = PublicClass.question_index;
    }
    var question = from c in PublicClass.question_data where c.question_id == PublicClass.question_list[question_index].question_id select c;
    foreach (var temqu in question)
    {
        //timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;//显示题目
        timu_xaml(question_index + 1 + "." + temqu.question_name);
        imagename = temqu.question_image; //获取题目对应图片文件名
        question_image();//显示图片
        break;
    }


    int step = 0;
    if (!PublicClass.question_list[question_index].question_type.Contains("PD"))
    {
        foreach (var an in PublicClass.question_list[question_index].answer)
        {
            PublicClass.Answer myan = an as PublicClass.Answer;
            var teman = from c in PublicClass.answer_data where c.answer_id == myan.answer_id select c;
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
    else
    {
        duicuo();
    }

    //foreach (var q in dati_canvas.Children)
    //{
    //    QuestionNum temnum = q as QuestionNum;
    //    if (temnum != null && temnum.Name == "q0")
    //    {
    //        temnum.setbackcolor();
    //    }
    //}

    string q = "q" + question_index;

    QuestionNum temnum = dati_canvas.FindName(q) as QuestionNum;
    if (temnum != null)
    {
        temnum.setbackcolor();
    }


    if (PublicClass.question_list[question_index].question_type.Contains("PD"))
    {
        current_question_type = "P";
    }
    else
    {
        var answers = from c in PublicClass.question_list[question_index].answer select c;
        int isright_count = 0;
        foreach (var answer in answers)
        {
            if (answer.isright == 1)
            {
                isright_count++;
            }
        }
        if (isright_count > 1)
        {
            current_question_type = "M";
        }
        else
        {
            current_question_type = "S";
        }
    }

    showright_answer(question_index);
    play_voice(doc_text());
    cur_question_lab_index = 0;
}));

        }


        //显示正确答案
        private void showright_answer(int question_index)
        {
            try
            {
                zhengque_textBlock.Text = "";
                if (current_question_type == "S" || current_question_type == "M")
                {
                    if (PublicClass.question_list[question_index].answer[0].isright == 1)
                    {
                        zhengque_textBlock.Text += 'A';
                    }
                    else if (PublicClass.question_list[question_index].answer[1].isright == 1)
                    {
                        zhengque_textBlock.Text += 'B';
                    }
                    else if (PublicClass.question_list[question_index].answer[2].isright == 1)
                    {
                        zhengque_textBlock.Text += 'C';
                    }
                    else if (PublicClass.question_list[question_index].answer[3].isright == 1)
                    {
                        zhengque_textBlock.Text += 'D';
                    }
                }
                else
                {
                    if (PublicClass.question_list[question_index].question_type.Contains("XD"))
                    {
                        zhengque_textBlock.Text = "√";
                    }
                    if (PublicClass.question_list[question_index].question_type.Contains("XC"))
                    {
                        zhengque_textBlock.Text = "×";
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Substring(0, 6) == "索引超出范围")
                {
                    MessageBox.Show("正确答案数据异常");
                }
            }


        }

        //判断题选项
        private void duicuo()
        {
            if (!check_error)
            {
                //a_button.Content = "√";
                //b_button.Content = "×";
                //c_button.Visibility = System.Windows.Visibility.Hidden;
                //d_button.Visibility = System.Windows.Visibility.Hidden;

                a_button.Visibility = System.Windows.Visibility.Hidden;
                b_button.Visibility = System.Windows.Visibility.Hidden;
                c_button.Content = "√";
                d_button.Content = "×";

            }
        }
        //选择题选项
        private void abcd()
        {
            if (!check_error)
            {
                //a_button.Content = "A";
                //b_button.Content = "B";
                //c_button.Visibility = System.Windows.Visibility.Visible;
                //d_button.Visibility = System.Windows.Visibility.Visible;

                a_button.Visibility = System.Windows.Visibility.Visible;
                b_button.Visibility = System.Windows.Visibility.Visible;
                c_button.Content = "C";
                d_button.Content = "D";

            }

        }

        //提取图片并显示
        private void question_image()
        {
            if (imagename != "")
            {
                try
                {
                    string path = System.Windows.Forms.Application.StartupPath + "\\Image\\" + imagename;

                    FileInfo fd = new FileInfo(path);
                    int Length = (int)fd.Length;
                    if (Length > 0)
                    {
                        gif_image.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                    }
                    else
                    {
                        gif_image.Source = null;
                    }
                }
                catch
                {
                    MessageBox.Show("图片损坏或被不存在,请重启软件并更新!", "提示");
                }
            }
            else
            {
                gif_image.Source = null;
            }
        }



        //题号单击事件
        void OK(object sender, MouseButtonEventArgs e)
        {
            //DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            //// 将数据加载到表 question 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            //// 将数据加载到表 answer 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            //int last_index = 0;
            //foreach (var i in dati_canvas.Children)
            //{
            //    QuestionNum myqu = i as QuestionNum;
            //    if (myqu != null)
            //    {
            //        if (myqu.canvas1.Background == Brushes.SkyBlue)
            //        {
            //            last_index = int.Parse(myqu.Name.Substring(1, myqu.Name.Length - 1));
            //        }
            //        myqu.canvas1.Background = Brushes.White;
            //    }


            //}
            last_question_lab_index = cur_question_lab_index;
            QuestionNum oldqu = dati_canvas.FindName("q" + last_question_lab_index) as QuestionNum;
            if (oldqu != null)
            {
                oldqu.canvas1.Background = Brushes.White;
            }
            QuestionNum qu = sender as QuestionNum;
            qu.setbackcolor();
            cur_question_lab_index = int.Parse(qu.label1.Content.ToString()) - 1;


            int question_index = cur_question_lab_index;
            lab_index = question_index;
            var question = from c in PublicClass.question_data where c.question_id == PublicClass.question_list[question_index].question_id select c;
            foreach (var temqu in question)
            {
                if (PublicClass.user_id != -2)//用户已登录
                {
                    //timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;//显示题目
                    timu_xaml(question_index + 1 + "." + temqu.question_name);
                    imagename = temqu.question_image; //获取题目对应图片文件名
                    question_image();//显示图片
                }
                else//没有用户登录为试用10题
                {
                    if (question_index < 10)//前10题正常显示
                    {
                        //timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;//显示题目
                        timu_xaml(question_index + 1 + "." + temqu.question_name);
                        imagename = temqu.question_image; //获取题目对应图片文件名
                        question_image();//显示图片
                    }
                    else//超出10题不予显示
                    {
                        //timu_textBlock.Text = "未注册用户,只能练习前10题,注册后可以练习全部试题,无任何限制!";
                        timu_xaml("未注册用户,只能练习前10题,注册后可以练习全部试题,无任何限制!");
                        register re = new register();
                        re.Show();
                        re.Topmost = true;
                    }

                }
                break;
            }

            //if (PublicClass.question_mode == 1)//考试下做题后不可修改            
            //{
            if (PublicClass.question_list[question_index].rept_do != 0)
            {
                a_button.IsEnabled = false;
                b_button.IsEnabled = false;
                c_button.IsEnabled = false;
                d_button.IsEnabled = false;
            }
            else
            {
                a_button.IsEnabled = true;
                b_button.IsEnabled = true;
                c_button.IsEnabled = true;
                d_button.IsEnabled = true;
            }
            //}

            process_question_type(question_index);//判断所选题型
            if (current_question_type == "S" || current_question_type == "M")
            {
                tishi_label.Content = "选择题,请在备选答案中选择您认为正确的答案!";
            }
            else
            {
                tishi_label.Content = "判断题,请在备选答案中选择您认为正确的答案!";
            }



            int step = 0;
            if (PublicClass.user_id != -2)//用户已登录
            {
                if (!PublicClass.question_list[question_index].question_type.Contains("PD"))
                {
                    foreach (var an in PublicClass.question_list[question_index].answer)
                    {
                        PublicClass.Answer myan = an as PublicClass.Answer;
                        var teman = from c in PublicClass.answer_data where c.answer_id == myan.answer_id select c;
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
                    abcd();
                }
                else
                {
                    xuanxiang_textBlock1.Text = "";
                    xuanxiang_textBlock2.Text = "";
                    xuanxiang_textBlock3.Text = "";
                    xuanxiang_textBlock4.Text = "";
                    duicuo();
                }


                ThreadPool.QueueUserWorkItem(judge_answer, "");
                ThreadPool.QueueUserWorkItem(answer_UI, "");
                //judge_answer();
                //answer_UI();
                if (!check_error)
                {
                    shouzheng_cal(last_question_lab_index);
                }

                //continuetodo(last_question_lab_index);//贮存记录

                errquestion(last_question_lab_index);
                //play_voice(timu_textBlock.Text);

                showright_answer(cur_question_lab_index);//正确答案显示
                error_messages(cur_question_lab_index);//错题提示,此处必须在正确答案显示后面
                play_voice(doc_text());
            }
            else //没有用户登录为试用10题
            {

                if (question_index < 10)//前10题正常显示
                {
                    if (!PublicClass.question_list[question_index].question_type.Contains("PD"))
                    {
                        foreach (var an in PublicClass.question_list[question_index].answer)
                        {
                            PublicClass.Answer myan = an as PublicClass.Answer;
                            var teman = from c in PublicClass.answer_data where c.answer_id == myan.answer_id select c;
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
                        abcd();
                    }
                    else
                    {
                        xuanxiang_textBlock1.Text = "";
                        xuanxiang_textBlock2.Text = "";
                        xuanxiang_textBlock3.Text = "";
                        xuanxiang_textBlock4.Text = "";
                        duicuo();
                    }

                    ThreadPool.QueueUserWorkItem(judge_answer, "");
                    ThreadPool.QueueUserWorkItem(answer_UI, "");
                    //judge_answer();
                    //answer_UI();
                    if (!check_error)
                    {
                        ThreadPool.QueueUserWorkItem(shouzheng_cal, last_question_lab_index);
                    }
                    //shouzheng_cal(last_question_lab_index);
                    ThreadPool.QueueUserWorkItem(errquestion, last_question_lab_index);
                    //errquestion(last_question_lab_index);
                    //play_voice(timu_textBlock.Text);


                    //continuetodo(last_question_lab_index);//贮存记录

                    showright_answer(cur_question_lab_index);//正确答案显示
                    error_messages(cur_question_lab_index);//错题提示,此处必须在正确答案显示后面

                }
                else//超出10题不予显示
                {
                    abcd();
                    xuanxiang_textBlock1.Text = "";
                    xuanxiang_textBlock2.Text = "";
                    xuanxiang_textBlock3.Text = "";
                    xuanxiang_textBlock4.Text = "";
                }
            }

        }


        //上一题
        private void up_button_Click(object sender, RoutedEventArgs e)
        {
            //int question_id = 0;
            //foreach (var lab in dati_canvas.Children)
            //{
            //    QuestionNum qu = lab as QuestionNum;
            //    if (qu.canvas1.Background == Brushes.SkyBlue)
            //    {
            //        question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1)) - 1;
            //        if (question_id >= 0)
            //        {
            //            select_question(question_id);
            //        }
            //        break;
            //    }
            //}

            last_question_lab_index = cur_question_lab_index;
            if (cur_question_lab_index > 0)
            {
                cur_question_lab_index--;
            }
            ThreadPool.QueueUserWorkItem(select_question, cur_question_lab_index);

            //if (PublicClass.question_mode == 1)//考试下做题后不可修改            
            //{
            if (PublicClass.question_list[cur_question_lab_index].rept_do != 0)
            {
                a_button.IsEnabled = false;
                b_button.IsEnabled = false;
                c_button.IsEnabled = false;
                d_button.IsEnabled = false;
            }
            else
            {
                a_button.IsEnabled = true;
                b_button.IsEnabled = true;
                c_button.IsEnabled = true;
                d_button.IsEnabled = true;
            }
            //}

            if (cur_question_lab_index == 0)
            {
                MessageBoxResult result = MessageBox.Show("已是第一题");
            }
            //else
            //{
            process_question_type(cur_question_lab_index);
            //}
            if (current_question_type == "S" || current_question_type == "M")
            {
                tishi_label.Content = "选择题,请在备选答案中选择您认为正确的答案!";
            }
            else
            {
                tishi_label.Content = "判断题,请在备选答案中选择您认为正确的答案!";
            }

            if (PublicClass.user_id != -2 || cur_question_lab_index < 10)
            {
                if (cur_question_lab_index != 0)
                {
                    ThreadPool.QueueUserWorkItem(judge_answer, "");
                    ThreadPool.QueueUserWorkItem(answer_UI, "");
                    //judge_answer();
                    //answer_UI();
                    if (!check_error)
                    {
                        ThreadPool.QueueUserWorkItem(shouzheng_cal, cur_question_lab_index + 1);
                    }
                    //shouzheng_cal(cur_question_lab_index - 1);
                    ThreadPool.QueueUserWorkItem(errquestion, cur_question_lab_index + 1);
                    //errquestion(cur_question_lab_index - 1);
                    ThreadPool.QueueUserWorkItem(err_count, cur_question_lab_index + 1);
                    //play_voice(timu_textBlock.Text);


                    //continuetodo(cur_question_lab_index + 1);//贮存记录

                    error_messages(cur_question_lab_index + 1);//错题提示,必须在正确答案显示前面
                    if (cur_question_lab_index > -1)
                    {
                        showright_answer(cur_question_lab_index);//正确答案显示
                    }
                    //play_voice(doc_text());
                }
            }
            if (PublicClass.question_mode == 1)
            {

            }
            //xuanxiang_textBlock.Text = "";    
        }

        //下一题
        private void do_button_Click(object sender, RoutedEventArgs e)
        {

            //if (PublicClass.question_mode == 1)//考试下做题后不可修改            
            //{
            //    int question_index = 0;
            //    List<int> questionsid = new List<int>();

            //    if (question_index >= 0)
            //    {
            //        questionsid.Add(question_index);//储存所有做过题的下标
            //    }
            //}
            //DateTime t1 = DateTime.Now;
            //int question_id = 0;
            //foreach (var lab in dati_canvas.Children)
            //{
            //    QuestionNum qu = lab as QuestionNum;
            //    if (qu.canvas1.Background == Brushes.SkyBlue)
            //    {
            //        question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1)) + 1;
            //        if (question_id < question_c)
            //        {
            //            select_question(question_id);
            //        }
            //        break;
            //    }
            //}
 
            last_question_lab_index = cur_question_lab_index;
            if (cur_question_lab_index < PublicClass.question_list.Count() - 1)
            {
                cur_question_lab_index++;
            }
            ThreadPool.QueueUserWorkItem(select_question, cur_question_lab_index);

            //select_question(cur_question_lab_index);



            //if (PublicClass.question_mode == 1)//考试下做题后不可修改            
            //{
            if (PublicClass.question_list[cur_question_lab_index].rept_do != 0)
            {
                a_button.IsEnabled = false;
                b_button.IsEnabled = false;
                c_button.IsEnabled = false;
                d_button.IsEnabled = false;
            }
            else
            {
                a_button.IsEnabled = true;
                b_button.IsEnabled = true;
                c_button.IsEnabled = true;
                d_button.IsEnabled = true;
            }
            //}


            if (cur_question_lab_index == PublicClass.question_list.Count() - 1)
            {
                MessageBoxResult result = MessageBox.Show("已是最后一题");
            }
            //else
            //{

            process_question_type(cur_question_lab_index);
            //}
            if (current_question_type == "S" || current_question_type == "M")
            {
                tishi_label.Content = "选择题,请在备选答案中选择您认为正确的答案!";
            }
            else
            {
                tishi_label.Content = "判断题,请在备选答案中选择您认为正确的答案!";
            }


            if (PublicClass.user_id != -2 || cur_question_lab_index < 10)
            {
                if (cur_question_lab_index != PublicClass.question_list.Count() - 1)
                {
                    ThreadPool.QueueUserWorkItem(judge_answer, "");
                    //judge_answer();
                    ThreadPool.QueueUserWorkItem(answer_UI, "");
                    //answer_UI();
                    if (!check_error)
                    {
                        ThreadPool.QueueUserWorkItem(shouzheng_cal, cur_question_lab_index - 1);
                    }
                    //shouzheng_cal(cur_question_lab_index - 1);
                    ThreadPool.QueueUserWorkItem(errquestion, cur_question_lab_index - 1);
                    //errquestion(cur_question_lab_index - 1);
                    ThreadPool.QueueUserWorkItem(err_count, cur_question_lab_index - 1);
                    //err_count(cur_question_lab_index - 1);
                    //play_voice(timu_textBlock.Text);


                    //continuetodo(cur_question_lab_index - 1);//贮存记录

                    error_messages(cur_question_lab_index - 1);//错题提示,必须在正确答案显示前面
                    if (cur_question_lab_index < question_c)
                    {
                        showright_answer(cur_question_lab_index);//正确答案显示
                    }
   
                }
            }

            //last_question_lab_index = cur_question_lab_index;

            //DateTime t2 = DateTime.Now;
            //MessageBox.Show((t2 - t1).ToString());
            //xuanxiang_textBlock.Text = "";

        }

        //下次继续做题储存信息
        private void continuetodo(int index)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.recordTableAdapter jiakaoDataSetrecordTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.recordTableAdapter();
            jiakaoDataSetrecordTableAdapter.Fill(jiakaoDataSet.record);

            if (PublicClass.create_method == 0 && PublicClass.question_mode != 1 && PublicClass.user_id != -2 && index != 0 && cur_question_lab_index != PublicClass.question_list.Count() - 1)
            {
                if (PublicClass.classflag == "章节练习")
                {
                    PublicClass.classtype = "";
                }

                var re = from c in jiakaoDataSet.record where c.user_id == PublicClass.user_id select c;
                if (re.Count() != 0)//更新
                {
                    foreach (var r in re)
                    {
                        r.subject = PublicClass.subjection;
                        r.driverlicense = PublicClass.cartype;
                        r._class = PublicClass.classflag;
                        r.class_flag = PublicClass.classtype;
                        r.chapter = PublicClass.listBox_index;
                        r.question_index = index;
                    }
                }
                else//创建
                {
                    jiakaoDataSet.record.AddrecordRow(PublicClass.user_id, PublicClass.subjection, PublicClass.cartype, PublicClass.classflag, PublicClass.classtype, PublicClass.listBox_index, index);

                }

                jiakaoDataSetrecordTableAdapter.Update(jiakaoDataSet.record);
                jiakaoDataSet.record.AcceptChanges();
            }
            else
            {
                var re = from c in jiakaoDataSet.record where c.user_id == PublicClass.user_id select c;
                foreach (var r in re)
                {
                    jiakaoDataSet.record.FindByID(r.ID).Delete();
                }

                jiakaoDataSetrecordTableAdapter.Update(jiakaoDataSet.record);
                jiakaoDataSet.record.AcceptChanges();
            }


        }

        //做错次数显示
        private void err_count(object data)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    int question_id = PublicClass.question_list[int.Parse(data.ToString())].question_id;//通过下标获取id

                    DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                    DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
                    lock (jiakaoDataSet)
                    {
                        jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);
                        var myerr = from c in jiakaoDataSet.errquest where question_id == c.question_id select c;
                        if (myerr.Count() > 0)
                        {
                            errcount.Text = myerr.First().amount.ToString();
                        }
                        else
                        {
                            errcount.Text = "0";
                        }
                    }
                }));

        }

        //做错时提示
        private void error_messages(int question_id)
        {
            if (timer_type == "考试")
            {
                if (PublicClass.question_list[question_id].check_answer == false)
                {
                    PublicClass.err_questionid = question_id;
                    PublicClass.question_answer = zhengque_textBlock.Text;

                    ErrorMessages err = new ErrorMessages();
                    C1.WPF.C1Window c1w = new C1.WPF.C1Window();
                    c1w.Content = err;
                    c1w.ShowModal();
                    c1w.Name = "做错";
                    c1w.Header = "提示";
                    c1w.ShowMinimizeButton = false;
                    c1w.ShowMaximizeButton = false;
                    c1w.Margin = PublicClass.window_thickness(err);
                }
            }
        }

        //计算首正
        private void shouzheng_cal(object data)
        {
            Dispatcher.Invoke(new Action(() =>
{
    int question_id = int.Parse(data.ToString());
    if (PublicClass.question_list[question_id].select_answer != "")
    {
        if (PublicClass.question_list[question_id].rept_do == 0 && PublicClass.question_list[question_id].check_answer == false)
        {
            PublicClass.question_list[question_id].sz = false;
        }
        PublicClass.question_list[question_id].rept_do += 1;
        int question_sz_count = (from c in PublicClass.question_list where c.sz == true select c).Count();
        shouzheng.Text = ((int)(((float)question_sz_count / (float)question_c) * 100)).ToString();

        int dadui_count = (from c in PublicClass.question_list where c.check_answer == true && c.rept_do > 0 select c).Count();
        dadui.Text = dadui_count.ToString();

        int dacuo_count = (from c in PublicClass.question_list where c.check_answer == false select c).Count();
        dacuo.Text = dacuo_count.ToString();

        int yida_count = (from c in PublicClass.question_list where c.rept_do > 0 select c).Count();
        dati_precent.Text = ((int)(((float)dadui_count / (float)yida_count) * 100)).ToString();

        chouti_precent.Text = ((int)(((float)dadui_count / (float)question_c) * 100)).ToString();
        PublicClass.fenshu = int.Parse(chouti_precent.Text);

        int weida_count = (from c in PublicClass.question_list where c.rept_do == 0 select c).Count();
        weida.Text = weida_count.ToString();
    }
}));
        }

        //判断所选题型
        private void process_question_type(int question_id)
        {
            //DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            //// 将数据加载到表 question 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            //// 将数据加载到表 answer 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
            if (PublicClass.question_list[question_id].question_type.Contains("PD"))
            {
                current_question_type = "P";
            }
            else if (PublicClass.question_list[question_id].question_type.Contains("XZ"))
            {
                var an = from c in PublicClass.question_list[question_id].answer where c.isright == 1 select c;
                if (an.Count() > 1)
                {
                    current_question_type = "M";
                }
                else
                {
                    current_question_type = "S";
                }
            }
        }

        //判断对错
        private void judge_answer(object data)
        {
            Dispatcher.Invoke(new Action(() =>
          {

              QuestionNum mylab = dati_canvas.FindName("q" + last_question_lab_index) as QuestionNum;
              if (mylab != null)
              {
                  mylab.check_answer(PublicClass.question_list[last_question_lab_index].check_answer);
              }
          }));


        }

        //储存错题
        private void errquestion(object data)
        {
            int id = int.Parse(data.ToString());
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            lock (jiakaoDataSet)
            {
                DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
                jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

                DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
                jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

                if (PublicClass.question_list[id].check_answer == false && is_click_flag)
                {
                    var err = from c in jiakaoDataSet.errquest where c.question_id == PublicClass.question_list[id].question_id && c.user_id == PublicClass.user_id select c;
                    if (err.Count() > 0)
                    {
                        err.First().amount += 1;
                    }
                    else
                    {
                        jiakaoDataSeterrquestTableAdapter.Insert(PublicClass.user_id, PublicClass.question_list[id].question_id, 1);
                    }
                    jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                    jiakaoDataSet.errquest.AcceptChanges();
                }
                else if (PublicClass.question_list[id].check_answer == true && PublicClass.delerr)
                {
                    var delquestion = from c in jiakaoDataSet.errquest where c.question_id == PublicClass.question_list[id].question_id && c.user_id == PublicClass.user_id select c;
                    if (delquestion.Count() != 0)
                    {
                        delquestion.First().Delete();
                    }
                    jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                    jiakaoDataSet.errquest.AcceptChanges();

                }
            }
            is_click_flag = false;
        }

        //生成题目和答案
        private void select_question(object data)
        {
            int question_id = int.Parse(data.ToString());
            //DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            //// 将数据加载到表 question 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            //// 将数据加载到表 answer 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            //foreach (var i in dati_canvas.Children)
            //{
            //    QuestionNum myqu = i as QuestionNum;
            //    if (myqu != null)
            //    {
            //        myqu.canvas1.Background = Brushes.White;
            //    }
            //    if (myqu.Name.ToString().Substring(1, myqu.Name.ToString().Length - 1) == question_id.ToString())
            //    {
            //        myqu.setbackcolor();
            //    }


            //}
            //if (last_question_lab_index != cur_question_lab_index)
            //{
            Dispatcher.Invoke(new Action(() =>
                   {
                       QuestionNum lastqu = dati_canvas.FindName("q" + last_question_lab_index) as QuestionNum;
                       if (lastqu != null)
                       {
                           lastqu.canvas1.Background = Brushes.White;
                       }
                       QuestionNum curqu = dati_canvas.FindName("q" + cur_question_lab_index) as QuestionNum;
                       if (curqu != null)
                       {
                           curqu.setbackcolor();
                       }

                       //}

                       //QuestionNum qu = sender as QuestionNum;
                       //qu.setbackcolor();

                       //int question_index = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1));
                       //lab_index = question_index;


                       var question = from c in PublicClass.question_data where c.question_id == PublicClass.question_list[question_id].question_id select c;
                       foreach (var temqu in question)
                       {
                           if (PublicClass.user_id != -2)//用户已登录
                           {
                               //timu_textBlock.Text = question_id + 1 + "." + temqu.question_name;//显示题目
                               timu_xaml(question_id + 1 + "." + temqu.question_name);
                               imagename = temqu.question_image; //获取题目对应图片文件名
                               question_image();//显示图片
                           }
                           else//没有用户登录为试用10题
                           {
                               if (question_id < 10)//前10题正常显示
                               {
                                   //timu_textBlock.Text = question_id + 1 + "." + temqu.question_name;//显示题目
                                   timu_xaml(question_id + 1 + "." + temqu.question_name);
                                   imagename = temqu.question_image; //获取题目对应图片文件名
                                   question_image();//显示图片
                               }
                               else//超出10题不予显示
                               {
                                   //timu_textBlock.Text = "未注册用户,只能练习前10题,注册后可以练习全部试题,无任何限制!";
                                   timu_xaml("未注册用户,只能练习前10题,注册后可以练习全部试题,无任何限制!");
                                   register re = new register();
                                   re.Show();
                                   re.Topmost = true;
                               }
                           }
                           break;
                       }

                       int step = 0;

                       if (PublicClass.user_id != -2)//用户已登录
                       {
                           if (!PublicClass.question_list[question_id].question_type.Contains("PD"))
                           {
                               foreach (var an in PublicClass.question_list[question_id].answer)
                               {
                                   PublicClass.Answer myan = an as PublicClass.Answer;
                                   var teman = from c in PublicClass.answer_data where c.answer_id == myan.answer_id select c;
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
                               abcd();
                           }
                           else
                           {
                               xuanxiang_textBlock1.Text = "";
                               xuanxiang_textBlock2.Text = "";
                               xuanxiang_textBlock3.Text = "";
                               xuanxiang_textBlock4.Text = "";
                               duicuo();
                           }
                           play_voice(doc_text());
                       }
                       else //没有用户登录为试用10题
                       {

                           if (question_id < 10)//前10题正常显示
                           {
                               if (!PublicClass.question_list[question_id].question_type.Contains("PD"))
                               {
                                   foreach (var an in PublicClass.question_list[question_id].answer)
                                   {
                                       PublicClass.Answer myan = an as PublicClass.Answer;
                                       var teman = from c in PublicClass.answer_data where c.answer_id == myan.answer_id select c;
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
                                   abcd();
                               }
                               else
                               {
                                   xuanxiang_textBlock1.Text = "";
                                   xuanxiang_textBlock2.Text = "";
                                   xuanxiang_textBlock3.Text = "";
                                   xuanxiang_textBlock4.Text = "";
                                   duicuo();
                               }

                           }
                           else//超出10题不予显示
                           {
                               abcd();
                               xuanxiang_textBlock1.Text = "";
                               xuanxiang_textBlock2.Text = "";
                               xuanxiang_textBlock3.Text = "";
                               xuanxiang_textBlock4.Text = "";
                           }
                       }
                   }));
        }

        //处理所选答案在UI的显示
        private void answer_UI(object data)
        {
            Dispatcher.Invoke(new Action(() =>
                    {
                        a_button.IsChecked = false;
                        b_button.IsChecked = false;
                        c_button.IsChecked = false;
                        d_button.IsChecked = false;
                        xuanxiang_textBlock.Text = "";

                        QuestionNum mylab = dati_canvas.FindName("q" + cur_question_lab_index) as QuestionNum;
                        if (mylab != null)
                        {
                            if (mylab.label2.Content.ToString().Contains("A"))
                            {
                                a_button.IsChecked = true;
                                xuanxiang_textBlock.Text += "A";
                            }
                            if (mylab.label2.Content.ToString().Contains("B"))
                            {
                                b_button.IsChecked = true;
                                xuanxiang_textBlock.Text += "B";
                            }
                            if (mylab.label2.Content.ToString().Contains("C"))
                            {
                                c_button.IsChecked = true;
                                xuanxiang_textBlock.Text += "C";
                            }
                            if (mylab.label2.Content.ToString().Contains("D"))
                            {
                                d_button.IsChecked = true;
                                xuanxiang_textBlock.Text += "D";
                            }
                            if (mylab.label2.Content.ToString().Contains("√"))
                            {
                                c_button.IsChecked = true;
                                xuanxiang_textBlock.Text += "√";
                            }
                            if (mylab.label2.Content.ToString().Contains("×"))
                            {
                                d_button.IsChecked = true;
                                xuanxiang_textBlock.Text += "×";
                            }
                        }
                    }));


        }

        //语音朗读
        private void play_voice(string playtxt)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            lock (jiakaoDataSet)
            {
                jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

                var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
                foreach (var s in set)
                {
                    if (s.phonetic_reading != 2)//启用
                    {
                        playvoice = true;
                        if (s.phonetic_reading == 0)//女声
                        {
                            synth.SpeakAsyncCancelAll();
                            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
                            synth.SpeakAsync(playtxt);
                        }
                        else//男声
                        {
                            synth.SpeakAsyncCancelAll();
                            synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
                            synth.SpeakAsync(playtxt);
                        }
                    }
                    else//不启用
                    { }
                }
            }
        }

        //选项
        private void xuanxiang_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            //// 将数据加载到表 question 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            //jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            //// 将数据加载到表 answer 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            //jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            //// 将数据加载到表 setting 中。可以根据需要修改此代码。
            //DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            //jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);


            var question_index = 0;
            string select_lab = "";
            //QuestionNum selectquestionnum = new QuestionNum();
            //foreach (var q_index in dati_canvas.Children)
            //{
            //    QuestionNum mylab = q_index as QuestionNum;
            //    if (mylab != null)
            //    {
            //        if (mylab.canvas1.Background == Brushes.SkyBlue)
            //        {
            //            question_index = int.Parse(mylab.Name.ToString().Substring(1, mylab.Name.ToString().Length - 1));
            //            selectquestionnum = q_index as QuestionNum;
            //        }
            //    }
            //    ////
            //}

            question_index = cur_question_lab_index;
            QuestionNum selectquestionnum = dati_canvas.FindName("q" + cur_question_lab_index) as QuestionNum;
            //int question_id = 0;

            //continuetodo(question_index + 1);//贮存记录

            //process_question_type(question_index);
            //选择题选项
            if (current_question_type == "S")
            {
                a_button.IsChecked = false;
                b_button.IsChecked = false;
                c_button.IsChecked = false;
                d_button.IsChecked = false;

                ToggleButton select_button = sender as ToggleButton;
                select_button.IsChecked = true;
                select_lab = select_button.Name.ToString().Substring(0, 1).ToUpper();
                xuanxiang_textBlock.Text = select_lab;
                PublicClass.question_list[question_index].select_answer = select_lab;
                selectquestionnum.setnum(int.Parse(selectquestionnum.label1.Content.ToString()), true, select_lab);
                selectquestionnum.setbackcolor();
                int step = 0;
                bool isright = false;
                foreach (var ans in PublicClass.question_list[question_index].answer)
                {
                    if (ans.isright == 1)
                    {
                        if (select_lab == "A" && step == 0)
                        {
                            isright = true;
                            //play_voice("正确答案 A");
                        }
                        else if (select_lab == "B" && step == 1)
                        {
                            isright = true;
                            //play_voice("正确答案 B");
                        }
                        else if (select_lab == "C" && step == 2)
                        {
                            isright = true;
                            //play_voice("正确答案 C");
                        }
                        else if (select_lab == "D" && step == 3)
                        {
                            isright = true;
                            //play_voice("正确答案 D");
                        }
                    }
                    step++;
                }

                PublicClass.question_list[question_index].check_answer = isright;


            }



            //判断题选项
            else if (current_question_type == "P")
            {
                a_button.IsChecked = false;
                b_button.IsChecked = false;

                ToggleButton select_button = sender as ToggleButton;
                select_button.IsChecked = true;
                select_lab = select_button.Content.ToString();
                xuanxiang_textBlock.Text = select_lab;
                PublicClass.question_list[question_index].select_answer = select_lab;
                selectquestionnum.setnum(int.Parse(selectquestionnum.label1.Content.ToString()), true, select_lab);
                selectquestionnum.setbackcolor();


                bool q;
                var ques = from c in PublicClass.question_data where c.question_id == PublicClass.question_list[question_index].question_id select c;
                if (ques.Last().is_judge == 1 && select_lab == "√")
                {
                    q = true;
                }
                else if (ques.Last().is_judge == 0 && select_lab == "×")
                {
                    q = true;
                }
                else
                {
                    q = false;
                }

                PublicClass.question_list[question_index].check_answer = q;
            }
            else if (current_question_type == "M")
            {
                a_button.IsChecked = false;
                b_button.IsChecked = false;
                c_button.IsChecked = false;
                d_button.IsChecked = false;

                ToggleButton select_button = sender as ToggleButton;
                select_button.IsChecked = true;
                if (a_button.IsChecked == true)
                {
                    xuanxiang_textBlock.Text += "A";
                }
                if (b_button.IsChecked == true)
                {
                    xuanxiang_textBlock.Text += "B";
                    // select_lab += "B";
                }
                if (c_button.IsChecked == true)
                {
                    xuanxiang_textBlock.Text += "C";
                    //select_lab += "C";
                }
                if (d_button.IsChecked == true)
                {
                    xuanxiang_textBlock.Text += "D";
                    //select_lab += "D";
                }
                //select_lab = select_button.Name.ToString().Substring(0, 1).ToUpper();
                //xuanxiang_textBlock.Text = select_lab;
                string tem_select = "";
                if (xuanxiang_textBlock.Text.Contains("A"))
                {
                    tem_select += "A";
                }
                if (xuanxiang_textBlock.Text.Contains("B"))
                {
                    tem_select += "B";
                }
                if (xuanxiang_textBlock.Text.Contains("C"))
                {
                    tem_select += "C";
                }
                if (xuanxiang_textBlock.Text.Contains("D"))
                {
                    tem_select += "D";
                }
                select_lab = tem_select;
                PublicClass.question_list[question_index].select_answer = select_lab;
                selectquestionnum.setnum(int.Parse(selectquestionnum.label1.Content.ToString()), true, select_lab);
                selectquestionnum.setbackcolor();
                bool is_right = true;
                int step = 0;
                var an = from c in PublicClass.question_list[question_index].answer select c;
                foreach (var myan in an)
                {
                    if (!(step == 0 && myan.isright == 1 && select_lab.Contains("A")))
                    {
                        is_right = false;
                    }
                    else if (!(step == 1 && myan.isright == 1 && select_lab.Contains("B")))
                    {
                        is_right = false;
                    }
                    else if (!(step == 2 && myan.isright == 1 && select_lab.Contains("C")))
                    {
                        is_right = false;
                    }
                    else if (!(step == 3 && myan.isright == 1 && select_lab.Contains("D")))
                    {
                        is_right = false;
                    }
                    step++;
                }


                PublicClass.question_list[question_index].check_answer = is_right;






            }




            if (playvoice)//启用
            {
                if (zhengque_textBlock.Text == "√")
                {
                    play_voice("正确答案 选对");
                }
                else if (zhengque_textBlock.Text == "×")
                {
                    play_voice("正确答案 选错");
                }
                else
                {
                    play_voice("正确答案 选" + zhengque_textBlock.Text);
                }
            }




            xuanxiang_textBlock.Text = PublicClass.question_list[question_index].select_answer;
            is_click_flag = true;

            string fanti = "";

            if (timer_type == "练习")
            {
                var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
                foreach (var s in set)
                {



                    if (s.next_question == 1)//选择答案后自动下一题
                    {
                        fanti = "自动翻题";
                    }
                    else if (s.next_question == 2)//答对后自动翻题
                    {
                        if (PublicClass.question_list[question_index].check_answer)
                        {
                            fanti = "自动翻题";
                        }
                    }
                }
                if (fanti == "自动翻题")
                {
                    do_button_Click(null, null);
                }
            }



        }

        //字体大小
        private void xiao_button_Click(object sender, RoutedEventArgs e)//变小
        {
            if (doc_reader.Zoom > 90)
            {
                //timu_textBlock.FontSize -= 2;
                doc_reader.Zoom -= 10;
                xuanxiang_textBlock1.FontSize -= 2;
                xuanxiang_textBlock2.FontSize -= 2;
                xuanxiang_textBlock3.FontSize -= 2;
                xuanxiang_textBlock4.FontSize -= 2;

                if (doc_reader.Zoom != 90)
                {
                    if (doc_reader.Zoom != 160)
                    {
                        da_button.IsEnabled = true;
                    }
                }
                else
                {
                    xiao_button.IsEnabled = false;
                    da_button.IsEnabled = true;
                }
            }
            else if (doc_reader.Zoom == 90)
            {
                xiao_button.IsEnabled = false;
                da_button.IsEnabled = true;
            }
            //else if (timu_textBlock.FontSize < 24)
            //{
            //    da_button.IsEnabled = true;
            //}
        }

        private void da_button_Click(object sender, RoutedEventArgs e)//变大
        {
            if (doc_reader.Zoom < 160)
            {
                doc_reader.Zoom += 10;
                xuanxiang_textBlock1.FontSize += 2;
                xuanxiang_textBlock2.FontSize += 2;
                xuanxiang_textBlock3.FontSize += 2;
                xuanxiang_textBlock4.FontSize += 2;

                if (doc_reader.Zoom != 160)
                {
                    if (doc_reader.Zoom != 90)
                    {
                        xiao_button.IsEnabled = true;
                    }
                }
                else
                {
                    xiao_button.IsEnabled = true;
                    da_button.IsEnabled = false;
                }
            }
            else if (doc_reader.Zoom == 160)
            {
                xiao_button.IsEnabled = true;
                da_button.IsEnabled = false;
            }
            //else if (timu_textBlock.FontSize > 14)
            //{
            //    xiao_button.IsEnabled = true;
            //}
        }

        //字体颜色
        private void ziti_c1ColorPicker_SelectedColorChanged(object sender, C1.WPF.PropertyChangedEventArgs<Color> e)
        {
            //timu_textBlock.Foreground = new SolidColorBrush(ziti_c1ColorPicker.SelectedColor);
        }

        //交卷
        private void jiaojuan_button_Click(object sender, RoutedEventArgs e)
        {
            if (PublicClass.question_mode == 1)//考试下交卷        
            {
                Assignment ass = new Assignment();
                C1.WPF.C1Window c1w = new C1.WPF.C1Window();
                ass.kaoshicishu = kaoshicishu;
                c1w.Content = ass;
                c1w.ShowModal();
                c1w.Name = "交卷";
                c1w.Header = "提示";
                c1w.Margin = PublicClass.window_thickness(ass);
            }
            else//练习下交卷
            {
                MessageBoxResult result = MessageBox.Show("尚有" + weida.Text + "道题未答,确定交卷吗？", "询问", MessageBoxButton.OKCancel);

                //确定
                if (result == MessageBoxResult.OK)
                {
                    DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                    // 将数据加载到表 question 中。可以根据需要修改此代码。
                    DrivingTest.jiakaoDataSetTableAdapters.recordTableAdapter jiakaoDataSetrecordTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.recordTableAdapter();
                    jiakaoDataSetrecordTableAdapter.Fill(jiakaoDataSet.record);

                    var re = from c in jiakaoDataSet.record where c.user_id == PublicClass.user_id select c;
                    foreach (var r in re)
                    {
                        jiakaoDataSet.record.FindByID(r.ID).Delete();
                    }

                    jiakaoDataSetrecordTableAdapter.Update(jiakaoDataSet.record);
                    jiakaoDataSet.record.AcceptChanges();

                    string xueyuanName = name_textBlock.Text;
                    string xueyuanMark = chouti_precent.Text;

                    EndTest en = new EndTest(xueyuanName, xueyuanMark);
                    C1.WPF.C1Window c1 = new C1.WPF.C1Window();
                    c1.Name = "end";
                    c1.Margin = PublicClass.window_thickness(en);
                    c1.Content = en;
                    c1.ShowModal();
                }

                //取消
                if (result == MessageBoxResult.Cancel)
                {

                }

            }
        }

        //手动添加错题
        private void tianjiacuoti_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

            int question_id = 0;
            foreach (var lab in dati_canvas.Children)
            {
                QuestionNum qu = lab as QuestionNum;
                if (qu.canvas1.Background == Brushes.SkyBlue)
                {
                    question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1));
                    if (question_id >= 0)
                    {
                        //errquestion(question_id);

                        var err = from c in jiakaoDataSet.errquest where c.question_id == PublicClass.question_list[question_id].question_id && c.user_id == PublicClass.user_id select c;
                        if (err.Count() > 0)
                        {
                            err.First().amount += 1;
                        }
                        else
                        {
                            jiakaoDataSeterrquestTableAdapter.Insert(PublicClass.user_id, PublicClass.question_list[question_id].question_id, 1);
                        }
                        jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                        jiakaoDataSet.errquest.AcceptChanges();
                    }
                    MessageBox.Show("添加到错题成功!", "提示");
                }
            }
        }

        //手动删除错题
        private void shanchucuoti_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);


            int question_id = 0;
            int id = 0;
            int errid = 0;
            foreach (var lab in dati_canvas.Children)
            {
                QuestionNum qu = lab as QuestionNum;
                if (qu.canvas1.Background == Brushes.SkyBlue)
                {
                    question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1));
                    if (question_id >= 0)
                    {
                        var question = from c in jiakaoDataSet.question where c.question_id == PublicClass.question_list[question_id].question_id select c;
                        foreach (var temqu in question)
                        {
                            id = temqu.question_id; //获取需删除题目ID
                        }
                        var questionerr = from c in jiakaoDataSet.errquest where c.question_id == id && c.user_id == PublicClass.user_id select c;
                        foreach (var errqu in questionerr)
                        {
                            errid = errqu.errquest_id;
                        }

                        jiakaoDataSet.errquest.FindByerrquest_id(errid).Delete();//删除错题


                        jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                        jiakaoDataSet.errquest.AcceptChanges();
                    }
                    MessageBox.Show("从错题中删除成功!", "提示");
                }
            }

        }

        //查看错题
        public void show_err_question(int mark)
        {
            if (mark == 100)//满分,没有错题
            {
                check_error = false;

                zhengque_label.Visibility = System.Windows.Visibility.Visible;
                zhengque_textBlock.Visibility = System.Windows.Visibility.Visible;
                a_button.Visibility = System.Windows.Visibility.Hidden;
                b_button.Visibility = System.Windows.Visibility.Hidden;
                c_button.Visibility = System.Windows.Visibility.Hidden;
                d_button.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                question_c = PublicClass.question_list.Count();
                create_question_num();
                questionindex();
                check_error = true;

                zhengque_label.Visibility = System.Windows.Visibility.Visible;
                zhengque_textBlock.Visibility = System.Windows.Visibility.Visible;
                a_button.Visibility = System.Windows.Visibility.Hidden;
                b_button.Visibility = System.Windows.Visibility.Hidden;
                c_button.Visibility = System.Windows.Visibility.Hidden;
                d_button.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //重新考试
        private void chongkao_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定重新考试吗？", "询问", MessageBoxButton.OKCancel);

            //确定
            if (result == MessageBoxResult.OK)
            {
                chongkao_button.Visibility = System.Windows.Visibility.Hidden;
                zongfen_TextBlock.Visibility = System.Windows.Visibility.Hidden;
                jiaojuan_button.Visibility = System.Windows.Visibility.Visible;
                dadui.Text = "0";
                dacuo.Text = "0";
                shouzheng.Text = "0";
                dati_precent.Text = "0";
                chouti_precent.Text = "0";

                if (PublicClass.question_mode == 0)//练习下显示重考错题
                {
                    DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
                    // 将数据加载到表 question 中。可以根据需要修改此代码。
                    DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
                    jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

                    int question_id = 0;
                    List<int> questionsid = new List<int>();
                    foreach (var lab in dati_canvas.Children)
                    {
                        QuestionNum qu = lab as QuestionNum;
                        if (qu.label2.Foreground == Brushes.Black && qu.label2.Content.ToString() != "")
                        {
                            question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1));
                            if (question_id >= 0)
                            {
                                var question = from c in jiakaoDataSet.question where c.question_id == PublicClass.question_list[question_id].question_id select c;
                                foreach (var temqu in question)
                                {
                                    questionsid.Add(temqu.question_id); //获取需删除题目ID
                                }
                            }
                        }
                    }
                    for (int i = 0; i < questionsid.Count(); i++)
                    {
                        for (int j = 0; j < PublicClass.questions_id.Count; j++)
                        {
                            if (questionsid[i] == PublicClass.questions_id[j])
                            {
                                PublicClass.questions_id.RemoveAt(j);
                                j--;
                            }
                        }
                    }

                    PublicClass.question_list = new List<PublicClass.Question>();
                    create_question(PublicClass.create_method, PublicClass.question_mode, PublicClass.cartype, PublicClass.subject, PublicClass.questions_id);//重新执行抽题
                    Window_Loaded(null, null);//重新执行界面
                    weida.Text = chouti_count.Text;

                    check_error = false;
                    zhengque_label.Visibility = System.Windows.Visibility.Hidden;
                    zhengque_textBlock.Visibility = System.Windows.Visibility.Hidden;
                    a_button.Visibility = System.Windows.Visibility.Visible;
                    b_button.Visibility = System.Windows.Visibility.Visible;
                    c_button.Visibility = System.Windows.Visibility.Visible;
                    d_button.Visibility = System.Windows.Visibility.Visible;
                    xuanxiang_textBlock.Text = "";
                    questionindex();
                }
                else//考试
                {
                    PublicClass.question_list = new List<PublicClass.Question>();
                    create_question(PublicClass.create_method, PublicClass.question_mode, PublicClass.cartype, PublicClass.subject, PublicClass.questions_id);//重新执行抽题
                    Window_Loaded(null, null);//重新执行界面
                    kaoshicishu = kaoshicishu + 1;
                    weida.Text = chouti_count.Text;
                }
            }

            //取消
            if (result == MessageBoxResult.Cancel)
            {

            }

        }


        private void gif_image_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = ((MediaElement)sender).Position.Add(TimeSpan.FromMilliseconds(1));
            //(sender as MediaElement).Stop();
            //(sender as MediaElement).Play();  
        }


        //显示答案单击事件
        private void xianshi_question()
        {
            //    DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            //    // 将数据加载到表 setting 中。可以根据需要修改此代码。
            //    DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            //    jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            //    var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            //    foreach (var s in set)
            //    {
            //        if (xianshi_checkBox.IsChecked == true)
            //        {
            //            zhengque_label.Visibility = System.Windows.Visibility.Visible;
            //            zhengque_textBlock.Visibility = System.Windows.Visibility.Visible;
            //        }
            //        else
            //        {
            //            zhengque_label.Visibility = System.Windows.Visibility.Hidden;
            //            zhengque_textBlock.Visibility = System.Windows.Visibility.Hidden;
            //        }
            //    }
        }

        //显示考试科目和车型
        private void cart_sub(string cartpye, string subject)
        {
            if (cartpye == "C1")
            {
                cart_textBlock.Text = "C1C2C3C4";
            }
            if (cartpye == "A1")
            {
                cart_textBlock.Text = "A1A3B1";
            }
            if (cartpye == "A2")
            {
                cart_textBlock.Text = "A2B2";
            }
            if (cartpye == "D")
            {
                cart_textBlock.Text = "DEF";
            }
            if (cartpye == "HF")
            {
                cart_textBlock.Text = "恢复驾考";
            }
            if (subject == "科目一")
            {
                sub_textBlock.Text = "科目一";
            }
            if (subject == "科目二")
            {
                sub_textBlock.Text = "科目二";
            }
            if (subject == "科目三")
            {
                sub_textBlock.Text = "科目三";
            }
            if (subject == "科目四")
            {
                sub_textBlock.Text = "科目四";
            }
        }


        //判断是否显示正确答案
        private void display_answers()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var set = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            foreach (var s in set)
            {
                if (s.display_answers == 1)
                {
                    zhengque_label.Visibility = System.Windows.Visibility.Visible;
                    zhengque_textBlock.Visibility = System.Windows.Visibility.Visible;
                }
                else
                { }
            }
        }

        //快捷键注册
        private void register_key()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 setting 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter jiakaoDataSetsettingTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.settingTableAdapter();
            jiakaoDataSetsettingTableAdapter.Fill(jiakaoDataSet.setting);

            var key = from c in jiakaoDataSet.setting where c.setting_id == 1 select c;
            foreach (var k in key)
            {
                PublicClass.key = k.shortcut_key.Split(',');

                KeyConverter ke = new KeyConverter();

                Key mykey = (Key)ke.ConvertFromString(PublicClass.key[0]);
                key_ButtonA.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[1]);
                key_ButtonB.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[2]);
                key_ButtonC.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[3]);
                key_ButtonD.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[4]);
                Key_ButtonYes.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[5]);
                Key_ButtonNo.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[6]);
                key_ButtonUpOne.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[7]);
                key_ButtonNextOne.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[8]);
                key_ButtonFirstOne.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[9]);
                key_ButtonLastOne.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[10]);
                key_ButtonHandExams.Key = mykey;
                mykey = (Key)ke.ConvertFromString(PublicClass.key[11]);
                key_ButtonConfirmHandExams.Key = mykey;

            }
        }

        //点击查看原图
        private void gif_image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            int question_id = 0;
            foreach (var lab in dati_canvas.Children)
            {
                QuestionNum qu = lab as QuestionNum;
                if (qu.canvas1.Background == Brushes.SkyBlue)
                {
                    question_id = int.Parse(qu.Name.ToString().Substring(1, qu.Name.ToString().Length - 1));
                    if (question_id >= 0)
                    {
                        var question = from c in jiakaoDataSet.question where c.question_id == PublicClass.question_list[question_id].question_id select c;
                        foreach (var temqu in question)
                        {
                            PublicClass.question_image = temqu.question_image; //获取题目对应图片文件名

                            OriginalImages or = new OriginalImages();
                            C1.WPF.C1Window co = new C1.WPF.C1Window();
                            co.Name = "查看原图";
                            //co.ToolTip = "查看全图";
                            co.ShowMinimizeButton = false;
                            co.ShowMaximizeButton = false;
                            co.Margin = PublicClass.window_thickness(or);
                            co.Content = or;
                            co.ShowModal();
                            //co.Focus();
                            //co.IsActive = true;

                        }
                    }
                }
            }
        }

        //快捷键触发事件
        private void CommandBinding_ButtonA_Executed(object sender, ExecutedRoutedEventArgs e)//A
        {
            xuanxiang_button_Click(a_button, null);
        }
        private void CommandBinding_ButtonB_Executed(object sender, ExecutedRoutedEventArgs e)//B
        {
            xuanxiang_button_Click(b_button, null);
        }
        private void CommandBinding_ButtonC_Executed(object sender, ExecutedRoutedEventArgs e)//C
        {
            xuanxiang_button_Click(c_button, null);
        }
        private void CommandBinding_ButtonD_Executed(object sender, ExecutedRoutedEventArgs e)//D
        {
            xuanxiang_button_Click(d_button, null);
        }
        private void CommandBinding_ButtonYes_Executed(object sender, ExecutedRoutedEventArgs e)//对
        {
            xuanxiang_button_Click(a_button, null);
        }
        private void CommandBinding_ButtonNo_Executed(object sender, ExecutedRoutedEventArgs e)//错
        {
            xuanxiang_button_Click(b_button, null);
        }
        private void CommandBinding_ButtonUpOne_Executed(object sender, ExecutedRoutedEventArgs e)//上一题
        {
            up_button_Click(up_button, null);
        }
        private void CommandBinding_ButtonNextOne_Executed(object sender, ExecutedRoutedEventArgs e)//下一题
        {
            do_button_Click(do_button, null);
        }
        private void CommandBinding_ButtonFirstOne_Executed(object sender, ExecutedRoutedEventArgs e)//第一题
        {
            questionindex();
        }
        private void CommandBinding_ButtonLastOne_Executed(object sender, ExecutedRoutedEventArgs e)//最后一题
        {
            select_question(question_c - 1);
        }
        private void CommandBinding_ButtonHandExams_Executed(object sender, ExecutedRoutedEventArgs e)//交卷
        {
            jiaojuan_button_Click(null, null);
        }
        private void CommandBinding_ButtonConfirmHandExams_Executed(object sender, ExecutedRoutedEventArgs e)//确认交卷
        {

        }

        private void timu_xaml(string timu_text)
        {
            SautinSoft.HtmlToRtf h = new SautinSoft.HtmlToRtf();
            string rtfstr = h.ConvertString(timu_text);
            rtfstr = rtfstr.Substring(0, rtfstr.LastIndexOf("________________________________________________________") - 1);
            //doc_reader.Document = (FlowDocument)rtfstr;

            Byte[] bytes = Encoding.UTF8.GetBytes(rtfstr);
            MemoryStream memoryStream = new MemoryStream(bytes);
            XmlTextReader xmlTextReader = new XmlTextReader(memoryStream);
            FlowDocument doc = new FlowDocument();
            var content = new TextRange(doc.ContentStart, doc.ContentEnd);
            content.Load(memoryStream, DataFormats.Rtf);
            doc_reader.Document = doc;
        }

        private string doc_text()
        {
            TextRange flowDocSelection = new TextRange(doc_reader.Document.ContentStart, doc_reader.Document.ContentEnd);
            string vocestr = flowDocSelection.Text;
            if (vocestr == null)
            {
                vocestr = "";
            }
            return flowDocSelection.Text;
        }



        #region 计时器
        /// <summary>
        /// 处理倒计时的委托
        /// </summary>
        /// <returns></returns>
        public delegate bool CountDownHandler();
        /// <summary>
        /// Timer触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {

            DateTime nowtime = DateTime.Parse(SecondArea.Text);
            if (timer_type == "考试")
            {
                nowtime = nowtime.Subtract(TimeSpan.FromSeconds(1));
                if (SecondArea.Text == "00:00:00")
                {
                    timer.Stop();

                    Assignment ass = new Assignment();
                    C1.WPF.C1Window c1w = new C1.WPF.C1Window();
                    c1w.Content = ass;
                    c1w.ShowModal();
                    c1w.Name = "交卷";
                    c1w.Header = "提示";
                    ass.kaoshicishu = kaoshicishu;
                    c1w.Margin = PublicClass.window_thickness(ass);
                    ass.queren_button_Click(null, null);
                    c1w.IsActive = true;
                }
                else
                {
                    SecondArea.Text = nowtime.ToLongTimeString();
                }
            }
            else
            {
                nowtime = nowtime.AddSeconds(1);

                SecondArea.Text = nowtime.ToLongTimeString();
            }

        }

        void en_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.Close();
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
        #endregion
}







