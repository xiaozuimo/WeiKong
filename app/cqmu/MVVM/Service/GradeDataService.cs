using cqmu.MVVM.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cqmu.MVVM.Service
{
    public class GradeDataService
    {
        public static ObservableCollection<Course> ParseGradeDataAsync(string gradeJson)
        {
            try
            {
                ObservableCollection<Course> gradeList = new ObservableCollection<Course>();
                JObject json = JObject.Parse(gradeJson);

                for (int i = 0; i < json["TranscriptData"].Count(); i++)
                {
                    Course course = new Course();

                    course.CourseCode = json["TranscriptData"][i]["CourseCode"].ToString();
                    course.FullName = json["TranscriptData"][i]["Name"].ToString();
                    course.Classify = json["TranscriptData"][i]["Classify"].ToString();
                    course.Credits = json["TranscriptData"][i]["Credits"].ToString();
                    course.GradePoint = json["TranscriptData"][i]["GradePoint"].ToString();
                    course.RegularGrade = json["TranscriptData"][i]["RegularGrade"].ToString();
                    course.MidtermGrade = json["TranscriptData"][i]["MidtermGrade"].ToString();
                    course.FinalexamGrade = json["TranscriptData"][i]["FinalexamGrade"].ToString();
                    course.ExperimentGrade = json["TranscriptData"][i]["ExperimentGrade"].ToString();
                    course.TotalMark = json["TranscriptData"][i]["TotalMark"].ToString();
                    course.MinorMark = json["TranscriptData"][i]["MinorMark"].ToString();
                    course.MakeupTotalGrade = json["TranscriptData"][i]["MakeupTotalGrade"].ToString();
                    course.MakeupGrade = json["TranscriptData"][i]["MakeupGrade"].ToString();
                    course.RetakeGrade = json["TranscriptData"][i]["RetakeGrade"].ToString();
                    course.Academy = json["TranscriptData"][i]["Academy"].ToString();

                    gradeList.Add(course);
                }

                return gradeList;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }
    }
}
