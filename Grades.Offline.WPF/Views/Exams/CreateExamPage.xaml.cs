using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Models.ViewModels;
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

        public CreateExamPage()
        {
            _dbContext = new ApplicationDbContext();

            InitializeComponent();
            DataContext = this;

            #region InitialClassSelector
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            // Tip selection
            dataTable.Rows.Add(Guid.Empty, "Select a class");

            // Class selection
            _dbContext.Classes.ToList().ForEach(c => dataTable.Rows.Add(c.Id, c.Name));
            ClassSelector.ItemsSource = dataTable.DefaultView;
            ClassSelector.SelectedIndex = 0;
            #endregion
        }

        // Selected a class, update the student name list
        private void ClassSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Which class?
            var selectedRowElementArray = ((DataRowView)ClassSelector.SelectedItem).Row.ItemArray;
            var classId = (Guid)selectedRowElementArray.ElementAt(0);
            var @class = _dbContext.Classes.FirstOrDefault(c => c.Id == classId);

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
                            .Where(s => s.Class == @class)
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
                            .Where(s => s.Class == @class)
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

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
