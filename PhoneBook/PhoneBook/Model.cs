using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PhoneBook;

public class PhoneBookContext : DbContext
{
    
    public DbSet<Contact> Contacts { get; set; }
    
    public string DbPath { get;  }

    public PhoneBookContext()
    {
        DbPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "phonebook.db");
        
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source = {DbPath}").UseSeeding(((context, _) =>
        {
            var checkIfExists = context.Set<Contact>().FirstOrDefault(c => c.ContactId == 1);
            if (checkIfExists == null) // if there's no data, proceed
            {
                var testContact = new Contact
                    { Name = "Frodo", Email = "frodo@theshire.com", PhoneNumber = "134-435-8899" };
                context.Add(testContact);
                context.SaveChanges();
            }


        }));


}

public class Contact
{
    public int ContactId { get; set; }

    [Required] [StringLength(256)] public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number format")]
    [StringLength(10)]
    public string PhoneNumber { get; set; } = string.Empty;
}


