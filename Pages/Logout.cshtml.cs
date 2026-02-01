using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages
{
  public class LogoutModel(SignInManager<ApplicationUser> signInManager) : PageModel
  {
    public async Task<IActionResult> OnPost()
    {
      await signInManager.SignOutAsync();
      return RedirectToPage("/Login");
    }
  }
}
