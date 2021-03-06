﻿using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Helpers;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Views.Exams;
using LiveCharts;
using LiveCharts.Wpf;
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

        #region LatestExamModels
        private string[] ExamDates;
        #endregion

        public StudentDetailPage(Guid studentId)
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();

            Student = _dbContext.Students.FirstOrDefault(s => s.Id == studentId);

            StudentNameTextBox.Text = Student.FullName;
            StudentSnoTextBox.Text = Student.Sno.ToString();

            InitialRecentExamsTab();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StudentNameTextBox.Text) && !string.IsNullOrWhiteSpace(StudentSnoTextBox.Text))
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

        private void InitialRecentExamsTab()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(Localization.Resources.Date);

            var exams = _dbContext.Exams
                .Where(e => e.ClassId == Student.ClassId)
                // Recent exams at first
                .OrderByDescending(e => e.Date)
                .ToList();

            // This student attended the exam
            exams.Where(e => e.StudentScores.IsStudentAttended(Student.Id))
                // Take first 3 exams
                .Take(3)
                .ToList()
                .ForEach(exam =>
                {
                    var currentExamTable = RankTabelExtension.GetUIFriendlyRankTableByExamId(exam.Id);

                    object[] targetRowData = null;
                    foreach (DataRow row in currentExamTable.Rows)
                    {
                        // A student row is structured like this: (<Sno>) <FullName>, so we get the last characters (This may cause error!)
                        if (row.ItemArray[0].ToString().EndsWith(Student.FullName))
                        {
                            targetRowData = row.ItemArray;
                            break;
                        }
                    }

                    // Add new subjects recursively
                    currentExamTable.Columns.RemoveAt(0);
                    foreach (DataColumn subjectColumn in currentExamTable.Columns)
                    {
                        if (!dataTable.Columns.Contains(subjectColumn.ColumnName))
                        {
                            dataTable.Columns.Add(subjectColumn.ColumnName);
                        }
                    }

                    // Add ExamDate to first column
                    // Delete first item in targetRowData because the first item stored student name
                    object[] actualRowData = new object[] { exam.Date.ToShortDateString() };
                    actualRowData = actualRowData.Concat(targetRowData[1..]).ToArray();
                    dataTable.Rows.Add(actualRowData);
                });

            LatestExamPersonalData.ItemsSource = dataTable.DefaultView;
           
            #region InitialChart
            var chartSeries = new SeriesCollection();
            var labels = new List<string>();

            // We don't need column TotalScore which is the last column
            // Also, we don't need the first column which stored the date of the exam
            for (int i = 1; i < dataTable.Columns.Count; i++)
            {
                var columnName = dataTable.Columns[i].ColumnName;
                chartSeries.Add(new ColumnSeries
                {
                    Title = columnName,
                    Values = new ChartValues<decimal>()
                });
            }

            // Add total score series
            chartSeries.Add(new LineSeries
            {
                Title = dataTable.Columns[dataTable.Columns.Count - 1].ColumnName,
                Values = new ChartValues<decimal>(),
                Fill = Brushes.Transparent
            });
            foreach (DataRow row in dataTable.Rows)
            {
                // Just like we said before...
                for (int i = 1; i < dataTable.Columns.Count - 1; i++)
                {
                    var rowData = row.ItemArray[i].ToString();
                    decimal currentSubjectScore = decimal.Parse(rowData.Substring(0, rowData.IndexOf('(')));

                    // Use [i - 1] because the counter starts from 1
                    chartSeries[i - 1].Values.Add(currentSubjectScore);
                }

                // Add total score
                var lastRowItem = row.ItemArray.Last().ToString();
                decimal total = decimal.Parse(lastRowItem.Substring(0, lastRowItem.IndexOf('(')));
                chartSeries.Last().Values.Add(total);

                labels.Add(row.ItemArray[0].ToString());
            }

            LatestExamChart.Series = chartSeries;
            ExamDates = labels.ToArray();
            #endregion
        }
    }
}
