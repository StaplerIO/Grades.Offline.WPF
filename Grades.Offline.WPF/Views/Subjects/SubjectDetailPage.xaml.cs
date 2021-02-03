using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
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

namespace Grades.Offline.WPF.Views.Subjects
{
    /// <summary>
    /// Interaction logic for SubjectDetailPage.xaml
    /// </summary>
    public partial class SubjectDetailPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        private DbSubject Subject { get; set; }

        #region ChartOptions
        public string[] ExamDates { get; set; }
        #endregion

        public SubjectDetailPage(Guid subjectId)
        {
            _dbContext = new ApplicationDbContext();
            Subject = _dbContext.Subjects.FirstOrDefault(s => s.Id == subjectId);

            InitializeComponent();
            DataContext = this;

            SubjectNameTextBox.Text = Subject.Name;
            InitialGraph();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateButton.Visibility = Visibility.Collapsed;
            ProgressRing.Visibility = Visibility.Visible;

            Subject.Name = SubjectNameTextBox.Text;

            _dbContext.Subjects.Update(Subject);
            await _dbContext.SaveChangesAsync();

            // reload page
            DataContext = null;
            DataContext = new SubjectDetailPage(Subject.Id);

            UpdateButton.Visibility = Visibility.Visible;
            ProgressRing.Visibility = Visibility.Hidden;
        }

        private void InitialGraph()
        {
            // Select exams in specific class 
            var exams = _dbContext.Exams
                .Where(e => e.ClassId == Subject.ClassId)
                .ToList();

            // Select the exams which contains current subject
            exams = exams.Where(e => e.StudentScores.SubjectScores.FirstOrDefault(s => s.SubjectId == Subject.Id) != null).ToList();

            // Sort by date
            exams.Sort((a, b) => a.Date.CompareTo(b.Date));

            // Convet all dates from DateTime to ShortDateString
            ExamDates = exams.Select(s => s.Date)
                .ToList()
                .ConvertAll(d => d.ToShortDateString())
                .ToArray();

            var examSummaries = exams.Select(e => e.StudentScores).ToList();
            var graphValues = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Average Score",
                    Values = new ChartValues<decimal>(),
                    Fill = Brushes.Transparent,
                    AreaLimit = (double)examSummaries[0].SubjectScores.FirstOrDefault(s => s.SubjectId == Subject.Id).TotalScore
                },
                new LineSeries
                {
                    Title = "Highest Score",
                    Values = new ChartValues<decimal>(),
                    Fill = Brushes.Transparent,
                    AreaLimit = (double)examSummaries[0].SubjectScores.FirstOrDefault(s => s.SubjectId == Subject.Id).TotalScore
                },
                new LineSeries
                {
                    Title = "Lowest Score",
                    Values = new ChartValues<decimal>(),
                    Fill = Brushes.Transparent,
                    AreaLimit = (double)examSummaries[0].SubjectScores.FirstOrDefault(s => s.SubjectId == Subject.Id).TotalScore
                },
            };
           
            examSummaries.ForEach(e =>
            {
                graphValues[0].Values.Add(e.GetSubjectAverageScore(Subject.Id));
                graphValues[1].Values.Add(e.GetHighestScoreBySubject(Subject.Id).Value);
                graphValues[2].Values.Add(e.GetLowestScoreBySubject(Subject.Id).Value);
            });

            Chart.Series = graphValues;
        }
    }
}
