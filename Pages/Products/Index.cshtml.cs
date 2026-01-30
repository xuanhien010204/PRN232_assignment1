using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Pages.Products;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Product> Products { get; set; } = [];
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 4;
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    // 🔍 Search & Filter Properties
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortOrder { get; set; } = "asc";

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 4)
    {
        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 4;
        if (pageSize > 50) pageSize = 50;

        PageNumber = pageNumber;
        PageSize = pageSize;

        // Start with base query
        var query = _context.Products.AsQueryable();

        // 🔍 SEARCH: Filter by name or description
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var searchLower = Search.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchLower) || 
                p.Description.ToLower().Contains(searchLower));
        }

        // 💰 FILTER: Filter by price range
        if (MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= MinPrice.Value);
        }
        if (MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= MaxPrice.Value);
        }

        // 📊 SORT: Apply sorting
        query = SortBy?.ToLower() switch
        {
            "name" => SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Name) 
                : query.OrderBy(p => p.Name),
            "price" => SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Price) 
                : query.OrderBy(p => p.Price),
            "newest" => query.OrderByDescending(p => p.Id),
            "oldest" => query.OrderBy(p => p.Id),
            _ => query.OrderByDescending(p => p.Id)
        };

        // Get total count after filtering
        TotalItems = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

        // Apply pagination
        Products = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
