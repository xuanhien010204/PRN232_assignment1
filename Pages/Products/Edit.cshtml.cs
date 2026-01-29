using Assignment1.Data;
using Assignment1.Models;
using Assignment1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Pages.Products;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public EditModel(AppDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    [BindProperty]
    public Product Product { get; set; } = null!;

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return Page();
        }

        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Remove ImageUrl from validation since it might be null or kept from previous
        ModelState.Remove("Product.ImageUrl");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingProduct = await _context.Products.FindAsync(Product.Id);

        if (existingProduct == null)
        {
            return NotFound();
        }

        // Upload new image if provided
        if (ImageFile != null && ImageFile.Length > 0)
        {
            try
            {
                existingProduct.ImageUrl = await _cloudinaryService.UploadImageAsync(ImageFile);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ImageFile", $"Image upload failed: {ex.Message}");
                return Page();
            }
        }

        // Update other fields
        existingProduct.Name = Product.Name;
        existingProduct.Description = Product.Description;
        existingProduct.Price = Product.Price;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(Product.Id))
            {
                return NotFound();
            }
            throw;
        }

        TempData["SuccessMessage"] = "Product updated successfully!";
        return RedirectToPage("Index");
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
