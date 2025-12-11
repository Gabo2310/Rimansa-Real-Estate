
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RimansaRealEstate.Data;
using RimansaRealEstate.Models;
namespace RimansaRealEstate.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Properties.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,Location,Type,Status,Bedrooms,Bathrooms,AreaSquareMeters,ImageUrl,IsActive")] Property property)
        {
            if (ModelState.IsValid)
            {
                property.CreatedAt = DateTime.Now;
                _context.Add(property);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Propiedad creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(property);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,Location,Type,Status,Bedrooms,Bathrooms,AreaSquareMeters,ImageUrl,CreatedAt,IsActive")] Property property)
        {
            if (id != property.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    property.UpdatedAt = DateTime.Now;
                    _context.Update(property);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Propiedad actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(property.Id))
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
            return View(property);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var property = await _context.Properties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Propiedad eliminada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}