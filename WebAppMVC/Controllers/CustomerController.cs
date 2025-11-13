using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class CustomerController : Controller
    {
        static List<Customer> customers = new List<Customer>(){
            new Customer {customerId = 1001, name = "A", address = "dia chi cua A", image = "https://placehold.co/100"},
            new Customer {customerId = 1002, name = "B", address = "dia chi cua B", image = "https://placehold.co/100"},
            new Customer {customerId = 1003, name = "C", address = "dia chi cua C", image = "https://placehold.co/100"},
            new Customer {customerId = 1004, name = "D", address = "dia chi cua D", image = "https://placehold.co/100"},
            new Customer {customerId = 1005, name = "E", address = "dia chi cua E", image = "https://placehold.co/100"},
        };

        public IActionResult Customers()
        {
            TestCustomer customers = new TestCustomer();
            ViewData["message"] = "This is message transfer by ViewData";
            ViewData["customer1"] = new Customer() { customerId = 999, name = "customer 1", address = "HP" };
            ViewBag.customer2 = new Customer() { customerId = 1000, name = "customer 2", address = "HD" };
            return View(customers);
        }

        [Route("test")]
        public String test(String? keyword)
        {
            String message = keyword == null ? "del co" : keyword;
            return "this is test routing " + message;
        }

        [Route("number/{number:int?}")]
        public String number(int? number)
        {
            return "The number is: " + number;
        }

        public IActionResult Index()
        {
            return View(customers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            customers.Add(customer);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Customer customer = new Customer();
            foreach (Customer item in customers)
            {
                if (item.customerId == id) { customer = item; break; }
            }
            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Customer customer = new Customer();
            foreach (Customer item in customers)
            {
                if (item.customerId == id) { customer = item; break; }
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            foreach (Customer item in customers)
            {
                if (item.customerId == customer.customerId)
                {
                    item.name = customer.name;
                    item.address = customer.address;
                    item.image = customer.image;
                    break;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Customer customer = new Customer();
            foreach (Customer item in customers)
            {
                if (item.customerId == id) { customer = item; break; }
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult confirmDelete(int customerId)
        {
            var customer = customers.FirstOrDefault(item => item.customerId == customerId);
            if (customer != null) customers.Remove(customer);
            return RedirectToAction("Index");
        }
    }
}
