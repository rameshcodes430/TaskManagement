using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for registration: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Register attempt for username: {Username}", model.Username);
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Username} registered successfully", model.Username);
                return Ok(new { message = "User registered successfully" });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger.LogWarning("Registration failed for {Username}: {Errors}", model.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Login attempt for username: {Username}", model.Username);
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userName = user.UserName ?? string.Empty;
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT Key is not configured properly.");
                    return BadRequest(new { message = "JWT Key is not configured properly." });
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.UtcNow.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                _logger.LogInformation("User {Username} logged in successfully", model.Username);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            _logger.LogWarning("Invalid login attempt for username: {Username}", model.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }
    }
}
