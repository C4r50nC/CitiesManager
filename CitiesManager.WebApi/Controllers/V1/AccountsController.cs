using Asp.Versioning;
using CitiesManager.Core.Dto;
using CitiesManager.Core.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class AccountsController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("\n", ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage));
                return Problem(errorMessage);
            }

            ApplicationUser applicationUser = new()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.Email,
                PersonName = registerDto.PersonName,
            };

            IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, registerDto.Password);
            if (!identityResult.Succeeded)
            {
                string errorMessage = string.Join("\n", identityResult.Errors.Select(error => error.Description));
                return Problem(errorMessage);
            }

            await _signInManager.SignInAsync(applicationUser, false);

            return Ok(applicationUser);
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            if (await _userManager.FindByEmailAsync(email) == null)
            {
                return Ok(false);
            }

            return Ok(true);
        }
    }
}
