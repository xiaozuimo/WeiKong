using cqmu.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace cqmu.MVVM.ViewModels
{
    public class GradeDetailPageViewModel
    {
        public List<Course> DesignTimeGradeList { get; set; } = new List<Course>();

        public static ObservableCollection<Course> GradeCollection { get; set; } = new ObservableCollection<Course>();
        

        public GradeDetailPageViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                LoadDesignTimeData();
            }
        }

        private void LoadDesignTimeData()
        {
            DesignTimeGradeList.Add(new Course()
            {
                FullName = "信息科学与技术学院计算机科学与技术专业",
                Credits = "3",
                RegularGrade = "100",
                FinalexamGrade = "95",
                TotalMark = "96",
                GradePoint = "4.6"
            });

            DesignTimeGradeList.Add(new Course()
            {
                FullName = "信息科学与技术",
                Credits = "3",
                RegularGrade = "100",
                FinalexamGrade = "95",
                TotalMark = "96.5",
                GradePoint = "4.6"
            });

            DesignTimeGradeList.Add(new Course()
            {
                FullName = "信息科学与技术",
                Credits = "3",
                RegularGrade = "100",
                FinalexamGrade = "95",
                TotalMark = "96.5",
                GradePoint = "4.6"
            });
        }
        
    }
}
