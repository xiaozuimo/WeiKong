using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using cqmu.MVVM.ViewModels;
using cqmu.MVVM.Models;
using cqmu.MVVM.Service;
using System.Diagnostics;
using Windows.UI.Popups;
using cqmu.MVVM.Helper;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Input;
using System.Collections.ObjectModel;
using cqmu.MVVM.Views;

namespace cqmu.MVVM.ViewModels
{
    public class CoursePageViewModel : ViewModelBase
    {
        //添加课程时所需的List
        public ObservableCollection<string> SelectedModeList => new ObservableCollection<string> { "单周", "双周", "连续" };
        public ObservableCollection<string> DayOfWeekList => new ObservableCollection<string> { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };
        public ObservableCollection<string> CourseStartList { get; set; }
        public ObservableCollection<string> CourseEndList { get; set; }
        public ObservableCollection<string> WeekStartList { get; set; }
        public ObservableCollection<string> WeekEndList { get; set; }

        private string _title = "第 1 周";//用于在界面上显示当前周数

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private Visibility _showCurrentWeek = Visibility.Visible;//用于判断所显示的课表是否为当前周课表

        public Visibility ShowCurrentWeek
        {
            get { return _showCurrentWeek; }
            set
            {
                _showCurrentWeek = value;
                NotifyPropertyChanged("ShowCurrentWeek");
            }
        }

        //public Visibility NavigationButtonVisibility => (DeviceHelper.IsMobile() || !App.AppSettings.UserSignedIn) ? Visibility.Collapsed : Visibility.Visible;
        
        public SolidColorBrush[] bgdCourse = new SolidColorBrush[12];
        public static int colorIndex = new Random().Next(12);

