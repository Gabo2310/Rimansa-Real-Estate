using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RimansaRealEstate.Data;
using RimansaRealEstate.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text;


namespace RimansaRealEstate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _context.Properties
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(properties);
        }

        public async Task<IActionResult> Properties(string? type, string? location, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Properties.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(type))
            {
                if (Enum.TryParse<PropertyType>(type, out var propertyType))
                {
                    query = query.Where(p => p.Type == propertyType);
                }
            }

            if (!string.IsNullOrEmpty(location))
            {
                // Normalizar tanto el texto de búsqueda como los datos en BD
                var normalizedSearch = RemoveAccents(location.ToLower());

                var allProperties = await query.ToListAsync();
                var filteredProperties = allProperties
                    .Where(p => RemoveAccents(p.Location.ToLower()).Contains(normalizedSearch))
                    .ToList();

                query = _context.Properties.Where(p => filteredProperties.Select(fp => fp.Id).Contains(p.Id));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var properties = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            return View(properties);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string RemoveAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}