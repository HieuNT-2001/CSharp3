using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Data;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Customers()
        {
            TestCustomer customers = new TestCustomer();
            ViewData["message"] = "This is message transfer by ViewData";
            ViewData["customer1"] = new Customer() { CustomerId = 999, Name = "customer 1", Address = "HP" };
            ViewBag.customer2 = new Customer() { CustomerId = 1000, Name = "customer 2", Address = "HD" };
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
            var customers = _context.Customers.ToList();
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
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(customer);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(long id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        [HttpGet]
        public IActionResult Delete(long id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        public IActionResult confirmDelete(long customerId)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
