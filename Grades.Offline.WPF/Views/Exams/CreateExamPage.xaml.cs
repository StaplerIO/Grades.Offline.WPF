using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Models.ViewModels;
using Grades.Offline.WPF.Views.Classes;
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
    /// Interaction logic for CreateExamPage.xaml
    /// </summary>
    public partial class CreateExamPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        private DbClass _selectedClass;

        public CreateExamPage(Guid? classId)
        {
            _dbContext = new ApplicationDbContext();

            InitializeComponent();
            DataContext = this;

            #region InitialClassSelector
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            // Class selection
            dataTable.Rows.Add(Guid.Empty, "Select a class");
            if (classId.HasValue)
            {
                var @class = _dbContext.Classes.FirstOrDefault(c => c.Id == classId.Value);
                dataTable.Rows.Add(classId, @class.Name);

                ClassSelector.SelectedIndex = 1;
            }
            else
            {
                _dbContext.Classes.ToList().ForEach(c => dataTable.Rows.Add(c.Id, c.Name));
                ClassSelector.SelectedIndex = 0;
            }

            ClassSelector.ItemsSource = dataTable.DefaultView;
            #endregion
        }

        // Selected a class, update the student name list
        private void ClassSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Which class?
            var selectedRowElementArray = ((DataRowView)ClassSelector.SelectedItem).Row.ItemArray;
            var classId = (Guid)selectedRowElementArray.ElementAt(0);
            var @class = _selectedClass = _dbContext.Classes.FirstOrDefault(c => c.Id == classId);

            if (ClassSelector.SelectedItem != null)
            {
                #region UpdateStudentList
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
                    dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
                    // Checkbox property
                    dataTable.Columns.Add(new DataColumn("Attended", typeof(bool)));

                    // If selected a class, then add students to list
                    // If not, leave it empty
                    if (@class != null)
                    {
                        _dbContext.Students
                            .Where(s => s.ClassId == classId)
                            .ToList()
                            .ForEach(s =>
                            {
                                dataTable.Rows.Add(s.Id, s.FullName, false);
                            });
                    }

                    ExamieeList.ItemsSource = dataTable.DefaultView;
                }

                #endregion

                #region UpdateSubjectList
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
                    dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
                    // Checkbox property
                    dataTable.Columns.Add(new DataColumn("Attended", typeof(bool)));

                    // If selected a class, then add students to list
                    // If not, leave it empty
                    if (@class != null)
                    {
                        _dbContext.Subjects
                            .Where(s => s.ClassId == classId)
                            .ToList()
                            .ForEach(s =>
                            {
                                dataTable.Rows.Add(s.Id, s.Name, false);
                            });
                    }

                    SubjectList.ItemsSource = dataTable.DefaultView;
                }
                #endregion
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate data input
            if (!string.IsNullOrWhiteSpace(ExamNameTextBox.Text) && ExamDate.SelectedDate.HasValue)
            {
                // Disable all input
                ExamNameTextBox.IsEnabled = false;
                ExamDate.IsEnabled = false;
                ExamieeList.IsEnabled = false;
                SubjectList.IsEnabled = false;

                NextButton.Visibility = Visibility.Collapsed;
                ProgressRing.Visibility = Visibility.Visible;

                var attendedStudentsId = new List<Guid>();
                var attendedSubjectsId = new List<Guid>();

                foreach (var examieeRow in ExamieeList.Items)
                {
                    var rowData = ((DataRowView)examieeRow).Row;

                    Guid studentId = (Guid)rowData.ItemArray[0];
                    bool isAttended = (bool)rowData.ItemArray[2];
                    if (isAttended)
                    {
                        attendedStudentsId.Add(studentId);
                    }
                }

                foreach (var subjectRow in SubjectList.Items)
                {
                    var rowData = ((DataRowView)subjectRow).Row;

                    Guid subjectId = (Guid)rowData.ItemArray[0];
                    bool isAttended = (bool)rowData.ItemArray[2];
                    if (isAttended)
                    {
                        attendedSubjectsId.Add(subjectId);
                    }
                }

                var scoreRecordWindow = new ScoreRecordWindow(attendedStudentsId, attendedSubjectsId, new DbExam { Name = ExamNameTextBox.Text, Date = ExamDate.SelectedDate.Value, ClassId = _selectedClass.Id });
                scoreRecordWindow.Show();

                scoreRecordWindow.Closed += (s, e) =>
                {
                    MessageBox.Show("Exam created successfully!", "Grades", MessageBoxButton.OK);
                    NavigationService.Navigate(new ClassDetailPage(_selectedClass.Id));
                };
            }
        }
    }
}
