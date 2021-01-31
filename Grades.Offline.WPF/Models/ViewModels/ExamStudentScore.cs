using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.ViewModels
{
    public class ExamStudentScore
    {
        public Guid StudentId { get; set; }

        // <SubjectId, SubjectScored>
        public Dictionary<Guid, decimal> SubjectScored { get; set; }

        public decimal TotalScore
        {
            get
            {
                decimal value = decimal.Zero;
                foreach (var score in SubjectScored)
                {
                    value += score.Value;
                }

                return value;
            }
        }
    }
}
