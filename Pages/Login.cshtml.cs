using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages
{
  public class LoginModel(SignInManager<ApplicationUser> signInManager) : PageModel
  {
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public string ErrorMessage { get; set; } = string.Empty;

    public class InputModel
    {
      public string Email { get; set; } = null!;
      public string Password { get; set; } = null!;
    }

    public IActionResult OnGet()
    {
      if (User.Identity!.IsAuthenticated)
        return RedirectToPage("/Index");

      return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid) return Page();

      var result = await signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, false);

      if (result.Succeeded)
        return RedirectToPage("/Index"); 
      else
      {
        ErrorMessage = "Usuario o contraseña incorrecta";
        return Page();
      }
    }
  }
}
