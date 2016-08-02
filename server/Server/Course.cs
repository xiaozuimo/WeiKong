using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server
{
    public class Course
    {
        /// <summary>
        /// 选课课号
        /// </summary>
        public string CourseNum { get; set; }

        /// <summary>
        /// 课程代码
        /// </summary>
        public string CourseCode { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// 课程性质
        /// </summary>
        public string Classify { get; set; }

        /// <summary>
        /// 学分
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// 周学时
        /// </summary>
        public string WeeklyHours { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string LessonTime { get; set; }

        /// <summary>
        /// 上课地点
        /// </summary>
        public string LessonVenue { get; set; }

        /// <summary>
        /// 教材
        /// </summary>
        public string Textbook { get; set; }

        /// <summary>
        /// 是否选课
        /// </summary>
        public string IsSelected { get; set; }

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

    }
}