using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "abdo kabaka",
                    Email = "abdokabaka@gmail.com",
                    Department = Dept.IT,
                    PhotoPath ="what the fuck am I supposed to represent"
                });
     
    }
    }
}
