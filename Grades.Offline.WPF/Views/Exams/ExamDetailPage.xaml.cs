using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
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

        public ExamDetailPage(Guid examId)
        {
            _dbContext = new ApplicationDbContext();

            InitializeComponent();
            DataContext = this;

            Exam = _dbContext.Exams.FirstOrDefault(e => e.Id == examId);

            ExamNameTextBox.Text = Exam.Name;
            ExamDatePicker.SelectedDate = Exam.Date;

            InitiateRankTable();
        }

        private void InitiateRankTable()
        {
            var dataTable = new DataTable();

            #region DataTableColumns
            dataTable.Columns.Add("Student");
            var examSummary = Exam.StudentScores;
            examSummary.SubjectScores.ForEach(subjectScore =>
            {
                var subject = _dbContext.Subjects.FirstOrDefault(s => s.Id == subjectScore.SubjectId);
                dataTable.Columns.Add($"{subject.Name} ({subjectScore.TotalScore})");
            });
            dataTable.Columns.Add($"Total ({examSummary.TotalScore})");
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

                dataTable.Rows.Add(rowData.ToArray());
            });

            // dataTable.Rows.Add("Average", new { });
            #endregion

            RankTable.ItemsSource = dataTable.DefaultView;
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
