using Microsoft.AspNetCore.Mvc;
using RetailProject.Models;
using RetailProject.Services;

namespace RetailProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableStorageService _tableService;

        public CustomerController(TableStorageService tableService)
        {
            _tableService = tableService;
        }

        // 1. LIST (Index)
        public async Task<IActionResult> Index()
        {
            var customers = await _tableService.GetCustomersAsync();
            return View(customers);
        }

        // 2. CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREATE (POST)
        [HttpPost]
        public async Task<IActionResult> Create(string firstName, string lastName, string email)
        {
            var newCustomer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                DateRegistered = DateTime.UtcNow
            };

            await _tableService.SaveCustomerAsync(newCustomer);
            return RedirectToAction("Index");
        }

        // 4. DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(string id) // 
        {
            await _tableService.DeleteCustomerAsync(id);
            return RedirectToAction("Index");
        }
    }
}