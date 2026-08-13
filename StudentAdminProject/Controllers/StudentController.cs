using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;

namespace StudentAdminProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        // 1. عرض بيانات الطالب (بناءً على الـ userId اللي مبعوت في الـ Route)
        [HttpGet("{userId}/profile")]
        public IActionResult GetProfile(int userId)
        {
            // استخدام دالة FindByUserId للبحث
            Student student = Student.FindByUserId(userId);

            if (student == null)
                return NotFound("لم يتم العثور على بيانات الطالب.");

            return Ok(student.SDTO); // نرجع البيانات في شكل JSON
        }

        // 2. تحديث بيانات الطالب
        [HttpPut("{userId}/profile")]
        public IActionResult UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
        {
            // نجيب بيانات الطالب الحالية من الداتابيز
            Student student = Student.FindByUserId(userId);

            if (student == null)
                return NotFound("الطالب غير موجود.");

            // نحدث الخصائص بالبيانات الجديدة المبعوتة
            student.FullName = request.FullName;
            student.Email = request.Email;
            student.Department = request.Department;

            // استخدام دالة Save اللي بتعمل Update
            if (student.Save())
            {
                return Ok(new { message = "تم تحديث البيانات بنجاح", data = student.SDTO });
            }

            return StatusCode(500, "حدث خطأ أثناء حفظ البيانات.");
        }

        // 3. عرض كل الطلاب 
        [HttpGet("all")]
        public IActionResult GetAllStudents()
        {
            var students = Student.GetAllStudents();
            return Ok(students);
        }

    
        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            Student student = Student.Find(id);

            if (student == null)
                return NotFound($"الطالب رقم {id} غير موجود.");

            return Ok(student.SDTO);
        }


        [HttpDelete("{id}")]
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
    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
    }
}