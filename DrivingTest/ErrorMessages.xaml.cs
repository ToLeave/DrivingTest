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
using System.IO;

namespace DrivingTest
{
    /// <summary>
    /// ErrorMessages.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorMessages : UserControl
    {
        public ErrorMessages()
        {
            InitializeComponent();
        }

        
        string imagename = "";//图片文件名

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            DrivingTest.jiakaoDataSet jiakaoDataSet = ((DrivingTest.jiakaoDataSet)(this.FindResource("jiakaoDataSet")));
            // 将数据加载到表 question 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter jiakaoDataSetquestionTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.questionTableAdapter();
            jiakaoDataSetquestionTableAdapter.Fill(jiakaoDataSet.question);

            // 将数据加载到表 answer 中。可以根据需要修改此代码。
            DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter jiakaoDataSetanswerTableAdapter = new DrivingTest.jiakaoDataSetTableAdapters.answerTableAdapter();
            jiakaoDataSetanswerTableAdapter.Fill(jiakaoDataSet.answer);



            var question = from c in jiakaoDataSet.question where c.question_id == PublicClass.question_list[PublicClass.err_questionid].question_id select c;
            foreach (var temqu in question)
            {
                textBlock1.Text = PublicClass.err_questionid + 1 + "." + temqu.question_name;//显示题目
                imagename = temqu.question_image; //获取题目对应图片文件名
                question_image();//显示图片 

                break;
            }

            int step = 0;
            if (!PublicClass.question_list[PublicClass.err_questionid].question_type.Contains("PD"))
            {
                foreach (var an in PublicClass.question_list[PublicClass.err_questionid].answer)
                {
                    PublicClass.Answer myan = an as PublicClass.Answer;
                    var teman = from c in jiakaoDataSet.answer where c.answer_id == myan.answer_id select c;
                    foreach (var temann in teman)
                    {
                        switch (step)
                        {
                            case 0:
                                textBlock2.Text = "A." + temann.answer;
                                break;
                            case 1:
                                textBlock3.Text = "B." + temann.answer;
                                break;
                            case 2:
                                textBlock4.Text = "C." + temann.answer;
                                break;
                            case 3:
                                textBlock5.Text = "D." + temann.answer;
                                break;
                        }

                    }

                    step++;
                }
            }
            else
            {
                textBlock2.Text = "";
                textBlock3.Text = "";
                textBlock4.Text = "";
                textBlock5.Text = "";
            }
            textBlock6.Text = "正确答案为:" + PublicClass.question_answer;


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
                        image1.Source = new Uri(path, UriKind.Absolute);
                    }
                    else
                    {
                        image1.Source = null;
                    }
                }
                catch
                {
                    MessageBox.Show("图片损坏或被不存在,请重启软件并更新!", "提示");
                }
            }
            else
            {
                image1.Source = null;
            }
        }

        private void image_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = ((MediaElement)sender).Position.Add(TimeSpan.FromMilliseconds(1));
        }

        private void jixu_button_Click(object sender, RoutedEventArgs e)
        {
            C1.WPF.C1Window cp = MainWindow.FindChild<C1.WPF.C1Window>(Application.Current.MainWindow, "做错");
            if (cp != null)
            {
                cp.Close();
            }
        }
    }
}
