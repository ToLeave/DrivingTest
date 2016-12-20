using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

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
    }
}
