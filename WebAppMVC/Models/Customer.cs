namespace WebAppMVC.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public Customer() { }

        public Customer(int customerId, string name, string address, string image)
        {
            this.CustomerId = customerId;
            this.Name = name;
            this.Address = address;
            this.Image = image;
        }
    }
}
