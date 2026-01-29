using Assignment1.Data;
using Assignment1.Models;
using Assignment1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assignment1.Pages.Products;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public CreateModel(AppDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    [BindProperty]
    public Product Product { get; set; } = new();

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Remove ImageUrl from validation since it's set programmatically
        ModelState.Remove("Product.ImageUrl");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Upload image to Cloudinary if provided
        if (ImageFile != null && ImageFile.Length > 0)
        {
            try
            {
                Product.ImageUrl = await _cloudinaryService.UploadImageAsync(ImageFile);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ImageFile", $"Image upload failed: {ex.Message}");
                return Page();
            }
        }

        _context.Products.Add(Product);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Product created successfully!";
        return RedirectToPage("Index");
    }
}
