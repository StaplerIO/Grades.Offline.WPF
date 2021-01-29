using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Models.ViewModels;
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

namespace Grades.Offline.WPF.Views.Classes
{
    /// <summary>
    /// Interaction logic for ClassDetailPage.xaml
    /// </summary>
    public partial class ClassDetailPage : Page
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbClass _class;

        public ClassDetailPage(Guid classId)
        {
            InitializeComponent();

            _dbContext = new ApplicationDbContext();
            _class = _dbContext.Classes.FirstOrDefault(c => c.Id == classId);

            ClassNameLabel.Content = $"Class: {_class.Name}";
            ClassIdLabel.Content = _class.Id;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void StudentList_Loaded(object sender, RoutedEventArgs e)
        {
            var students = _dbContext.Students
                .Where(s => s.Class == _class)
                .ToList();

            // Sort by Sno
            students.Sort((a, b) => a.Sno.CompareTo(b.Sno));

            var studentViewList = new List<StudentViewModel>();
            foreach (var student in students)
            {
                studentViewList.Add(new StudentViewModel
                {
                    Sno = student.Sno,
                    FullName = student.FullName
                });
            }
            StudentList.ItemsSource = studentViewList;
        }
    }
}
