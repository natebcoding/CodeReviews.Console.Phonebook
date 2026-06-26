using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhoneBook;

using var db = new CourseDirectory();

Console.WriteLine($"Database path: {db.DbPath}");

// Create
Console.WriteLine("Inserting a new course");
db.Add(new Course {CourseName = "CS-50"});
await db.SaveChangesAsync();

// Read 
Console.WriteLine("Querying for a course");
var targetCourse = await db.CourseDir.OrderBy(c => c.CourseId).FirstAsync();

// Update
Console.WriteLine("Updating a course");
targetCourse.CourseName = "Game Development 101";
await db.SaveChangesAsync();

// Delete
Console.WriteLine("Delete the course");
db.Remove(targetCourse);
await db.SaveChangesAsync();


