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
    public int PageSize { get; set; } = 4; // Chỉ hiển thị 4 sản phẩm mỗi trang
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 4)
    {
        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 4; // Mặc định 4 sản phẩm
        if (pageSize > 50) pageSize = 50; // Tối đa 50 sản phẩm mỗi trang

        PageNumber = pageNumber;
        PageSize = pageSize;

        TotalItems = await _context.Products.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

        Products = await _context.Products
            .OrderByDescending(p => p.Id)
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
