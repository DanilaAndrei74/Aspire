using Microsoft.EntityFrameworkCore;

namespace Aspire.ApiService;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Class> Grades { get; set; }
    public DbSet<StudentInClass> StudentGrades { get; set; }
    
}

public class Class
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<int> Description { get; set; }
}

public class Student
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }   
    public string Email { get; set; }  
    public DateOnly Birthday { get; set; }
}

public class StudentInClass
{
    public Guid Id { get; set; }
    public Student Student { get; set; }
    public Class Class { get; set; }
    public List<int> Grades { get; set; }
}