using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static int end = 0;
        public static string http { get; set; }

        public static string login = "";//登录用户名
        public static int user_id = -1;//登录用户ID
        public static int fenshu = 0;//考试总分数

        public static string cartype = "";//车型
        public static string subjection = "";//科目
        public static bool delerr = false;//是否删除错题
        public static string shezhi = "";//设置或退出
    }
}
