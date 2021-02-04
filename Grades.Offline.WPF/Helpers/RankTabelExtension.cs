using Grades.Offline.WPF.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Grades.Offline.WPF.Helpers
{
    class RankTabelExtension
    {
        public static DataTable GetUIFriendlyRankTableByExamId(Guid examId)
        {
            var dbContext = new ApplicationDbContext();
            var dataTable = new DataTable();
            var exam = dbContext.Exams.FirstOrDefault(e => e.Id == examId);

            #region DataTableColumns
            dataTable.Columns.Add("Student");
            var examSummary = exam.StudentScores;
            examSummary.SubjectScores.ForEach(subjectScore =>
            {
                var subject = dbContext.Subjects.FirstOrDefault(s => s.Id == subjectScore.SubjectId);
                dataTable.Columns.Add($"{subject.Name} ({subjectScore.TotalScore})");
            });
            dataTable.Columns.Add($"Total ({examSummary.TotalScore})");
            #endregion

            #region DataTableRows
            examSummary.StudentScores.ForEach(studentScore =>
            {
                var student = dbContext.Students.FirstOrDefault(s => s.Id == studentScore.StudentId);

                var rowData = new List<object>();

                rowData.Add($"({student.Sno}) {student.FullName}");
                for (int i = 0; i < studentScore.SubjectScored.Count; i++)
                {
                    var subject = examSummary.SubjectScores[i];
                    var subjectScore = studentScore.SubjectScored[subject.SubjectId];

                    // Calculate correct rate by saving 2 numbers after decimal point
                    rowData.Add($"{subjectScore} ({subjectScore / subject.TotalScore * 100:#.##}%)");
                }
                rowData.Add($"{studentScore.TotalScore} ({studentScore.TotalScore / examSummary.TotalScore * 100:#.##}%)");

                dataTable.Rows.Add(rowData.ToArray());
            });

            // _dataTable.Rows.Add(AverageScoreRowData(examSummary));

            #endregion

            return dataTable;
        }
    }
}
