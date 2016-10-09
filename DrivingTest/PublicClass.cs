﻿using System;
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
    }
}
