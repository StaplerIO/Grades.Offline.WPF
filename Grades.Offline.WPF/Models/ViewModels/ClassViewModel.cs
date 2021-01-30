using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.ViewModels
{
    class ClassViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int StudentCount { get; set; } = -1;
    }
}
