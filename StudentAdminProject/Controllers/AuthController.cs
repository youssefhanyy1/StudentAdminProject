using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
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
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(IConfiguration config)
        {
            _jwtService = new JwtService(config);
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("اسم المستخدم وكلمة المرور مطلوبان.");
            }
            BusinessLogicLayer.User user = BusinessLogicLayer.User.FindByUsername(request.Username);

            if (user == null || !user.VerifyPassword(request.Password))
            {
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


                BusinessLogicLayer.User user = BusinessLogicLayer.User.Find(userId);
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