using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAdminProject.DTOs.Requests;
using StudentAdminProject.Requests;

namespace StudentAdminProject.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
  
        [Authorize(Policy = "CanAccessProfile")]
        [HttpGet("{userId}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProfile(int userId)
        {
            Student student = Student.FindByUserId(userId);

            if (student == null)
                return NotFound("لم يتم العثور على بيانات الطالب.");

            return Ok(student.SDTO);
        }


        [Authorize(Policy = "CanAccessProfile")]
        [HttpPut("{userId}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
        {
            Student student = Student.FindByUserId(userId);

            if (student == null)
                return NotFound("الطالب غير موجود.");

            student.FullName = request.FullName;
            student.Email = request.Email;
            student.Department = request.Department;

            if (student.Save())
            {
                return Ok(new { message = "تم تحديث البيانات بنجاح", data = student.SDTO });
            }

            return StatusCode(500, "حدث خطأ أثناء حفظ البيانات.");
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
                return NotFound($"الطالب رقم {id} غير موجود.");

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
            Student student = Student.Find(id);

            if (student == null)
                return NotFound("الطالب غير موجود.");

            if (Student.DeleteStudent(id))
            {
                return Ok(new { message = "تم حذف الطالب بنجاح." });
            }

            return StatusCode(500, "حدث خطأ أثناء الحذف.");
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
                return BadRequest("بيانات الطلب فارغة.");

            BusinessLogicLayer.User user = BusinessLogicLayer.User.Find(request.UserId);

            if (user == null)
            {
                return BadRequest($"لا يوجد مستخدم في النظام بهذا الـ UserId ({request.UserId}).");
            }

            if (!string.Equals(user.Role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"المستخدم رقم ({request.UserId}) ليس من نوع طالب (Role = {user.Role}). لا يمكن إضافة بيانات طالب له.");
            }

            Student existingStudent = Student.FindByUserId(request.UserId);
            if (existingStudent != null)
            {
                return BadRequest("هذا المستخدم مسجل له بيانات طالب بالفعل.");
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

            return StatusCode(500, "حدث خطأ أثناء حفظ بيانات الطالب.");
        }
    }
}