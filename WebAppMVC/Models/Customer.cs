namespace WebAppMVC.Models
{
    public class Customer
    {
        public int customerId { get; set; }
        public String name { get; set; }
        public String address { get; set; }
        public String image { get; set; }

        public Customer() { }

        public Customer(int customerId, string name, string address, string image)
        {
            this.customerId = customerId;
            this.name = name;
            this.address = address;
            this.image = image;
        }
    }
}
