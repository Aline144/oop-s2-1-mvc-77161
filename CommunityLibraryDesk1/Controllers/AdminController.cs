using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommunityLibraryDesk1.Controllers;

public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IActionResult Roles()
    {
        var roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
        return View(roles);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        return RedirectToAction(nameof(Roles));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null && role.Name != "Admin")
        {
            await _roleManager.DeleteAsync(role);
        }

        return RedirectToAction(nameof(Roles));
    }
}