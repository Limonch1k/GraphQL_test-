using Microsoft.EntityFrameworkCore;
using ModelsDb;

namespace DbContexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<TaskDb> Tasks { get; set; }
    public DbSet<UserDb> Users { get; set; }
    
    public DbSet<TaskAssignedDb> TasksAssigned { get; set;}

    public DbSet<TaskStatusDb> TaskStatuses { get; set; }

    public DbSet<UserRoleDb> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskDb>(entity =>
        {
            entity.ToTable("Task");

            entity.HasKey(t => t.Id);   // Явно указать первичный ключ
        });

        modelBuilder.Entity<UserDb>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(t => t.Id);   // Явно указать первичный ключ
        });

        modelBuilder.Entity<TaskAssignedDb>(entity =>
        {
            entity.ToTable("TaskAssigned");

            entity.HasKey(t => new { t.TaskId, t.UserId });
        });

        modelBuilder.Entity<TaskStatusDb>(entity =>
        {
            entity.ToTable("TaskStatus");

            entity.HasKey(t => t.StatusId);
        });

        modelBuilder.Entity<UserRoleDb>(entity =>
        {
            entity.ToTable("UserRole");

            entity.HasKey(t => t.Id);
        });

        modelBuilder.Entity<TaskDb>()
        .HasOne(t => t.taskStatus)
        .WithMany(t => t.tasks)
        .HasForeignKey(t => t.Status);

        modelBuilder.Entity<TaskDb>()
        .HasOne(t => t.user)
        .WithMany(t => t.tasks)
        .HasForeignKey(t => t.CreatedBy);

        modelBuilder.Entity<TaskAssignedDb>()
        .HasOne(t => t.task)
        .WithMany(t => t.taskAssigned)
        .HasForeignKey(t => t.TaskId);

        modelBuilder.Entity<TaskAssignedDb>()
        .HasOne(t => t.user)
        .WithMany(t => t.taskAssigneds)
        .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<UserDb>()
        .HasOne(t => t.userRole)
        .WithMany(t => t.users)
        .HasForeignKey(t => t.Role);

    }
}