using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbStudent
    {
        [Key]
        public Guid Id { get; set; }

        public int Sno { get; set; }

        public Guid ClassId { get; set; }

        public string FullName { get; set; }
    }
}
