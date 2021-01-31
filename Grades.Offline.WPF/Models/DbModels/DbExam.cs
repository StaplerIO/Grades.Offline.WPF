using Grades.Offline.WPF.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

namespace Grades.Offline.WPF.Models.DbModels
{
    public class DbExam
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ClassId { get; set; }

        public DateTime Date { get; set; }

        public string StudentScoresEncoded { get; set; }

        [NotMapped]
        public ExamScoreSummary StudentScores
        {
            get
            {
                try
                {
                    return JsonSerializer.Deserialize<ExamScoreSummary>(StudentScoresEncoded);
                }
                catch
                {
                    return new ExamScoreSummary();
                }
            }
            set
            {
                StudentScoresEncoded = JsonSerializer.Serialize(value);
            }
        }
    }
}
