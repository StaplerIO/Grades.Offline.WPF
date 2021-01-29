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
        }

        // If we do this in constructor, the ClassSelector will be NULL, will throw exceptions
        private void ClassSelector_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            // Tip selection
            dataTable.Rows.Add(Guid.Empty, "Select a class");

            // Class selection
            _dbContext.Classes.ToList().ForEach(c => dataTable.Rows.Add(c.Id, c.Name));
            ClassSelector.ItemsSource = dataTable.DefaultView;
            ClassSelector.SelectedIndex = 0;
        }

        // Examiee is Student
        private void ExamieeList_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
            // Checkbox property
            dataTable.Columns.Add(new DataColumn("Attended", typeof(bool)));
        }
    }
}
