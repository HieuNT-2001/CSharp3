namespace WebAppMVC.Models
{
    public class TestCustomer
    {
        public List<Customer> _customers;

        public TestCustomer()
        {
            _customers = new List<Customer>()
            {
                new Customer() {CustomerId = 1, Name = "FpolyHN", Address = "HN", Image = "https://placehold.co/400"},
                new Customer() {CustomerId = 2, Name = "FpolyHCM", Address = "HCM", Image = "https://placehold.co/400"},
                new Customer() {CustomerId = 3, Name = "FpolyDN", Address = "DN", Image = "https://placehold.co/400"},
                new Customer() {CustomerId = 4, Name = "FpolyCT", Address = "CT", Image = "https://placehold.co/400"},
                new Customer() {CustomerId = 5, Name = "FpolyTN", Address = "TN", Image = "https://placehold.co/400"}
            };
        }
    }
}
