using AeroVista.Data;
using AeroVista.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AeroVista.Controllers
{
    [Authorize(Roles = "Admin")] // Sirf admin access kar sakta hai
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; // Apna database context

        // Single constructor with both dependencies
        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //Dashboard 
        public IActionResult AdminDashboard()
        {
            return View();
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Users");
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["Success"] = $"User {user.Email} deleted successfully";
                }
                else
                {
                    TempData["Error"] = $"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                }

                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Users");
            }
        }
        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Get user claims (optional)
            var claims = await _userManager.GetClaimsAsync(user);

            // Create a view model with all user details
            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                Roles = roles.ToList(),
                Claims = claims.Select(c => $"{c.Type}: {c.Value}").ToList(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Sex = user.Sex,
                DateOfBirth = user.DateOfBirth
                //CreatedAt = user.CreatedAt,
                // LastLogin = user.LastLogin
            };

            return View(model);
        }

        //get cities 
        [HttpGet]
        public async Task<IActionResult> Cities()
        {
            var cities = await _context.Cities.ToListAsync();
            return View(cities);
        }

        public async Task<IActionResult> AddCities()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCities(City city)
        {
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
            return RedirectToAction("Cities");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCities(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCities(City city)
        {
            if (ModelState.IsValid)
            {
                _context.Cities.Update(city);
                await _context.SaveChangesAsync();

                return RedirectToAction("Cities");
            }

            return View(city);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCities(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCities(City city)
        {
            if (ModelState.IsValid)
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();

                return RedirectToAction("Cities");
            }

            return View(city);
        }

        //Flights 
        [HttpGet]
        public async Task<IActionResult> Flights()
        {
            // Fetch cities for dropdowns
            var cities = await _context.Cities.ToListAsync();
            ViewBag.FromCityId = new SelectList(cities, "CityId", "CityName");
            ViewBag.ToCityId = new SelectList(cities, "CityId", "CityName");

            // Fetch all flights with city info
            var flights = await _context.Flights
                .Include(f => f.FromCity)
                .Include(f => f.ToCity)
                .ToListAsync();

            return View(flights); // Pass IEnumerable<Flights> to view
        }

        [HttpGet]
        public async Task<IActionResult> AddFlight()
        {
            var cities = await _context.Cities.ToListAsync();
            ViewBag.FromCityId = new SelectList(cities, "CityId", "CityName");
            ViewBag.ToCityId = new SelectList(cities, "CityId", "CityName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFlight(Flights flight)
        {
            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction("Flights");          
        }

        [HttpGet]
        public async Task<IActionResult> UpdateFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            var cities = await _context.Cities.ToListAsync();
            ViewBag.FromCityId = new SelectList(cities, "CityId", "CityName", flight.FromCityId);
            ViewBag.ToCityId = new SelectList(cities, "CityId", "CityName", flight.ToCityId);

            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFlight(Flights flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Update(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction("Flights");
            }

            var cities = await _context.Cities.ToListAsync();
            ViewBag.FromCityId = new SelectList(cities, "CityId", "CityName", flight.FromCityId);
            ViewBag.ToCityId = new SelectList(cities, "CityId", "CityName", flight.ToCityId);

            return View(flight);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.FromCity)
                .Include(f => f.ToCity)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();

            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFlightConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Flights");
        }

        [HttpGet]
        public async Task<IActionResult> FlightDetails(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.FromCity)
                .Include(f => f.ToCity)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();

            return View(flight);
        }
    }
}