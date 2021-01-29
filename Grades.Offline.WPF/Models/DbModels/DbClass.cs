using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbClass
    {
        public Guid Id { get; set; }

        public IEnumerable<DbStudent> Students { get; set; }

        public string Name { get; set; }
    }
}
