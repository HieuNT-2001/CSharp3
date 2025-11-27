using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebAppMVC.Models.Entities
{
    public class User
    {
        public String? Name { get; set; }
        public String? Email { get; set; }
        public String? Password { get; set; }
        public double Percentage { get; set; }

        public User() { }

        public User(String name, String email)
        {
            Name = name;
            Email = email;
        }

        public User(String name, String email, String password, double percentage)
        {
            Name = name;
            Email = email;
            Password = password;
            Percentage = percentage;
        }
    }
}
