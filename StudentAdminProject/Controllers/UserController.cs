using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using StudentAdminProject.DTOs.Requests;
using System.Security.Claims;

namespace StudentAdminProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            BusinessLogicLayer.Users user = BusinessLogicLayer.Users.Find(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Role
            });
        }

        [HttpPut("{id}/password")]
        public IActionResult UpdatePassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (request == null || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("New password is required.");
            }

            BusinessLogicLayer.Users user = BusinessLogicLayer.Users.Find(id);
            if (user == null)
                return NotFound("User not found.");

            user.SetPassword(request.NewPassword);

            if (user.Save())
            {
                _logger.LogInformation(
                   "Admin action completed. AdminId={AdminId}, Action=DeleteStudent, TargetStudentId={TargetId}, TargetEmail={TargetEmail}, IP={IP}",
                   currentAdminId,
                   user.Id,
                   user.Username,
                   ip
               );
                return Ok(new { message = "Password updated successfully." });
            }

            return StatusCode(500, "An error occurred while updating the password.");
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            if (BusinessLogicLayer.Users.FindByUsername(request.Username) != null)
            {
                return BadRequest("Username already exists.");
            }

            BusinessLogicLayer.Users newUser = new BusinessLogicLayer.Users();
            newUser.Username = request.Username;
            newUser.Role = request.Role ?? "Student";

            newUser.SetPassword(request.Password);

            if (newUser.Save())
            {
                _logger.LogInformation(
                 "Admin action completed. AdminId={currentAdminId}, Action=DeleteStudent, IP={IP}",
                 currentAdminId,
                 ip
             );
                return Ok(new
                {
                    message = "User created successfully.",
                    userId = newUser.Id
                });
            }

            return StatusCode(500, "An error occurred while saving the user.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            BusinessLogicLayer.Users user = BusinessLogicLayer.Users.Find(id);
            if (user == null)
                return NotFound("User not found.");
            if (Student.DeleteStudent(id))
            {
                return Ok(new { message = "Student deleted successfully." });
            }
            if (BusinessLogicLayer.Users.DeleteUser(id))
            {
                _logger.LogInformation(
                    "Admin action completed. AdminId={AdminId}, Action=DeleteStudent, TargetStudentId={TargetId}, TargetEmail={TargetEmail}, IP={IP}",
                    currentAdminId,
                    user.Id,
                    user.Username,
                    ip
                );
                return Ok(new { message = "User deleted successfully." });
            }
     
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }
}