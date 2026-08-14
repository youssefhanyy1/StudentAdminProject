using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using StudentAdminProject.DTOs.Auth;
using StudentAdminProject.Helpers;
using StudentAdminProject.Requests;
using System;

using LoginRequest = StudentAdminProject.DTOs.Auth.LoginRequest;

namespace StudentAdminProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("AuthLimiter")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IConfiguration config, ILogger<AuthController> logger)
        {
            _jwtService = new JwtService(config);
            _logger = logger;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning(
                    "Failed login attempt (email not found). User name={Username}, IP={IP}",
                    request.Username,
                    ip
                );
                return BadRequest("اسم المستخدم وكلمة المرور مطلوبان.");
            }
            BusinessLogicLayer.Users user = BusinessLogicLayer.Users.FindByUsername(request.Username);

            if (user == null || !user.VerifyPassword(request.Password))
            {
                _logger.LogWarning(
                    "Failed login attempt (email not found). User name={Username}, IP={IP}",
                    request.Username,
                    ip
                );
                return Unauthorized("اسم المستخدم أو كلمة المرور غير صحيحة.");
            }


            string accessToken = _jwtService.GenerateAccessToken(user);
            string refreshToken = _jwtService.GenerateRefreshToken();
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(60);


            var tokenResponse = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
                 _logger.LogInformation(
                  "Successful login. UserId={Id}, Email={Username}, IP={IP}",
                  user.Id,
                  user.Username,
                  ip
                 );

            return Ok(new
            {
                message = "تم تسجيل الدخول بنجاح",
                tokens = tokenResponse,
                user = new
                {
                    userId = user.Id,
                    username = user.Username,
                    role = user.Role
                }
            });
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] RefreshRequest request)
        {

            if (request == null || string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.RefreshToken))
            {

                return BadRequest("الـ AccessToken والـ RefreshToken مطلوبان.");
            }

            try
            {

                var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
                if (principal == null)
                {
                    return BadRequest("التوكن غير صالح.");
                }

                var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {

                    return BadRequest("بيانات التوكن غير صحيحة.");
                }


                BusinessLogicLayer.Users user = BusinessLogicLayer.Users.Find(userId);
                if (user == null)
                {
                    return Unauthorized("المستخدم غير موجود بالسيستم.");
                }


                string newAccessToken = _jwtService.GenerateAccessToken(user);
                string newRefreshToken = _jwtService.GenerateRefreshToken();
                DateTime expiresAt = DateTime.UtcNow.AddMinutes(60);

                var tokenResponse = new TokenResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = expiresAt
                };

                return Ok(new
                {
                    message = "تم تجديد التوكن بنجاح",
                    tokens = tokenResponse
                });
            }
            catch (Exception)
            {
                return BadRequest("حدث خطأ أثناء تجديد التوكن، يرجى إعادة تسجيل الدخول.");
            }
        }
        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {

            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("الـ RefreshToken مطلوب لإتمام عملية تسجيل الخروج.");
            }

            return Ok(new
            {
                message = "تم تسجيل الخروج بنجاح."
            });
        }
    }
}