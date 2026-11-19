using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartDiscount.Identity.API.Models;

namespace SmartDiscount.Identity.API.Quickstart.User;

[ApiController]
[Route("api/users")]
[AllowAnonymous]
public class UsersApiController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersApiController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{guid}")]
    public async Task<IActionResult> GetUserById(string guid)
    {
        var user = await _userManager.FindByIdAsync(guid);

        if (user == null)
            return NotFound();

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            name = user.Name,
            lastName = user.LastName,
            street = user.Street,
            city = user.City,
            state = user.State,
            country = user.Country,
            zipCode = user.ZipCode
        });
    }
}