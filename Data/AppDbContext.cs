using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //只允許get，避免外部修改DbSet

        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<Grammar> Grammars => Set<Grammar>();
        public DbSet<Vocabulary> Vocabularies => Set<Vocabulary>();
        public DbSet<GrammarExample> GrammarExamples => Set<GrammarExample>();
        public DbSet<QuizAttempt> QuizAttempt => Set<QuizAttempt>();
        public DbSet<QuizAnswer> QuizAnswer => Set<QuizAnswer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Lesson 跟Vocabulary 一對多關聯
            modelBuilder.Entity<Vocabulary>()
                .HasOne(v => v.Lesson)
                .WithMany(l => l.Vocabularies)
                .HasForeignKey(v => v.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            //Lesson跟Grammer 一對多關聯
            modelBuilder.Entity<Grammar>()
                .HasOne(g => g.Lesson)
                .WithMany(l => l.Grammars)
                .HasForeignKey(g => g.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            //Grammer跟GrammerExample 一對多關聯
            modelBuilder.Entity<GrammarExample>()
                .HasOne(e => e.Grammar)
                .WithMany(g => g.GrammarExamples)
                .HasForeignKey(e => e.GrammarId)
                .OnDelete(DeleteBehavior.Cascade);

            //Lesson跟QuizAttempt一對多關聯
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(q => q.Lesson)
                .WithMany(l => l.QuizAttempts)
                .HasForeignKey(q => q.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            //QuizAttempt跟QuizAnswer一對多關聯
            modelBuilder.Entity<QuizAnswer>()
                .HasOne(a => a.QuizAttempt)
                .WithMany(q => q.QuizAnswers)
                .HasForeignKey(a => a.QuizAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            //Vocabulary跟QuizAnswer一對多關聯
            modelBuilder.Entity<QuizAnswer>()
                .HasOne(a => a.Vocabulary)
                .WithMany(v => v.QuizAnswers)
                .HasForeignKey(a => a.VocabularyId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}