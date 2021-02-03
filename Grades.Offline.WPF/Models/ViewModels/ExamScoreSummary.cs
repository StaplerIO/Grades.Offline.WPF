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

        public decimal GetSubjectAverageScore(Guid subjectId)
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

        public decimal TotalAverageScore()
        {
            decimal averageTotalScore = 0;
            StudentScores.ForEach(s =>
            {
                averageTotalScore += s.TotalScore;
            });
            return averageTotalScore /= StudentScores.Count;
        }

        public KeyValuePair<Guid, decimal> GetLowestScoreBySubject(Guid subjectId)
        {
            var currentLowset = new KeyValuePair<Guid, decimal>(Guid.Empty, decimal.MaxValue);
            StudentScores.ForEach(s =>
            {
                var subjectScore = s.SubjectScored.TryGetValue(subjectId, out decimal score);
                if(score < currentLowset.Value)
                {
                    currentLowset = new KeyValuePair<Guid, decimal>(s.StudentId, score);
                }
            });

            return currentLowset;
        }

        public KeyValuePair<Guid, decimal> GetHighestScoreBySubject(Guid subjectId)
        {
            var currentHighest = new KeyValuePair<Guid, decimal>(Guid.Empty, decimal.MinValue);
            StudentScores.ForEach(s =>
            {
                var subjectScore = s.SubjectScored.TryGetValue(subjectId, out decimal score);
                if (score > currentHighest.Value)
                {
                    currentHighest = new KeyValuePair<Guid, decimal>(s.StudentId, score);
                }
            });

            return currentHighest;
        }
    }
}
