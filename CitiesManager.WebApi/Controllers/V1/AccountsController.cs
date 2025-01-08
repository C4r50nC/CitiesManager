using Asp.Versioning;
using CitiesManager.Core.Dto;
using CitiesManager.Core.Identities;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebApi.Controllers.V1
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [AllowAnonymous]
    public class AccountsController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="jwtService"></param>
        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
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
            AuthenticationResponseDto authenticationResponse = _jwtService.CreateJwtToken(applicationUser);

            applicationUser.RefreshToken = authenticationResponse.RefreshToken;
            applicationUser.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
            await _userManager.UpdateAsync(applicationUser);

            return Ok(authenticationResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet(nameof(IsEmailAlreadyRegistered))]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            if (await _userManager.FindByEmailAsync(email) == null)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
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

            AuthenticationResponseDto authenticationResponse = _jwtService.CreateJwtToken(applicationUser);

            applicationUser.RefreshToken = authenticationResponse.RefreshToken;
            applicationUser.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
            await _userManager.UpdateAsync(applicationUser);

            return Ok(authenticationResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
