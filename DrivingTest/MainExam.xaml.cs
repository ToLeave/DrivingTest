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



namespace DrivingTest
{
    /// <summary>
    /// MainExam.xaml 的交互逻辑
    /// </summary>
    public partial class MainExam : UserControl
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private ProcessCount processCount;

        List<string> question_pd_list = new List<string>();//随机题号列表
        List<PublicClass.Question> question_list = new List<PublicClass.Question>();

        int question_c = 0;//总题数
        int question_x = 0;//选择题总题数
        int quesiton_p = 0;//判断题总题数
        int quesiton_d = 0;//多选题总题数
        int lab_index = 0;
        bool is_click_flag = false;//选答案判断
        string timer_type = "";
        SpeechSynthesizer synth = new SpeechSynthesizer();
        // Configure the audio output. 
        string current_question_type = "S";//S=单选 M=多选 P=判断

        string imagename = "";//图片文件名

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
            this.Name = "mainW";
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);



            //random_question();//随机抽题

            create_question_num();//生成题号

            questionindex();//初始化第一题
            chouti_count.Text = question_c.ToString();
            weida.Text = question_c.ToString();

            // Configure the audio output. 
            synth.SetOutputToDefaultAudioDevice();
            //synth.SelectVoiceByHints(VoiceGender.Male);
            // Speak a string.
            synth.Volume = 100;
            synth.Rate = 0;


            #region 启动定时器

            //设置定时器
            
            timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
            timer.Tick += new EventHandler(timer_Tick);



            //转换成秒数
            //Int32 hour = Convert.ToInt32(HourArea.Text);
            //Int32 minute = Convert.ToInt32(MinuteArea.Text);
            //Int32 second = Convert.ToInt32(SecondArea.Text);

            //处理倒计时的类
            //processCount = new ProcessCount(hour * 3600 + minute * 60 + second);
            //CountDown += new CountDownHandler(processCount.ProcessCountDown);


           
            #endregion
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

        
        //生成题库
        public void create_question(int create_method, int question_mode, string cartype, string subject, List<int> questions_id)// cerate_method 0 顺序,1随机; question_mode 0 练习,1考试,2错题; cartype 车型;subject 科目; questions_id 题库ID
        {
            cart_sub(cartype, subject);//显示车型科目

            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);
            DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter jiakaoDataSetsubjectTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.subjectTableAdapter();
            jiakaoDataSetsubjectTableAdapter.Fill(jiakaoDataSet.subject);

            List<PublicClass.Question> question_pd_list = new List<PublicClass.Question>();
            List<PublicClass.Question> question_xz_list = new List<PublicClass.Question>();
            var local_subject = from c in jiakaoDataSet.subject where c.subject.Contains(subject) select c;


            if (question_mode == 1)//考试
            {
                shezhi_grid.Visibility = System.Windows.Visibility.Hidden;
                xianshi_grid.Visibility = System.Windows.Visibility.Hidden;
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
                question_pd_list = random_question(question_pd_list, 40);

                var question_xz = from c in jiakaoDataSet.question where c.driverlicense_type.Contains(cartype) && c.question_type.Contains("XZ") && c.subject_id == local_subject.First().subject_id select c;

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
                question_xz_list = random_question(question_xz_list, 60);

                for (int i = 0; i < question_pd_list.Count(); i++)
                {
                    question_list.Add(question_pd_list[i]);
                }
                for (int i = 0; i < question_xz_list.Count(); i++)
                {
                    question_list.Add(question_xz_list[i]);
                    question_list.Last().answer = random_answer(question_list.Last().question_id);
                }

            }
            else //练习
            {
                timer_type = "练习";
                SecondArea.Text = "00:00:00";
                if (create_method == 0)
                {
                    for (int i = 0; i < questions_id.Count(); i++)
                    {
                        var question = from c in jiakaoDataSet.question where c.question_id == questions_id[i] select c;
                        PublicClass.Question local_question = new PublicClass.Question();
                        local_question.question_id = question.First().question_id;
                        local_question.check_answer = true;
                        local_question.select_answer = "";
                        local_question.question_type = question.First().question_type;
                        local_question.sz = true;
                        local_question.rept_do = 0;
                        local_question.answer = random_answer(question.First().question_id);
                        question_list.Add(local_question);
                    }
                }
                else
                {
                    for (int i = 0; i < questions_id.Count(); i++)
                    {
                        var question = from c in jiakaoDataSet.question where c.question_id == questions_id[i] select c;
                        PublicClass.Question local_question = new PublicClass.Question();
                        local_question.question_id = question.First().question_id;
                        local_question.check_answer = true;
                        local_question.select_answer = "";
                        local_question.question_type = question.First().question_type;
                        local_question.sz = true;
                        local_question.rept_do = 0;
                        local_question.answer = random_answer(question.First().question_id);
                        question_list.Add(local_question);
                    }
                    question_list = random_question(question_list, questions_id.Count());
                }

            }
            question_c = question_list.Count();

            //开启定时器
            timer.Start();
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
                if (temcount > 10)
                {
                    i = 10;
                }
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
            }
            question_x = question_xz_count;//获取选择题总数


            //question_c = question_pd_count + question_xz_count;//根据抽出题数生成题号
            question_c = 100;

        }



        //生成题号
        private void create_question_num()
        {
            int cou = question_c;
            for (int i = 0; i < cou; i++)
            {
                int x = i / 10;
                int y = i % 10;
                x *= 36;
                y *= 31;
                QuestionNum qu = new QuestionNum();

                qu.Margin = new Thickness(y, x, 0, 0);
                qu.label1.Content = i + 1;
                qu.Name = "q" + i.ToString();
                qu.MouseDown += new MouseButtonEventHandler(OK);
                qu.setnum(i + 1, true, "");
                dati_canvas.Children.Add(qu);
            }
            dati_canvas.Height = cou / 10 * 36;
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
                timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;//显示题目
                imagename = temqu.question_image; //获取题目对应图片文件名
                question_image();//显示图片
                break;
            }

            int step = 0;
            if (!question_list[question_index].question_type.Contains("PD"))
            {
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
            else
            {
                duicuo();
            }

            foreach (var q in dati_canvas.Children)
            {
                QuestionNum temnum = q as QuestionNum;
                if (temnum != null && temnum.Name == "q0")
                {
                    temnum.setbackcolor();
                }
            }


            if (question_list[0].question_type.Contains("PD"))
            {
                current_question_type = "P";
            }
            else
            {
                var answers = from c in question_list[0].answer select c;
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

        }

        //显示正确答案
        private void showright_answer(int question_index)
        {
            zhengque_textBlock.Text = "";
            if (current_question_type == "S" || current_question_type == "M")
            {
                if (question_list[question_index].answer[0].isright == 1)
                {
                    zhengque_textBlock.Text += 'A';
                }
                if (question_list[question_index].answer[1].isright == 1)
                {
                    zhengque_textBlock.Text += 'B';
                }
                if (question_list[question_index].answer[2].isright == 1)
                {
                    zhengque_textBlock.Text += 'C';
                }
                if (question_list[question_index].answer[3].isright == 1)
                {
                    zhengque_textBlock.Text += 'D';
                }
            }
            else
            {
                if (question_list[question_index].question_type.Contains("XD"))
                {
                    zhengque_textBlock.Text = "√";
                }
                if (question_list[question_index].question_type.Contains("XC"))
                {
                    zhengque_textBlock.Text = "×";
                }
            }


        }

        //判断题选项
        private void duicuo()
        {
            a_button.Content = "√";
            b_button.Content = "×";
            c_button.Visibility = System.Windows.Visibility.Hidden;
            d_button.Visibility = System.Windows.Visibility.Hidden;
        }
        //选择题选项
        private void abcd()
        {
            a_button.Content = "A";
            b_button.Content = "B";
            c_button.Visibility = System.Windows.Visibility.Visible;
            d_button.Visibility = System.Windows.Visibility.Visible;

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
                        gif_image.Source = new Uri(path, UriKind.Absolute);
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
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            int last_index = 0;
            foreach (var i in dati_canvas.Children)
            {
                QuestionNum myqu = i as QuestionNum;
                if (myqu != null)
                {
                    if (myqu.canvas1.Background == Brushes.SkyBlue)
                    {
                        last_index = int.Parse(myqu.Name.Substring(1, myqu.Name.Length - 1));
                    }
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
                if (PublicClass.user_id != -1)//用户已登录
                {
                    timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;//显示题目
                    imagename = temqu.question_image; //获取题目对应图片文件名
                    question_image();//显示图片
                }
                else//没有用户登录为试用10题
                {
                    if (question_index < 10)//前10题正常显示
                    {
                        timu_textBlock.Text = question_index + 1 + "." + temqu.question_name;//显示题目
                        imagename = temqu.question_image; //获取题目对应图片文件名
                        question_image();//显示图片
                    }
                    else//超出10题不予显示
                    {
                        timu_textBlock.Text = "未注册用户,只能练习前10题,注册后可以练习全部试题,无任何限制!";
                    }
                    
                }
                break;
            }


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
            if (PublicClass.user_id != -1)//用户已登录
            {
                if (!question_list[question_index].question_type.Contains("PD"))
                {
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



                judge_answer();
                answer_UI();
                shouzheng_cal(last_index);
                errquestion(last_index);
                play_voice(timu_textBlock.Text);

                showright_answer(question_index);
            }
            else //没有用户登录为试用10题
            {
             
                    if (question_index < 10)//前10题正常显示
                    {
                        if (!question_list[question_index].question_type.Contains("PD"))
                        {
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

                        judge_answer();
                        answer_UI();
                        shouzheng_cal(last_index);
                        errquestion(last_index);
                        play_voice(timu_textBlock.Text);

                        showright_answer(question_index);

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
            process_question_type(question_id + 1);
            if (current_question_type == "S" || current_question_type == "M")
            {
                tishi_label.Content = "选择题,请在备选答案中选择您认为正确的答案!";
            }
            else
            {
                tishi_label.Content = "判断题,请在备选答案中选择您认为正确的答案!";
            }

            if (PublicClass.user_id != -1 || question_id < 10)
            {
                judge_answer();
                answer_UI();
                shouzheng_cal(question_id + 1);
                errquestion(question_id + 1);
                play_voice(timu_textBlock.Text);
                showright_answer(question_id);
            }

            //xuanxiang_textBlock.Text = "";
        }

        //下一题
        private void do_button_Click(object sender, RoutedEventArgs e)
        {

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
            process_question_type(question_id - 1);
            if (current_question_type == "S" || current_question_type == "M")
            {
                tishi_label.Content = "选择题,请在备选答案中选择您认为正确的答案!";
            }
            else
            {
                tishi_label.Content = "判断题,请在备选答案中选择您认为正确的答案!";
            }


            if (PublicClass.user_id != -1 || question_id < 10)
            {
                judge_answer();
                answer_UI();
                shouzheng_cal(question_id - 1);
                errquestion(question_id - 1);
                play_voice(timu_textBlock.Text);
                showright_answer(question_id);
            }

            //xuanxiang_textBlock.Text = "";


        }



        private void shouzheng_cal(int question_id)//计算首正
        {
            if (question_list[question_id].select_answer != "")
            {
                if (question_list[question_id].rept_do == 0 && question_list[question_id].check_answer == false)
                {
                    question_list[question_id].sz = false;
                }
                question_list[question_id].rept_do += 1;
                int question_sz_count = (from c in question_list where c.sz == true select c).Count();
                shouzheng.Text = ((int)(((float)question_sz_count / (float)question_c) * 100)).ToString();

                int dadui_count = (from c in question_list where c.check_answer == true && c.rept_do > 0 select c).Count();
                dadui.Text = dadui_count.ToString();

                int dacuo_count = (from c in question_list where c.check_answer == false select c).Count();
                dacuo.Text = dacuo_count.ToString();

                int yida_count = (from c in question_list where c.rept_do > 0 select c).Count();
                dati_precent.Text = ((int)(((float)dadui_count / (float)yida_count) * 100)).ToString();

                chouti_precent.Text = ((int)(((float)dadui_count / (float)question_c) * 100)).ToString();
                PublicClass.fenshu = int.Parse(chouti_precent.Text);

                int weida_count = (from c in question_list where c.rept_do == 0 select c).Count();
                weida.Text = weida_count.ToString();
            }
        }

        private void process_question_type(int question_id)//判断所选题型
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);
            if (question_list[question_id].question_type.Contains("PD"))
            {
                current_question_type = "P";
            }
            else if (question_list[question_id].question_type.Contains("XZ"))
            {

                var an = from c in question_list[question_id].answer where c.isright == 1 select c;
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
        private void judge_answer()
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            //DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            //jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

            //    var answer = from c in jiakaoDataSet.answer where c.question_id == question_list[question_id].question_id select c;
            foreach (var lab in dati_canvas.Children)
            {
                QuestionNum mylab = lab as QuestionNum;
                int question_index = int.Parse(mylab.Name.ToString().Substring(1, mylab.Name.ToString().Length - 1));
                mylab.check_answer(question_list[question_index].check_answer);
            }


        }

        //储存错题
        private void errquestion(int id)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter jiakaoDataSeterrquestTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.errquestTableAdapter();
            jiakaoDataSeterrquestTableAdapter.Fill(jiakaoDataSet.errquest);

            if (question_list[id].check_answer == false && is_click_flag)
            {
                var err = from c in jiakaoDataSet.errquest where c.question_id == question_list[id].question_id && c.user_id == PublicClass.user_id select c;
                if (err.Count() > 0)
                {
                    err.First().amount += 1;
                }
                else
                {
                    jiakaoDataSeterrquestTableAdapter.Insert(PublicClass.user_id, question_list[id].question_id, 1);
                }
                jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                jiakaoDataSet.errquest.AcceptChanges();
            }
            else if (question_list[id].check_answer == true && PublicClass.delerr)
            {
                var delquestion = from c in jiakaoDataSet.errquest where c.question_id == question_list[id].question_id && c.user_id == PublicClass.user_id select c;
                if (delquestion.Count() != 0)
                {
                    delquestion.First().Delete();
                }
                jiakaoDataSeterrquestTableAdapter.Update(jiakaoDataSet.errquest);
                jiakaoDataSet.errquest.AcceptChanges();

            }
            is_click_flag = false;
        }

        //生成题目和答案
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
                if (PublicClass.user_id != -1)//用户已登录
                {
                    timu_textBlock.Text = question_id + 1 + "." + temqu.question_name;//显示题目
                    imagename = temqu.question_image; //获取题目对应图片文件名
                    question_image();//显示图片
                }
                else//没有用户登录为试用10题
                {
                    if (question_id < 10)//前10题正常显示
                    {
                        timu_textBlock.Text = question_id + 1 + "." + temqu.question_name;//显示题目
                        imagename = temqu.question_image; //获取题目对应图片文件名
                        question_image();//显示图片
                    }
                    else//超出10题不予显示
                    {
                        timu_textBlock.Text = "未注册用户,只能练习前10题,注册后可以练习全部试题,无任何限制!";
                    }
                }
                break;
            }

            int step = 0;

            if (PublicClass.user_id != -1)//用户已登录
            {
                if (!question_list[question_id].question_type.Contains("PD"))
                {
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
            else //没有用户登录为试用10题
            {

                if (question_id < 10)//前10题正常显示
                {
                    if (!question_list[question_id].question_type.Contains("PD"))
                    {
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

        }


        private void answer_UI()//处理所选答案在UI的显示
        {
            a_button.IsChecked = false;
            b_button.IsChecked = false;
            c_button.IsChecked = false;
            d_button.IsChecked = false;
            xuanxiang_textBlock.Text = "";
            foreach (var lab in dati_canvas.Children)
            {

                QuestionNum mylab = lab as QuestionNum;
                if (mylab.canvas1.Background == Brushes.SkyBlue)
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
                        a_button.IsChecked = true;
                        xuanxiang_textBlock.Text += "√";
                    }
                    if (mylab.label2.Content.ToString().Contains("×"))
                    {
                        b_button.IsChecked = true;
                        xuanxiang_textBlock.Text += "×";
                    }
                }

            }
        }

        private void play_voice(string playtxt)
        {
            synth.SpeakAsyncCancelAll();
            synth.SpeakAsync(playtxt);

        }


        //选项
      //  bool is_right=false
        private void xuanxiang_button_Click(object sender, RoutedEventArgs e)
        {
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);

            var question_index = 0;
            string select_lab = "";
            QuestionNum selectquestionnum = new QuestionNum();
            foreach (var q_index in dati_canvas.Children)
            {
                QuestionNum mylab = q_index as QuestionNum;
                if (mylab != null)
                {
                    if (mylab.canvas1.Background == Brushes.SkyBlue)
                    {
                        question_index = int.Parse(mylab.Name.ToString().Substring(1, mylab.Name.ToString().Length - 1));
                        selectquestionnum = q_index as QuestionNum;
                    }
                }
                ////
            }
            //int question_id = 0;


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
                question_list[question_index].select_answer = select_lab;
                selectquestionnum.setnum(int.Parse(selectquestionnum.label1.Content.ToString()), true, select_lab);
                selectquestionnum.setbackcolor();
                int step = 0;
                bool isright = false;
                foreach (var ans in question_list[question_index].answer)
                {
                    if (ans.isright == 1)
                    {
                        if (select_lab == "A" && step == 0)
                        {
                            isright = true;
                        }
                        else if (select_lab == "B" && step == 1)
                        {
                            isright = true;
                        }
                        else if (select_lab == "C" && step == 2)
                        {
                            isright = true;
                        }
                        else if (select_lab == "D" && step == 3)
                        {
                            isright = true;
                        }
                    }
                    step++;
                }

                question_list[question_index].check_answer = isright;


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
                question_list[question_index].select_answer = select_lab;
                selectquestionnum.setnum(int.Parse(selectquestionnum.label1.Content.ToString()), true, select_lab);
                selectquestionnum.setbackcolor();


                bool q;
                var ques = from c in jiakaoDataSet.question where c.question_id == question_list[question_index].question_id select c;
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

                question_list[question_index].check_answer = q;
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
                question_list[question_index].select_answer = select_lab;
                selectquestionnum.setnum(int.Parse(selectquestionnum.label1.Content.ToString()), true, select_lab);
                selectquestionnum.setbackcolor();
                bool is_right = true;
                int step = 0;
                var an = from c in question_list[question_index].answer select c;
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


                question_list[question_index].check_answer = is_right;

            }

            xuanxiang_textBlock.Text = question_list[question_index].select_answer;
            is_click_flag = true;

            if (zidong_radioButton.IsChecked == true)//选择答案后自动下一题
            {
                do_button_Click(null, null);
            }
            else if (ddzidong_radioButton.IsChecked == true)
            {

                //int question_ind = 0;
                //foreach (var lab in dati_canvas.Children)
                //{
                //    QuestionNum mylab = lab as QuestionNum;
                //   question_ind  = int.Parse(mylab.Name.ToString().Substring(1, mylab.Name.ToString().Length - 1));
                //   // mylab.check_answer(question_list[question_ind].check_answer);
                //}
                if (question_list[question_index].check_answer)
                {
                    do_button_Click(null, null);
                }
               // int question_ind = int.Parse(mylab.Name.ToString().Substring(1, mylab.Name.ToString().Length - 1));



            }
          


        }

        //字体大小
        private void xiao_button_Click(object sender, RoutedEventArgs e)
        {
            if (timu_textBlock.FontSize > 14)
            {
                timu_textBlock.FontSize -= 2;
                xuanxiang_textBlock1.FontSize -= 2;
                xuanxiang_textBlock2.FontSize -= 2;
                xuanxiang_textBlock3.FontSize -= 2;
                xuanxiang_textBlock4.FontSize -= 2;
            }
            else if (timu_textBlock.FontSize == 14)
            {
                xiao_button.IsEnabled = false;
                da_button.IsEnabled = true;
            }
        }

        private void da_button_Click(object sender, RoutedEventArgs e)
        {
            if (timu_textBlock.FontSize < 24)
            {
                timu_textBlock.FontSize += 2;
                xuanxiang_textBlock1.FontSize += 2;
                xuanxiang_textBlock2.FontSize += 2;
                xuanxiang_textBlock3.FontSize += 2;
                xuanxiang_textBlock4.FontSize += 2;
            }
            else if (timu_textBlock.FontSize == 24)
            {
                xiao_button.IsEnabled = true;
                da_button.IsEnabled = false;
            }
        }

        //字体颜色
        //private void ziti_c1ColorPicker_SelectedColorChanged(object sender, C1.WPF.PropertyChangedEventArgs<Color> e)
        //{
        //    timu_textBlock.Foreground = new SolidColorBrush(ziti_c1ColorPicker.SelectedColor);
        //}


        //交卷
        private void jiaojuan_button_Click(object sender, RoutedEventArgs e)
        {
            Assignment ass = new Assignment();
            C1.WPF.C1Window c1w = new C1.WPF.C1Window();
            c1w.Content = ass;
            c1w.ShowModal();
            c1w.Name = "交卷";
            c1w.Header = "提示";
        }

        private void gif_image_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = ((MediaElement)sender).Position.Add(TimeSpan.FromMilliseconds(1));
            //(sender as MediaElement).Stop();
            //(sender as MediaElement).Play();  
        }



        //显示答案单击事件
        private void xianshi_checkBox_Click(object sender, RoutedEventArgs e)
        {
            if (xianshi_checkBox.IsChecked == true)
            {
                zhengque_label.Visibility = System.Windows.Visibility.Visible;
                zhengque_textBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                zhengque_label.Visibility = System.Windows.Visibility.Hidden;
                zhengque_textBlock.Visibility = System.Windows.Visibility.Hidden;
            }
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


        #region 计时器
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
                    //EndTest en = new EndTest();
                    //C1.WPF.C1Window c1 = new C1.WPF.C1Window();
                    //c1.Name = "end";
                    //c1.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - en.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - en.Height / 2, 0, 0);
                    //c1.Content = en;
                    //c1.ShowModal();

                    Assignment ass = new Assignment();
                    C1.WPF.C1Window c1w = new C1.WPF.C1Window();
                    c1w.Content = ass;
                    c1w.ShowModal();
                    c1w.Name = "交卷";
                    c1w.Header = "提示";

                    c1w.Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - ass.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - ass.Height / 2, 0, 0); ;
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

    /// <summary>
    /// 处理倒计时的委托
    /// </summary>
    /// <returns></returns>
    public delegate bool CountDownHandler();
        #endregion
}







