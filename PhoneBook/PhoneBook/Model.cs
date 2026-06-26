using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PhoneBook;

// Note to self: In C#, class names can be anything, the class name below doesn't need to match the filename like in Java.


// Entity = CourseDirectory
// CourseDirectory has Course(s)

public class CourseDirectory : DbContext
{
    public DbSet<Course> CourseDir { get; set; }
    
    public string DbPath { get;  }

    public CourseDirectory()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Students.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source = {DbPath}");
}

public class Course
{
    public int CourseId { get; set; }
    [Required] public string CourseName { get; set; } 
    
}