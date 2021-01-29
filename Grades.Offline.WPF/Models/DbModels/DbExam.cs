using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbExam
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }
    }
}
