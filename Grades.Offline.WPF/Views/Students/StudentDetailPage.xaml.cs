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

namespace Grades.Offline.WPF.Views.Students
{
    /// <summary>
    /// Interaction logic for StudentDetailPage.xaml
    /// </summary>
    public partial class StudentDetailPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        private DbStudent Student { get; set; }

        public StudentDetailPage(Guid studentId)
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();

            Student = _dbContext.Students.FirstOrDefault(s => s.Id == studentId);

            StudentNameTextBox.Text = Student.FullName;
            StudentSnoTextBox.Text = Student.Sno.ToString();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(StudentNameTextBox.Text) && !string.IsNullOrWhiteSpace(StudentSnoTextBox.Text))
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
    }
}
