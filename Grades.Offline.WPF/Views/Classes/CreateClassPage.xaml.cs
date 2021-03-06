﻿using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for CreateClassPage.xaml
    /// </summary>
    public partial class CreateClassPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateClassPage()
        {
            InitializeComponent();
            DataContext = this;

            _dbContext = new ApplicationDbContext();
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            // Class name is required
            if (!string.IsNullOrWhiteSpace(ClassNameTextBox.Text))
            {
                DoneButton.Visibility = Visibility.Collapsed;
                ProgressRing.Visibility = Visibility.Visible;
                if (!string.IsNullOrWhiteSpace(ClassNameTextBox.Text))
                {
                    var entity = _dbContext.Classes.Add(new DbClass
                    {
                        Name = ClassNameTextBox.Text
                    }).Entity;
                    await _dbContext.SaveChangesAsync();

                    // Tell user that the class has been created
                    var dialog = new TaskDialog
                    {
                        WindowTitle = Localization.Resources.DialogTitle,
                        MainInstruction = Localization.Resources.ClassCreateSuccess,
                        MainIcon = TaskDialogIcon.Information,
                        Content = $"{Localization.Resources.Class} {Localization.Resources.Name}: {entity.Name}",
                        ButtonStyle = TaskDialogButtonStyle.CommandLinks
                    };
                    var ignoreButton = new TaskDialogButton(Localization.Resources.Continue_creating_class);
                    var proceedButton = new TaskDialogButton(Localization.Resources.GoClass);
                    dialog.Buttons.Add(ignoreButton);
                    dialog.Buttons.Add(proceedButton);

                    var result = dialog.ShowDialog(Window.GetWindow(this));

                    if (result == proceedButton)
                    {
                        NavigationService.Navigate(new ClassDetailPage(entity.Id));
                    }

                    ClassNameTextBox.Text = string.Empty;
                }

                DoneButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;
            }
        }
    }
}
