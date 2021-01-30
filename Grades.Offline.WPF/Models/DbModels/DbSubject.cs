using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbSubject
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DbClass Class { get; set; }
    }
}
