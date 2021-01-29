using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbStudent
    {
        public Guid Id { get; set; }

        public int Sno { get; set; }

        public DbClass Class { get; set; }

        public string FullName { get; set; }
    }
}
