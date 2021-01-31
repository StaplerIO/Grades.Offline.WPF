using Grades.Offline.WPF.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grades.Offline.WPF.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=App.db");
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
        }

        public DbSet<DbClass> Classes { get; set; }

        public DbSet<DbSubject> Subjects { get; set; }

        public DbSet<DbStudent> Students { get; set; }

        public DbSet<DbExam> Exams { get; set; }
    }
}
