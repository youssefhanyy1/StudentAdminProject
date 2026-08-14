using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAdminProject.DTOs.Requests;
using StudentAdminProject.Requests;
using System.Security.Claims;

namespace StudentAdminProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [Authorize(Policy = "CanAccessProfile")]
        [HttpGet("{userId}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProfile(int userId)
        {
            Student student = Student.FindByUserId(userId);

            if (student == null)
                return NotFound("Student profile not found.");

            return Ok(student.SDTO);
        }

        [Authorize(Policy = "CanAccessProfile")]
        [HttpPut("{userId}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
        {
            Student student = Student.FindByUserId(userId);

            if (student == null)
                return NotFound("Student not found.");

            student.FullName = request.FullName;
            student.Email = request.Email;
            student.Department = request.Department;

            if (student.Save())
            {
                return Ok(new { message = "Data updated successfully.", data = student.SDTO });
            }

            return StatusCode(500, "An error occurred while saving data.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetAllStudents()
        {
            var students = Student.GetAllStudents();
            return Ok(students);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetStudentById(int id)
        {
            Student student = Student.Find(id);

            if (student == null)
                return NotFound($"Student with ID {id} not found.");

            return Ok(student.SDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteStudent(int id)
        {
  
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";


            Student student = Student.Find(id);
            if (student == null)
                return NotFound("Student not found.");


            if (Student.DeleteStudent(id))
            {
                _logger.LogInformation(
                    "Admin action completed. AdminId={AdminId}, Action=DeleteStudent, TargetStudentId={TargetId}, TargetEmail={TargetEmail}, IP={IP}",
                    currentAdminId,
                    student.Id,
                    student.Email,
                    ip
                );

                return Ok(new { message = "Student deleted successfully." });
            }

            return StatusCode(500, "An error occurred while deleting the student.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateStudent([FromBody] CreateStudentRequest request)
        {
            if (request == null)
                return BadRequest("Request body is empty.");

            BusinessLogicLayer.Users user = BusinessLogicLayer.Users.Find(request.UserId);

            if (user == null)
            {
                return BadRequest($"No user found in the system with UserId ({request.UserId}).");
            }

            if (!string.Equals(user.Role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"User ({request.UserId}) is not a student (Role = {user.Role}). Cannot add student data for this user.");
            }

            Student existingStudent = Student.FindByUserId(request.UserId);
            if (existingStudent != null)
            {
                return BadRequest("This user already has student data registered.");
            }

            Student newStudent = new Student
            {
                UserId = request.UserId,
                FullName = request.FullName,
                Email = request.Email,
                Department = request.Department,
                GPA = request.GPA
            };

            if (newStudent.Save())
            {
                return CreatedAtAction(nameof(GetStudentById), new { id = newStudent.Id }, newStudent.SDTO);
            }

            return StatusCode(500, "An error occurred while saving student data.");
        }
    }
}