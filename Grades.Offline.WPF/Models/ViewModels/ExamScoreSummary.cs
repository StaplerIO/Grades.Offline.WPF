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

        public decimal AverageScore(Guid subjectId)
        {
            decimal average = 0;

            StudentScores.ForEach(s =>
            {
                s.SubjectScored.TryGetValue(subjectId, out decimal value);
                average += value;
            });

            average /= StudentScores.Count;

            return average;
        }
    }
}
