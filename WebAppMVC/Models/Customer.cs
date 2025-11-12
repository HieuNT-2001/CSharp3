namespace WebAppMVC.Models
{
    public class Customer
    {
        public int customerId { get; set; }
        public String name { get; set; }
        public String address { get; set; }
        public String image { get; set; }

        public Customer()
        {
            customerId = 1;
            name = "Thepv";
            address = "Tphcm";
        }
    }
}
