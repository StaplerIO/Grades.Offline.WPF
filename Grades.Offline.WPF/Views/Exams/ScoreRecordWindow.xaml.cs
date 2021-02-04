using CsvHelper;
using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Models.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grades.Offline.WPF.Views.Exams
{
    /// <summary>
    /// Interaction logic for ScoreRecordWindow.xaml
    /// User will be redirected to this window after entered basic informations of the exam
    /// </summary>
    public partial class ScoreRecordWindow : Window
    {
        private readonly ApplicationDbContext _dbContext;

        // Properties required in multiple methods
        private readonly List<DbSubject> _subjects = new List<DbSubject>();
        private readonly List<DbStudent> _students = new List<DbStudent>();

        private DbExam Exam { get; set; }

        public ScoreRecordWindow(List<Guid> studentsId, List<Guid> subjectsId, DbExam exam)
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();
            DataContext = this;

            var dataTable = new DataTable();
            dataTable.Columns.Add("Student").ReadOnly = true;

            // Add row to record full mark for each subject
            dataTable.Rows.Add("[Full mark]");

            // Add column Subject
            var subjects = new List<DbSubject>();
            subjectsId.ForEach(sId =>
            {
                var subject = _dbContext.Subjects.FirstOrDefault(s => s.Id == sId);
                subjects.Add(subject);
                dataTable.Columns.Add(subject.Name);
            });
            _subjects = subjects;

            // Add Student rows
            var students = new List<DbStudent>();
            studentsId.ForEach(sId =>
            {
                var student = _dbContext.Students.FirstOrDefault(s => s.Id == sId);
                students.Add(student);
                dataTable.Rows.Add(student.FullName);
            });
            _students = students;

            MasterTable.ItemsSource = dataTable.DefaultView;

            Exam = exam;
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            DoneButton.Visibility = Visibility.Collapsed;
            ProgressRing.Visibility = Visibility.Visible;

            // Disale all interactions with data table
            MasterTable.IsEnabled = false;


            var scoreSummary = new ExamScoreSummary
            {
                StudentScores = new List<ExamStudentScore>(),
                SubjectScores = new List<ExamSubject>()
            };

            var fullMarkRow = (MasterTable.Items[0] as DataRowView).Row;
            // Ignore first column (Name)
            for (int subjectIndex = 1; subjectIndex < fullMarkRow.ItemArray.Length; subjectIndex++)
            {
                scoreSummary.SubjectScores.Add(new ExamSubject
                {
                    SubjectId = _subjects.ElementAt(subjectIndex - 1).Id,
                    TotalScore = decimal.Parse((string)fullMarkRow.ItemArray[subjectIndex])
                });
            }

            // Add student score
            for (int studentIndex = 1; studentIndex < MasterTable.Items.Count; studentIndex++)
            {
                var item = MasterTable.Items[studentIndex];
                var dataRow = ((DataRowView)item).Row;
                var itemArray = dataRow.ItemArray;

                var studentScores = new ExamStudentScore
                {
                    StudentId = _students.ElementAt(studentIndex - 1).Id,
                    SubjectScored = new Dictionary<Guid, decimal>()
                };

                // Ignore first column (Student name)
                for (int subjectIndex = 1; subjectIndex < itemArray.Length; subjectIndex++)
                {
                    studentScores.SubjectScored.Add(_subjects.ElementAt(subjectIndex - 1).Id, decimal.Parse((string)itemArray[subjectIndex]));
                }

                scoreSummary.StudentScores.Add(studentScores);

                Exam.StudentScores = scoreSummary;
            }

            _dbContext.Exams.Add(Exam);
            await _dbContext.SaveChangesAsync();

            // Close window, trigger event
            Close();
        }

        private void MasterTable_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!decimal.TryParse(e.Text, out _) && e.Text != ".")
            {
                e.Handled = true;
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CSV Files|*.csv",
                FilterIndex = 1,
                Multiselect = false
            };

            // Selected a file
            if (dialog.ShowDialog().Value)
            {
                // Get selected file
                var fileName = dialog.FileName;
                var content = new StreamReader(fileName);

                // Load data to DataTable
                var csvReader = new CsvReader(content, CultureInfo.InvariantCulture);
                var dataReader = new CsvDataReader(csvReader);
                var csvDataTable = new DataTable();
                csvDataTable.Load(dataReader);
            }
        }
    }
}
