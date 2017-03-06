using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace DrivingTest
{
    class PublicClass
    {
        public class Question
        {
            public int question_id { get; set; }
            public bool check_answer { get; set; }
            public string select_answer { get; set; }
            public string question_type { get; set; }
            public List<Answer> answer { get; set; }
            public bool sz { get; set; }//首正
            public int rept_do { get; set; }//重做次数
            public int shownum = -1;
        }
                public class Answer
        {
            public int answer_id { get; set; }
            public int isright { get; set; }
        }

        public static List<PublicClass.Question> question_list = new List<PublicClass.Question>();
        public static int end = 0;
        public static string http { get; set; }

        public static string login = "";//登录用户名
        public static int user_id = -1;//登录用户ID
        public static int fenshu = 0;//考试总分数

        public static string cartype = "";//车型
        public static string subjection = "";//科目
        public static bool delerr = false;//是否删除错题
        public static string shezhi = "";//设置或退出
        public static int err_questionid = 0;//做错了的题号下标
        public static string question_answer = "";//正确答案
        public static string question_image = "";//图片文件名
        public static bool wuwangluo = false;//无网络状态
        public static bool tuojizhuce = false;//脱机码注册状态

            

        public static DispatcherTimer timer = new DispatcherTimer();

        //重新抽题 create_method 0 顺序,1随机; question_mode 0 练习,1考试,2错题; cartype 车型;subject 科目; questions_id 题库IDlist
        public static int create_method = 0;
        public static int question_mode = 0;
        public static string subject = "";
        public static List<int> questions_id = new List<int>();

        public static string[] key = new string[12];//快捷键数组 索引从0开始依次为 A;B;C;D;对;错;上一题;下一题;第一题;最后题;交卷;确认交卷
        public static string[] gongneng = new string[6];//索引从0开始依次为 章节练习;顺序练习;随机练习;专项练习;模拟考试;错题强化; 0为启用不选中,1为禁用选中
        public static string[] xinxi = new string[5];//索引从0开始依次为 注册流程;付款链接;客服QQ;客服旺旺;备注; 0为启用不选中,1为禁用选中
        public static string[] yuyin = new string[3];//索引从0 开始依次为 提示语音讲解;语音提示对错;答错语音提示 0为不启用,1为启用



        public class questions //题目表
        {
            public int question_id;
            public int chapter_id;
            public int subject_id;
            public string question_name;
            public string question_image;
            public string voice;
            public string driverlicense_type;
            public string question_type;
            public string update_at;
            public int is_judge;
        }
        public static List<questions> question_data = new List<questions>();

        public class answers //答案表
        {
            public int answer_id;
            public int question_id;
            public string answer;
            public string is_right;
            public string update_at;
        }
        public static List<answers> answer_data = new List<answers>();

        public class chapters //章节表
        {
            public int chapter_id;
            public string chapter;
            public string updated_at;
        }
        public static List<chapters> chapter_data = new List<chapters>();

        public class subjects //科目表
        {
            public int subject_id;
            public string subject;
            public string updated_at;
        }
        public static List<subjects> subject_data = new List<subjects>();

        public class _class //分类
        {
            public int class_id;
            public string class_flag;
            public string question_type;
            public string name;
            public string driverlicense_type;
            public string subject;
        }
        public static List<_class> class_data = new List<_class>();

        public class classdetail //分类明细
        {
            public int classdetail_id;
            public int class_id;
            public int question_id;
        }
        public static List<classdetail> classdetail_data = new List<classdetail>();


        //窗口居中的四个边位置
        public static Thickness window_thickness(UserControl window)
        {
            Thickness thick = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - window.Width / 2, SystemParameters.PrimaryScreenHeight / 2 - window.Height / 2, 0, 0);
            return thick;
        }

        public class avatar//广告
        {
            public string avatarurl;
            public string link;
            public string avatar_type;
        }

        public static List<avatar> avatar_list = new List<avatar>();//广告list

    }
}
