using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileUpdateService.Models
{
    public class Course
    {
        /// <summary>
        /// 课程名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 课程简称
        /// </summary>
        public string NameAbbreviation { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string Classroom { get; set; }

        /// <summary>
        /// 学分
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// 性质
        /// </summary>
        public string Classify { get; set; }

        /// <summary>
        /// 课程代码
        /// </summary>
        public string CourseCode { get; set; }

        /// <summary>
        /// 连堂节数
        /// </summary>
        public int CourseSpan { get; set; }

        /// <summary>
        /// 开课标记，用于标识该科是哪一节课
        /// </summary>
        public int StartMark { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 下课时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 绩点
        /// </summary>
        public string GradePoint { get; set; }

        /// <summary>
        /// 平时成绩
        /// </summary>
        public string RegularGrade { get; set; }

        /// <summary>
        /// 期中成绩
        /// </summary>
        public string MidtermGrade { get; set; }

        /// <summary>
        /// 期末成绩
        /// </summary>
        public string FinalexamGrade { get; set; }

        /// <summary>
        /// 实验成绩
        /// </summary>
        public string ExperimentGrade { get; set; }

        /// <summary>
        /// 总评成绩
        /// </summary>
        public string TotalMark { get; set; }

        /// <summary>
        /// 辅修标记
        /// </summary>
        public string MinorMark { get; set; }

        /// <summary>
        /// 补考成绩
        /// </summary>
        public string MakeupTotalGrade { get; set; }

        /// <summary>
        /// 卷面补考成绩
        /// </summary>
        public string MakeupGrade { get; set; }

        /// <summary>
        /// 重修成绩
        /// </summary>
        public string RetakeGrade { get; set; }

        /// <summary>
        /// 开课学院
        /// </summary>
        public string Academy { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        public string TermCode { get; set; }

        /// <summary>
        /// 用于显示的课程概览
        /// </summary>
        public string OverView
        {
            get
            {
                string s1 = string.IsNullOrEmpty(Classroom) ? "" : $"\n@{Classroom}";

                return FullName + s1;
            }
        }
    }
}
