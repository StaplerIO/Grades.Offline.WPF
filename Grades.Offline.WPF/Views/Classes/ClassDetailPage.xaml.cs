using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Models.ViewModels;
using Grades.Offline.WPF.Views.Exams;
using Grades.Offline.WPF.Views.Students;
using Grades.Offline.WPF.Views.Subjects;
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

            #region InitialStudentList
            var students = _dbContext.Students
                .Where(s => s.ClassId == classId)
                .ToList();

            // Sort by Sno
            students.Sort((a, b) => a.Sno.CompareTo(b.Sno));

            StudentList.ItemsSource = students;
            #endregion

            #region InitialSubjectList
            var subjects = _dbContext.Subjects
               .Where(s => s.ClassId == classId)
               .ToList();

            SubjectList.ItemsSource = subjects;
            #endregion

            #region InitialExamList
            var exams = _dbContext.Exams
                .Where(e => e.ClassId == classId)
                .ToList();

            ExamList.ItemsSource = exams.ConvertAll(e => new ExamIndexViewModel
            {
                ExamId = e.Id,
                Name = e.Name,
                DateString = e.Date.ToShortDateString()
            });
            #endregion
        }

        private void ExamList_ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = ((ListViewItem)sender).Content as ExamIndexViewModel;
            NavigationService.Navigate(new ExamDetailPage(viewModel.ExamId));
        }

        private void CreateExamButton_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CreateExamPage(_class.Id));

        private void CreateSubjectButton_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CreateSubjectPage(_class.Id));

        private void AddStudentButton_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CreateStudentPage(_class.Id));

        private void SubjectList_ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var targetSubject = ((ListViewItem)sender).Content as DbSubject;
            NavigationService.Navigate(new SubjectDetailPage(targetSubject.Id));
        }

        private void StudentList_ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var targetStudent = ((ListViewItem)sender).Content as DbStudent;
            NavigationService.Navigate(new StudentDetailPage(targetStudent.Id));
        }
    }
}
