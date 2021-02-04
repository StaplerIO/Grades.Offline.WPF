using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Helpers;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Views.Exams;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grades.Offline.WPF.Views.Students
{
    /// <summary>
    /// Interaction logic for StudentDetailPage.xaml
    /// </summary>
    public partial class StudentDetailPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        private DbStudent Student { get; set; }

        public StudentDetailPage(Guid studentId)
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();

            Student = _dbContext.Students.FirstOrDefault(s => s.Id == studentId);

            StudentNameTextBox.Text = Student.FullName;
            StudentSnoTextBox.Text = Student.Sno.ToString();

            InitialLatestExamTab();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(StudentNameTextBox.Text) && !string.IsNullOrWhiteSpace(StudentSnoTextBox.Text))
            {
                UpdateButton.Visibility = Visibility.Collapsed;
                ProgressRing.Visibility = Visibility.Visible;

                Student.FullName = StudentNameTextBox.Text;
                Student.Sno = int.Parse(StudentSnoTextBox.Text);

                _dbContext.Students.Update(Student);
                await _dbContext.SaveChangesAsync();

                UpdateButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;

                // reload page
                DataContext = null;
                DataContext = new StudentDetailPage(Student.Id);
            }
        }

        private void StudentSnoTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }

        private void InitialLatestExamTab()
        {
            var examsOrderedByDate = _dbContext.Exams
                .Where(e => e.ClassId == Student.ClassId)
                .OrderByDescending(e => e.Date)
                .ToList();

            var latestExam = examsOrderedByDate
                .Where(e => e.StudentScores.IsStudentAttended(Student.Id))
                .FirstOrDefault();

            #region InitialTextBox
            ExamOverviewSimplifiedTextBlock.Text = $"[{latestExam.Name}] at [{latestExam.Date.ToShortDateString()}]";
            ExamOverviewSimplifiedLabel.MouseDoubleClick += (s, e) => NavigationService.Navigate(new ExamDetailPage(latestExam.Id));
            #endregion

            #region InitialDataGrid
            var dataTable = RankTabelExtension.GetUIFriendlyRankTableByExamId(latestExam.Id);

            object[] targetRowData = null;
            foreach (DataRow row in dataTable.Rows)
            {
                // A student row is structured like this: (<Sno>) <FullName>, so we get the last characters (This may cause error!)
                if (row.ItemArray[0].ToString().EndsWith(Student.FullName))
                {
                    targetRowData = row.ItemArray;
                    break;
                }
            }

            dataTable.Rows.Clear();
            dataTable.Rows.Add(targetRowData);
            dataTable.Columns.RemoveAt(0);

            LatestExamPersonalData.ItemsSource = dataTable.DefaultView;
            #endregion
        }
    }
}
