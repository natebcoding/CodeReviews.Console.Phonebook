using Microsoft.EntityFrameworkCore;
using PhoneBook;
using System.Net.Mail;
using System.Text.RegularExpressions;

await using var db = new PhoneBookContext();
var contactHelper = new ContactHelper(db); // contactHelper reference

Console.WriteLine($"Database path: {db.DbPath}");

bool isOn = true;

while (isOn)
{
    Console.WriteLine("==== Phone Book ====");
    Console.WriteLine("1. Create a New Contact");
    Console.WriteLine("2. Update a Contact");
    Console.WriteLine("3. Delete a Contact");
    Console.WriteLine("4. Delete all Contacts");
    Console.WriteLine("5. View All Contacts");
    Console.WriteLine("6. Find a Contact");
    Console.WriteLine("Press Q to Exit");
    Console.WriteLine("=== Enter an Option Below ===");

    string input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await contactHelper.CreateContact();
            break;
        case "2":
            await contactHelper.UpdateContactById();
            break;
        case "3":
            await contactHelper.DeleteContactById();
            break;
        case "4":
            await contactHelper.DeleteAllContacts();
            break;
        case "5":
            await contactHelper.ViewAllContacts();
            break;
        case "6":
            await contactHelper.ViewContactById();
            break;
        case "Q":
        case "q":
            Console.WriteLine("Goodbye");
            isOn = false;
            break;
        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }

}

public class ContactHelper
{
    private readonly PhoneBookContext _db; // pass in the db

    public ContactHelper(PhoneBookContext db)
    {
        _db = db; // DI
    }
    public async Task CreateContact()
    {
        Console.WriteLine("Enter the contact name: ");
        string? contactName = Console.ReadLine();
        Console.WriteLine("Enter the contact email \n  Example: frodo@gmail.com");
        string? contactEmail = string.Empty;
        while (!IsValidEmail(contactEmail)) // Validate if email is correct format using System.Net.Mail
        {
            contactEmail = Console.ReadLine();
        }
        Console.WriteLine("Enter the contact phone number\n (Format - 856-234-5432): ");
        string? contactPhoneNumber = string.Empty;
        while (!IsValidPhoneNumber(contactPhoneNumber))
        { 
            contactPhoneNumber = Console.ReadLine();
        }

        _db.Add(new Contact { Name = contactName ?? "Placeholder Name", Email = contactEmail ?? "Placeholder Email", PhoneNumber = contactPhoneNumber ?? "Placeholder Phone Number" });
        await _db.SaveChangesAsync();
        

    }

    public async Task UpdateContactById()
    {
        await ViewAllContacts();

        Console.WriteLine("Enter the Contact ID for the record you want to update: ");
        int targetId = 0;
        if (int.TryParse(Console.ReadLine(), out int targetIdResult))
        {
            targetId = targetIdResult;
        }
        // Query for Contact by Id

        var existingEntity = await _db.Contacts.FindAsync(targetId); // await is used here to get the actual entity result
        

        if (existingEntity == null)
        {
            Console.WriteLine("This contact does not exist");
        }
        else
        {
            Console.WriteLine("What would you like to update? ");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Email");
            Console.WriteLine("3. Phone Number");
            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine("Enter new name:");
                    string contactNewName = Console.ReadLine();
                    existingEntity.Name = contactNewName;
                    await _db.SaveChangesAsync();
                    break;
                case "2":
                    Console.WriteLine("Enter new email:");
                    string? contactNewEmail = string.Empty;
                    while (!IsValidEmail(contactNewEmail))
                    {
                        contactNewEmail = Console.ReadLine();
                    }
                    existingEntity.Email = contactNewEmail;
                    await _db.SaveChangesAsync();
                    break;
                case "3":
                    Console.WriteLine("Enter a new Phone Number:");
                    string? contactNewPhoneNumber = string.Empty;

                    while (!IsValidPhoneNumber(contactNewPhoneNumber))
                    {
                        contactNewPhoneNumber = Console.ReadLine();
                    }
                    existingEntity.PhoneNumber = contactNewPhoneNumber;
                    await _db.SaveChangesAsync();
                    break;
                default:
                    Console.WriteLine("Incorrect input");
                    break;
                    
            }
            
        }
        
    }

    public async Task ViewAllContacts()
    {
        var allUsers = await _db.Contacts.ToListAsync();
        foreach (var user in allUsers)
        {
            Console.WriteLine($"ID = {user.ContactId} | Name: {user.Name} | Email: {user.Email} | Phone Number: {user.PhoneNumber}");
        }
    }

    public async Task ViewContactById()
    {
        Console.WriteLine("Enter the ID of the contact you want to view: ");
        int targetId = 0;
        if (int.TryParse(Console.ReadLine(), out int targetIdResult))
        {
            targetId = targetIdResult;
            var tarContact = await _db.Contacts.FindAsync(targetId);
            if (tarContact == null)
            {
                Console.WriteLine("That contact doesn't exist");
                
            }
            else
            {
                Console.WriteLine($"ID = {tarContact?.ContactId} | Name: {tarContact?.Name} | Email: {tarContact?.Email} | Phone Number: {tarContact?.PhoneNumber}");
            }
        }
        

    }

    public async Task DeleteContactById()
    {
        Console.WriteLine("Enter the ID of the contact you want to delete: ");
        int targetId = 0;
        if (int.TryParse(Console.ReadLine(), out int targetIdResult))
        {
            targetId = targetIdResult;
            var wasDeleted = await _db.Contacts.Where(c => c.ContactId == targetId).ExecuteDeleteAsync();
            if (wasDeleted == 0)
            {
                Console.WriteLine("Contact doesn't exist. Nothing was deleted.");
            }
            await _db.SaveChangesAsync();
        }
       
        
    }

    public async Task DeleteAllContacts()
    {
        var checkIfContactsExist = _db.Set<Contact>().Any();
        if (!checkIfContactsExist)
        {
            Console.WriteLine("There are no contacts to delete");
        }
        else
        {
            await _db.Contacts.ExecuteDeleteAsync();
        }
    }

    static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        try
        {
            var emailAddress = new MailAddress(email);
            return emailAddress.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    static bool IsValidPhoneNumber(string phoneNumber)
    {
        string pattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

        if (Regex.IsMatch(phoneNumber, pattern))
        {
            return true;
        }

        return false;
    }
    
    
}
