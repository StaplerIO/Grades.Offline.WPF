using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
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

        public SubjectDetailPage(Guid subjectId)
        {
            _dbContext = new ApplicationDbContext();
            Subject = _dbContext.Subjects.FirstOrDefault(s => s.Id == subjectId);

            InitializeComponent();
            DataContext = this;

            SubjectNameTextBox.Text = Subject.Name;
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
    }
}
