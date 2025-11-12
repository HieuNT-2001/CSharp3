using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            Customer customer = new Customer();
            return View(customer);
        }

        public IActionResult Customers()
        {
            TestCustomer customers = new TestCustomer();
            ViewData["message"] = "This is message transfer by ViewData";
            ViewData["customer1"] = new Customer() { customerId = 999, name = "customer 1", address = "HP" };
            ViewBag.customer2 = new Customer() { customerId = 1000, name = "customer 2", address = "HD" };
            return View(customers);
        }
    }
}
