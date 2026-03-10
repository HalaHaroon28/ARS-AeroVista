using AeroVista.Data;
using AeroVista.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public IActionResult Search(FlightSearchViewModel model)
        {
            return RedirectToAction("Results", "Flights", model);
        }
    }
}