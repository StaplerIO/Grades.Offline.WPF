using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Models.ViewModels;
using MahApps.Metro.Controls;
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

namespace Grades.Offline.WPF.Views.Exams
{
    /// <summary>
    /// Interaction logic for ExamDetailPage.xaml
    /// </summary>
    public partial class ExamDetailPage : Page
    {
        private readonly ApplicationDbContext _dbContext;
        private DbExam Exam { get; set; }

        private DataTable _dataTable { get; set; } = new DataTable();

        public ExamDetailPage(Guid examId)
        {
            _dbContext = new ApplicationDbContext();

            InitializeComponent();
            DataContext = this;

            Exam = _dbContext.Exams.FirstOrDefault(e => e.Id == examId);

            ExamNameTextBox.Text = Exam.Name;
            ExamDatePicker.SelectedDate = Exam.Date;

            TotalScoreLabel.Content = Exam.StudentScores.TotalScore.ToString();

            InitiateRankTable();
        }

        private void InitiateRankTable()
        {
            _dataTable = new DataTable();

            #region DataTableColumns
            _dataTable.Columns.Add("Student");
            var examSummary = Exam.StudentScores;
            examSummary.SubjectScores.ForEach(subjectScore =>
            {
                var subject = _dbContext.Subjects.FirstOrDefault(s => s.Id == subjectScore.SubjectId);
                _dataTable.Columns.Add($"{subject.Name} ({subjectScore.TotalScore})");
            });
            _dataTable.Columns.Add($"Total ({examSummary.TotalScore})");
            #endregion

            #region DataTableRows
            examSummary.StudentScores.ForEach(studentScore =>
            {
                var student = _dbContext.Students.FirstOrDefault(s => s.Id == studentScore.StudentId);

                var rowData = new List<object>();

                rowData.Add($"({student.Sno}) {student.FullName}");
                for (int i = 0; i < studentScore.SubjectScored.Count; i++)
                {
                    var subject = examSummary.SubjectScores[i];
                    var subjectScore = studentScore.SubjectScored[subject.SubjectId];

                    // Calculate correct rate by saving 2 numbers after decimal point
                    rowData.Add($"{subjectScore} ({subjectScore / subject.TotalScore * 100:#.##}%)");
                }
                rowData.Add($"{studentScore.TotalScore} ({studentScore.TotalScore / examSummary.TotalScore * 100:#.##}%)");

                _dataTable.Rows.Add(rowData.ToArray());
            });

            // _dataTable.Rows.Add(AverageScoreRowData(examSummary));

            #endregion

            RankTable.ItemsSource = _dataTable.DefaultView;
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateButton.Visibility = Visibility.Collapsed;
            ProgressRing.Visibility = Visibility.Visible;

            Exam.Name = ExamNameTextBox.Text;
            if (ExamDatePicker.SelectedDate.HasValue)
            {
                Exam.Date = ExamDatePicker.SelectedDate.Value;
            }

            _dbContext.Exams.Update(Exam);
            await _dbContext.SaveChangesAsync();

            // reload page
            DataContext = null;
            DataContext = new ExamDetailPage(Exam.Id);
        }

        private void ShowAverageScoreSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggler = (ToggleSwitch)sender;
            if (toggler.IsOn)
            {
                _dataTable.Rows.Add(AverageScoreRowData(Exam.StudentScores));
                RankTable.SelectedIndex = _dataTable.Rows.Count - 1;
            }
            else
            {
                _dataTable.Rows.RemoveAt(_dataTable.Rows.Count - 1);
            }
        }

        private object[] AverageScoreRowData(ExamScoreSummary examSummary)
        {
            // Add first column (name)
            var averageScores = new List<object>
            {
                "[Average]"
            };
            
            // Calculate each subject
            examSummary.SubjectScores.ForEach(s =>
            {
                var currentAverage = examSummary.AverageScore(s.SubjectId);
                var averagePercentage = currentAverage / s.TotalScore;
                averageScores.Add($"{currentAverage:#.##} ({averagePercentage * 100:#.##}%)");
            });

            decimal averageTotalScore = 0;
            // Calculate total score
            examSummary.StudentScores.ForEach(s =>
            {
                averageTotalScore += s.TotalScore;
            });
            averageTotalScore /= examSummary.StudentScores.Count;
            averageScores.Add($"{averageTotalScore:#.##} ({averageTotalScore / examSummary.TotalScore * 100:#.##}%)");

            return averageScores.ToArray();
        }
    }
}
