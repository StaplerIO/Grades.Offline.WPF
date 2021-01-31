using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.ViewModels
{
    class ExamIndexViewModel
    {
        public Guid ExamId { get; set; }

        public string Name { get; set; }

        public string DateString { get; set; }
    }
}
