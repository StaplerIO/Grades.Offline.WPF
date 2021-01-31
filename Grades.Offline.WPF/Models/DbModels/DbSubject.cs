using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbSubject
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ClassId { get; set; }
    }
}
