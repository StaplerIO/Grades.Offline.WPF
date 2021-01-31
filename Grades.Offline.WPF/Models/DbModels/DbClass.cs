using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbClass
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