        List<List<Course>> weeklyList;
        private TextBlock _tbkMonth = new TextBlock() { TextAlignment = TextAlignment.Center, Foreground = new SolidColorBrush(Colors.White) };
        private List<TextBlock> _listDateDetail = new List<TextBlock>();
        private Grid _rootGrid = new Grid();
        private Grid _courseGrid = new Grid();// { MinHeight = 500 };
        private Grid _detailGrid = new Grid();
        private ScrollViewer _detailPanel = new ScrollViewer();
        private string[] _dayOfWeek = new string[] { "一", "二", "三", "四", "五", "六", "日" };
        private Pivot _pivot = new Pivot();
        private ScrollViewer _sv0 = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden};
        private ScrollViewer _sv1 = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
        private ScrollViewer _sv2 = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };

        public int weekOfTerm;

        public CoursePageViewModel()
        {
            InitializeColor();

            InitializeComboData();
            //_rootGrid.Children.Add(_detailGrid);
        }

        //初始化课程背景色
        private void InitializeColor()
        {
            bgdCourse[0] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)31, (byte)182, (byte)110));
            bgdCourse[1] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)7, (byte)189, (byte)230));
            bgdCourse[2] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)222, (byte)16, (byte)16));
            bgdCourse[3] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)16, (byte)36, (byte)166));
            bgdCourse[4] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)194, (byte)17, (byte)83));
            bgdCourse[5] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)15, (byte)192, (byte)60));
            bgdCourse[6] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)224, (byte)30, (byte)92));
            bgdCourse[7] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)116, (byte)0, (byte)255));
            bgdCourse[8] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)190, (byte)199, (byte)15));
            bgdCourse[9] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)216, (byte)12, (byte)198));
            bgdCourse[10] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)0, (byte)122, (byte)204));
            bgdCourse[11] = new SolidColorBrush(Color.FromArgb((byte)200, (byte)68, (byte)21, (byte)122));
        }

        //加载添加课程时所需的数据
        private void InitializeComboData()
        {
            CourseStartList = new ObservableCollection<string>();
            CourseEndList = new ObservableCollection<string>();
            WeekStartList = new ObservableCollection<string>();
            WeekEndList = new ObservableCollection<string>();

            for (int i = 1; i <= 12; i++)
            {
                CourseStartList.Add(i + "节");

                CourseEndList.Add(i + "节");
            }

            for (int i = 1; i <= App.AppSettings.WeekNum; i++)
            {
                WeekStartList.Add(i + "周");

                WeekEndList.Add(i + "周");
            }
        }

        public async void InitializeRootGrid(Grid rootGrid)
        {
            CreateUI(rootGrid);

            CalculateWeekOfTerm();

            InitializeCourseGrid();

            await LoadCourseData(weekOfTerm, 0);
        }

        //创建课表界面的UI
        private void CreateUI(Grid rootGrid)
        {
            rootGrid.Children.Clear();
            rootGrid.RowDefinitions.Clear();
            rootGrid.ColumnDefinitions.Clear();

            //划分行、列
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });//用于显示月份等信息
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            rootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Pixel) });//用于显示课程标号等信息
            for(int i = 0; i < 7; i++)
            {
                rootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            //将显示月份和日期的文本框添加到界面上
            //_tbkMonth = new TextBlock() { Text = $"{DateTime.Today.Month}\n月" };
            rootGrid.Children.Add(_tbkMonth);

            _listDateDetail.Clear();
            for(int i = 0; i < 7; i++)
            {
                TextBlock tbkDayOfWeek = new TextBlock();
                tbkDayOfWeek.TextAlignment = TextAlignment.Center;
                tbkDayOfWeek.Foreground = new SolidColorBrush(Colors.White);
                //tbkDayOfWeek.Text = $"{i + 1}\n周{_dayOfWeek[i]}";
                Grid.SetColumn(tbkDayOfWeek, i + 1);
                rootGrid.Children.Add(tbkDayOfWeek);

                _listDateDetail.Add(tbkDayOfWeek);
            }

            //将用于显示课表的Grid添加到界面上，并添加相应的转轴控件
            _courseGrid = new Grid();
            Grid.SetColumnSpan(_courseGrid, 8);
            Grid.SetRow(_courseGrid, 1);
            rootGrid.Children.Add(_courseGrid);

            _pivot = new Pivot();
            _sv0 = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
            _sv1 = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
            _sv2 = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
            _pivot.Items.Add(new PivotItem() { Content = _sv0, Margin = new Thickness(0, -40, 0, 0) });
            _pivot.Items.Add(new PivotItem() { Content = _sv1, Margin = new Thickness(0, -40, 0, 0) });
            _pivot.Items.Add(new PivotItem() { Content = _sv2, Margin = new Thickness(0, -40, 0, 0) });
            _courseGrid.Children.Add(_pivot);

            _pivot.SelectionChanged += _pivot_SelectionChanged;

            //将显示课程细节的面板添加到界面上
            _detailPanel = new ScrollViewer() { Visibility = Visibility.Collapsed, Background = new SolidColorBrush(new Color() { A = 0x50, R = 0xf5, G = 0xf5, B = 0xf5 }), VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
            
            Grid.SetRowSpan(_detailPanel, rootGrid.RowDefinitions.Count);
            Grid.SetColumnSpan(_detailPanel, rootGrid.ColumnDefinitions.Count);
            rootGrid.Children.Add(_detailPanel);

            _detailPanel.Tapped += _detailPanel_Tapped;
        }

        //初始化课表主体部分，划分行列并初始化课程序号
        private void InitializeCourseGrid()
        {
            _courseGrid = new Grid();
            _courseGrid.MinHeight = 500;

            _courseGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Pixel) });
            for(int i = 0; i < 7; i++)
            {
                _courseGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            for(int j = 0; j < 12; j++)
            {
                _courseGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            _courseGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.25, GridUnitType.Star) });

        }

        //计算当前日期所对应的周数
        private void CalculateWeekOfTerm()
        {
            int daySpan = (DateTime.Today - App.AppSettings.TermlyDate).Days;

            if(daySpan < 0 || daySpan > (App.AppSettings.WeekNum) * 7)
            {
                weekOfTerm = 1;
            }
            else
            {
                weekOfTerm = 1 + daySpan / 7;
            }
        }

        //加载数据
        private async Task LoadCourseData(int weekOfTerm, int index)
        {
            //清空页面数据
            _sv0.Content = null;
            _sv1.Content = null;
            _sv2.Content = null;

            //周数显示
            Title = $"第 {weekOfTerm} 周";

            int daySpan = (DateTime.Today - App.AppSettings.TermlyDate).Days;
            int currentWeek = daySpan < 0 ? -1 : daySpan / 7;
            ShowCurrentWeek = weekOfTerm - 1 == currentWeek ? Visibility.Visible : Visibility.Collapsed;

            //初始化月份及日期
            DateTime weeklyDate = App.AppSettings.TermlyDate.AddDays((weekOfTerm - 1) * 7);

            _tbkMonth.Text = $"{weeklyDate.Month}\n月";

            for(int i = 0; i < 7; i++)
            {
                _listDateDetail[i].Text = $"{weeklyDate.AddDays(i).Day.ToString()}\n周{_dayOfWeek[i]}";

                _listDateDetail[i].Opacity = weeklyDate.AddDays(i) == DateTime.Today ? 1 : 0.7;
            }

            //清空课程面板的元素
            _courseGrid.Children.Clear();

            //初始化课程序号
            for (int j = 0; j < 12; j++)
            {
                TextBlock tbkCourseRank = new TextBlock();
                tbkCourseRank.VerticalAlignment = VerticalAlignment.Center;
                tbkCourseRank.TextAlignment = TextAlignment.Center;
                tbkCourseRank.Foreground = new SolidColorBrush(Colors.White);
                tbkCourseRank.Text = (j + 1).ToString();
                Grid.SetColumn(tbkCourseRank, 0);
                Grid.SetRow(tbkCourseRank, j);

                _courseGrid.Children.Add(tbkCourseRank);
            }

            //从本地加载当前周课表信息
            weeklyList = new List<List<Course>>();
            weeklyList = await CourseDataService.LoadWeeklyDataFromIsoStoreAsync(weeklyList, weekOfTerm);

            if (weeklyList != null)
            {
                for (int i = 0; i < weeklyList.Count; i++)
                {
                    for (int j = 0; j < weeklyList[i].Count; j++)
                    {
                        Style btnCourseStyle = App.Current.Resources["CourseButtonStyle"] as Style;
                        Course course = weeklyList[i][j];

                        Button btn = new Button();
                        
                        btn.Style = btnCourseStyle;
                        btn.Content = course.OverView;
                        btn.FontSize = 9.5;
                        btn.Margin = new Thickness(1);
                        btn.Background = bgdCourse[colorIndex];                       
                        btn.Tag = $"[{i}][{j}]";//用于标记点击时button位置

                        Grid.SetColumn(btn, i + 1);//第一列是课程序号
                        Grid.SetRow(btn, course.StartMark);
                        Grid.SetRowSpan(btn, course.CourseSpan);

                        //btn.Click += BtnCourse_Click;
                        btn.Tapped += btnCourse_Tapped;
                        btn.Holding += btnCourse_Holding;
                        btn.RightTapped += btnCourse_RightTapped;
                        _courseGrid.Children.Add(btn);

                        colorIndex = colorIndex < 11 ? ++colorIndex : 0;
                    }
                }
            }
            else
            {
                await new MessageDialog("获取课表内容失败，请重启应用！！").ShowAsync();
            }

            switch (index)
            {
                case 0:
                    _sv0.Content = _courseGrid;
                    break;
                case 1:
                    _sv1.Content = _courseGrid;
                    break;
                case 2:
                    _sv2.Content = _courseGrid;
                    break;
            }
        }

        //初始化课程详细信息面板
        private void InitializeDetailGrid(Course course)
        {
            _detailGrid = new Grid() { Height = 400, Width = 300, Background = new SolidColorBrush(Colors.Black), Opacity = 0.92, CornerRadius = new CornerRadius(10), Visibility = Visibility.Visible };
            
            //数据初始化
            List<string> labelList = new List<string>() { "课程", "教室", "教师", "学分", "性质", "时间" };
            List<string> detailList = new List<string>() { course.FullName, course.Classroom, course.Teacher, course.Credits, course.Classify, $"{course.StartTime} - {course.EndTime}" };

            //行列划分
            _detailGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            _detailGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            
            _detailGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });

            #region Lable - Detail
            for (int i = 0; i < 6; i++)
            {
                _detailGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                //标签
                TextBlock tbkLabel = new TextBlock();
                tbkLabel.Text = labelList[i];
                tbkLabel.HorizontalAlignment = HorizontalAlignment.Center;
                tbkLabel.VerticalAlignment = VerticalAlignment.Center;
                tbkLabel.Foreground = new SolidColorBrush(Colors.White);
                tbkLabel.FontSize = 15;

                Grid.SetRow(tbkLabel, i + 1);
                _detailGrid.Children.Add(tbkLabel);

                //详细信息 -- 使用TextBox的本意是打算将修改课程信息的权限授予用户，但思考一段时间后还是决定放弃
                TextBox txtDetail = new TextBox();
                txtDetail.Text = detailList[i].Replace("&nbsp;", "");
                txtDetail.TextWrapping = TextWrapping.Wrap;
                txtDetail.HorizontalAlignment = HorizontalAlignment.Left;
                txtDetail.VerticalAlignment = VerticalAlignment.Center;
                txtDetail.Margin = new Thickness(0, 0, 16, 0);
                txtDetail.Foreground = new SolidColorBrush(Colors.White);
                txtDetail.Background = new SolidColorBrush(Colors.Transparent);
                txtDetail.BorderThickness = new Thickness(0);
                txtDetail.Padding = new Thickness(6);
                txtDetail.FontSize = 15;
                txtDetail.IsHitTestVisible = false;

                Grid.SetRow(txtDetail, i + 1);
                Grid.SetColumn(txtDetail, 1);
                _detailGrid.Children.Add(txtDetail);
            }
            #endregion
            
            //底部确认按钮
            //_detailGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.8, GridUnitType.Star) });
            //Button btnConfirm = new Button() { Foreground = new SolidColorBrush(Colors.White), Style = App.Current.Resources["btnMenuStyle"] as Style, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top };
            //Grid.SetRow(btnConfirm, 7);
            //Grid.SetColumnSpan(btnConfirm, 2);
            //_detailGrid.Children.Add(btnConfirm);

            _detailGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });

            _detailPanel.Content = _detailGrid;

            //_detailGrid.Tapped += _detailGrid_Tapped;
        }
        
        //显示课程详细信息
        private void btnCourse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button btn = sender as Button;

            Match tagMatch = Regex.Match(btn.Tag.ToString(), @"\[(\d+?)\]\[(\d+?)\]");
            int i = int.Parse(tagMatch.Groups[1].Value);
            int j = int.Parse(tagMatch.Groups[2].Value);

            InitializeDetailGrid(weeklyList[i][j]);

            _detailPanel.Visibility = Visibility.Visible;
        }

        //长按删除课程(手机)
        private async void btnCourse_Holding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;

            try
            {
                Button btn = sender as Button;

                Match tagMatch = Regex.Match(btn.Tag.ToString(), @"\[(\d+?)\]\[(\d+?)\]");
                int i = int.Parse(tagMatch.Groups[1].Value);
                int j = int.Parse(tagMatch.Groups[2].Value);

                MessageDialog dialog = new MessageDialog("将要删除本节课，是吗？");
                dialog.Title = "温馨提示";
                dialog.Commands.Add(new UICommand("确定", async command =>
                {
                    CoursePage.Current.TipPanel.Visibility = Visibility.Visible;

                    await CourseDataService.DeleteCourse(weeklyList, i, j, weekOfTerm);

                    InitializeRootGrid(CoursePage.Current.CourseGrid);

                    CoursePage.Current.TipPanel.Visibility = Visibility.Collapsed;
                }));
                dialog.Commands.Add(new UICommand("取消"));

                await dialog.ShowAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //右键删除课程(电脑)
        private async void btnCourse_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;

            try
            {
                Button btn = sender as Button;

                Match tagMatch = Regex.Match(btn.Tag.ToString(), @"\[(\d+?)\]\[(\d+?)\]");
                int i = int.Parse(tagMatch.Groups[1].Value);
                int j = int.Parse(tagMatch.Groups[2].Value);

                MessageDialog dialog = new MessageDialog("将要删除本节课，是吗？");
                dialog.Title = "温馨提示";
                dialog.Commands.Add(new UICommand("确定", async command =>
                {
                    CoursePage.Current.TipPanel.Visibility = Visibility.Visible;

                    await CourseDataService.DeleteCourse(weeklyList, i, j, weekOfTerm);

                    InitializeRootGrid(CoursePage.Current.CourseGrid);

                    CoursePage.Current.TipPanel.Visibility = Visibility.Collapsed;
                }));
                dialog.Commands.Add(new UICommand("取消"));

                await dialog.ShowAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void _detailGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void _detailPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _detailPanel.Visibility = Visibility.Collapsed;

            e.Handled = true;
        }

        public void btnLastWeek_Click(object sender,RoutedEventArgs e)
        {
            if (_detailPanel.Visibility == Visibility.Visible)
            {
                _detailPanel.Visibility = Visibility.Collapsed;

                return;
            }

            if (_pivot.SelectedIndex == 0)
                _pivot.SelectedIndex = 2;
            else
                --_pivot.SelectedIndex;
        }

        public void btnNextWeek_Click(object sender, RoutedEventArgs e)
        {
            if (_detailPanel.Visibility == Visibility.Visible)
            {
                _detailPanel.Visibility = Visibility.Collapsed;

                return;
            }

            if (_pivot.SelectedIndex == 2)
                _pivot.SelectedIndex = 0;
            else
                ++_pivot.SelectedIndex;
        }

        //ComboBox逻辑
        public void ComboCourseOrWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int index;

            switch (combo.Name)
            {
                case "ComboCourseStart":
                    index = CoursePage.Current.ComboCourseEnd.SelectedIndex;
                    CoursePage.Current.ComboCourseEnd.SelectedIndex = index > combo.SelectedIndex ? index : combo.SelectedIndex;
                    break;
                case "ComboCourseEnd":
                    index = CoursePage.Current.ComboCourseStart.SelectedIndex;
                    CoursePage.Current.ComboCourseStart.SelectedIndex = index < combo.SelectedIndex ? index : combo.SelectedIndex;
                    break;
                case "ComboWeekStart":
                    index = CoursePage.Current.ComboWeekEnd.SelectedIndex;
                    CoursePage.Current.ComboWeekEnd.SelectedIndex = index > combo.SelectedIndex ? index : combo.SelectedIndex;
                    break;
                case "ComboWeekEnd":
                    index = CoursePage.Current.ComboWeekStart.SelectedIndex;
                    CoursePage.Current.ComboWeekStart.SelectedIndex = index < combo.SelectedIndex ? index : combo.SelectedIndex;
                    break;
            }
            
        }
        
        //确认添加课程
        public async void btnAddConfirm_Click(object sender, RoutedEventArgs e)
        {
            var current = CoursePage.Current;
            if (string.IsNullOrEmpty(current.txtCourseName.Text) ||
                current.ComboDayOfWeek.SelectedIndex == -1 || current.ComboSelectedMode.SelectedIndex == -1 ||
                current.ComboCourseStart.SelectedIndex == -1 || current.ComboCourseEnd.SelectedIndex == -1 ||
                current.ComboWeekStart.SelectedIndex == -1 || current.ComboWeekEnd.SelectedIndex == -1)
            {
                await new MessageDialog("亲，带 * 的都是必填选项哟，先check一下下吧(●'◡'●)").ShowAsync();

                return;
            }

            Course course = new Course();
            course.FullName = current.txtCourseName.Text;
            course.Classroom = current.txtClassroom.Text;
            course.Teacher = current.txtTeacher.Text;
            course.Credits = current.txtCredit.Text;
            course.Classify = current.txtClassify.Text;
            course.StartMark = current.ComboCourseStart.SelectedIndex;
            course.CourseSpan = current.ComboCourseEnd.SelectedIndex - current.ComboCourseStart.SelectedIndex + 1;

            //处理UI及后台逻辑
            current.TipPanel.Visibility = Visibility.Visible;
            current.AddCourseGrid.Visibility = Visibility.Collapsed;

            string termlyString = await CourseDataService.ProcessCourse(course, current.ComboSelectedMode.SelectedIndex, current.ComboDayOfWeek.SelectedIndex, current.ComboWeekStart.SelectedIndex, current.ComboWeekEnd.SelectedIndex);

            if (!string.IsNullOrEmpty(termlyString))
            {
                await CourseDataService.SaveTermlyJsonToIsoStoreAsync(termlyString);
                
                current.ViewModel.InitializeRootGrid(current.CourseGrid);

                current.TipPanel.Visibility = Visibility.Collapsed;
                current.btnUserControl.IsEnabled = true;
            }
            else
            {
                current.TipPanel.Visibility = Visibility.Collapsed;
                current.btnUserControl.IsEnabled = true;

                await new MessageDialog("添加课程失败了，再试一次吧~_~").ShowAsync();
            }
        }

        //取消添加课程
        public void btnAddCancel_Click(object sender, RoutedEventArgs e)
        {
            CoursePage.Current.AddCourseGrid.Visibility = Visibility.Collapsed;

            CoursePage.Current.btnUserControl.IsEnabled = true;
        }

        //转轴切周
        private async void _pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(App.AppSettings.UserSignedIn);
            int index = _pivot.SelectedIndex;
            bool isNormalCase = true;//用于判断事件是否因跳转到课表页面而触发

            if ((index == 0 && _sv1.Content != null) || (index == 1 && _sv2.Content != null) || (index == 2 && _sv0.Content != null))//左→右
                --weekOfTerm;
            else if ((index == 0 && _sv2.Content != null) || (index == 1 && _sv0.Content != null) || (index == 2 && _sv1.Content != null))//右→左
                ++weekOfTerm;
            else
                isNormalCase = false;

            if (isNormalCase)
            {
                if (weekOfTerm < 1)
                    weekOfTerm = App.AppSettings.WeekNum;
                else if (weekOfTerm > App.AppSettings.WeekNum)
                    weekOfTerm = 1;

                try
                {
                    await LoadCourseData(weekOfTerm, index);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
