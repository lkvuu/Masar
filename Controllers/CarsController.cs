
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Masar.Models;
using Masar.Data;
using Microsoft.AspNetCore.Authorization;

public class CarsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarsController(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index()
    {
        return View(await _context.Car.ToListAsync());
    }

    public async Task<IActionResult> SearchForm()
    {
        return View();
    }

    public async Task<IActionResult> ShowSearchFormResult(string SearchCar, string location, DateTime pickupDate, DateTime dropoffDate)
    {
        if (_context.Car == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Car' is null.");
        }

        int days = (dropoffDate - pickupDate).Days;
        if (days < 1)
        {
            days = 1;
        }

        ViewBag.location = location;
        ViewBag.pickupDate = pickupDate.ToShortDateString();
        ViewBag.dropoffDate = dropoffDate.ToShortDateString();
        ViewBag.days = days;

        List<Car> cars;
        if (SearchCar == null)
        {
            cars = await _context.Car.ToListAsync();
        }
        else
        {
            cars = await _context.Car
                .Where(j => j.name.Contains(SearchCar) || j.category.Contains(SearchCar))
                .ToListAsync();
        }

        return View("Results", cars);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car
            .FirstOrDefaultAsync(m => m.id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }


    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("id,name,category,price,image")] Car car)
    {
        if (ModelState.IsValid)
        {
            _context.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(car);
    }

    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }
        return View(car);
    }


    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("id,name,category,price,image")] Car car)
    {
        if (id != car.id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(car);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(car.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(car);
    }

    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car
            .FirstOrDefaultAsync(m => m.id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var car = await _context.Car.FindAsync(id);
        if (car != null)
        {
            _context.Car.Remove(car);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CarExists(int? id)
    {
        return _context.Car.Any(e => e.id == id);
    }
}


