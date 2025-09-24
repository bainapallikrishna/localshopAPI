using LocalShop.Models.DTOs;
using LocalShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LocalShop.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IMemoryCache _memoryCache;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IMemoryCache memoryCache)
        {
            _authService = authService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _authService.AuthenticateAsync(request);
                if (response == null)
                    return Unauthorized(new { message = "Invalid username or password" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("validate-token")]
        [AllowAnonymous]
        public IActionResult ValidateToken([FromBody] TokenValidateRequest? request)
        {
            try
            {
                string? token = null;
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
                if (string.IsNullOrWhiteSpace(token))
                {
                    token = request?.Token;
                }
                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest(new { message = "Token is required" });
                }

                var jwtService = HttpContext.RequestServices.GetRequiredService<IJwtService>();
                var principal = jwtService.ValidateToken(token);
                if (principal == null)
                {
                    return Ok(new TokenValidateResponse { IsValid = false });
                }

                var username = principal.FindFirstValue(ClaimTypes.Name);
                var email = principal.FindFirstValue(ClaimTypes.Email);
                var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                DateTime? expiresAt = null;
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                if (jwt.ValidTo != DateTime.MinValue)
                {
                    expiresAt = jwt.ValidTo.ToUniversalTime();
                }

                return Ok(new TokenValidateResponse
                {
                    IsValid = true,
                    Username = username,
                    Email = email,
                    Roles = roles,
                    ExpiresAt = expiresAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("request-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Username))
                    return BadRequest(new { message = "Username is required" });

                var ok = await _authService.RequestOtpAsync(request.Username);
                if (!ok) return NotFound(new { message = "User not found" });

                // In dev, you might want to return OTP for ease of testing, but we won't expose it here.
                return Ok(new { message = "OTP sent if user exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during OTP request");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerify request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Otp))
                    return BadRequest(new { message = "Username and OTP are required" });

                var response = await _authService.VerifyOtpAsync(request.Username, request.Otp);
                if (response == null)
                    return Unauthorized(new { message = "Invalid or expired OTP" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during OTP verification");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
       
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _authService.RegisterAsync(request);
                if (!success)
                    return BadRequest(new { message = "Registration failed. User might already exist." });

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _authService.RegisterAsync(request, "Admin");
                if (!success)
                    return BadRequest(new { message = "Admin registration failed. User might already exist." });

                return Ok(new { message = "Admin user registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during admin registration");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
} 