using AeroVista.Data;
using AeroVista.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace AirlineSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index()
        {
            var citiesList = await _context.Cities.ToListAsync();

            var model = new FlightSearchViewModel
            {
                Cities = citiesList
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                await _context.Feedbacks.AddAsync(feedback);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thank you for your feedback!";
                return RedirectToAction("ContactUs");
            }
            return View(feedback);
        }

        public async Task<IActionResult> AboutUs()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(FlightSearchViewModel model)
        {
            return RedirectToAction("Results", "Flights", model);
        }
    }
}