using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Models.ViewModels
{
    public class ExamScoreSummary
    {
        public decimal TotalScore
        {
            get
            {
                decimal total = 0;
                SubjectScores.ForEach(s =>
                {
                    total += s.TotalScore;
                });

                return total;
            }
        }

        public List<ExamSubject> SubjectScores { get; set; }

        public List<ExamStudentScore> StudentScores { get; set; }
    }
}
