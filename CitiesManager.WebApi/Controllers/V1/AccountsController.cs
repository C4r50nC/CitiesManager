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

        [HttpPost("Register")]
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

        [HttpGet(nameof(IsEmailAlreadyRegistered))]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            if (await _userManager.FindByEmailAsync(email) == null)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> PostLogin(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("\n", ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage));
                return Problem(errorMessage);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
            if (!signInResult.Succeeded)
            {
                return Problem("Invalid email or password");
            }

            ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(loginDto.Email);
            if (applicationUser == null)
            {
                return NoContent();
            }

            return Ok(new { personName = applicationUser.PersonName, email = applicationUser.Email });
        }

        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
