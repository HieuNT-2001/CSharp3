namespace WebAppMVC.Models
{
    public class TestCustomer
    {
        public List<Customer> _customers;

        public TestCustomer()
        {
            _customers = new List<Customer>()
            {
                new Customer() {customerId = 1, name = "FpolyHN", address = "HN", image = "https://placehold.co/400"},
                new Customer() {customerId = 2, name = "FpolyHCM", address = "HCM", image = "https://placehold.co/400"},
                new Customer() {customerId = 3, name = "FpolyDN", address = "DN", image = "https://placehold.co/400"},
                new Customer() {customerId = 4, name = "FpolyCT", address = "CT", image = "https://placehold.co/400"},
                new Customer() {customerId = 5, name = "FpolyTN", address = "TN", image = "https://placehold.co/400"}
            };
        }
    }
}
