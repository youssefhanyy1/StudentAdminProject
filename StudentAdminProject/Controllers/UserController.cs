using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using StudentAdminProject.DTOs.Requests;

namespace StudentAdminProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {

            BusinessLogicLayer.User user = BusinessLogicLayer.User.Find(id);
            if (user == null)
                return NotFound("المستخدم غير موجود.");

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
            if (request == null || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("كلمة المرور الجديدة مطلوبة.");
            }

            BusinessLogicLayer.User user = BusinessLogicLayer.User.Find(id);
            if (user == null)
                return NotFound("المستخدم غير موجود.");

            user.SetPassword(request.NewPassword);

            if (user.Save())
            {
                return Ok(new { message = "تم تحديث كلمة المرور بنجاح." });
            }

            return StatusCode(500, "حدث خطأ أثناء تحديث كلمة المرور.");
        }
   
        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("اسم المستخدم وكلمة المرور مطلوبان.");
            }

   
            if (BusinessLogicLayer.User.FindByUsername(request.Username) != null)
            {
                return BadRequest("اسم المستخدم موجود بالفعل.");
            }

            BusinessLogicLayer.User newUser = new BusinessLogicLayer.User();
            newUser.Username = request.Username;
            newUser.Role = request.Role ?? "Student"; 

            newUser.SetPassword(request.Password);

            if (newUser.Save())
            {
                return Ok(new
                {
                    message = "تم إنشاء المستخدم بنجاح.",
                    userId = newUser.Id
                });
            }

            return StatusCode(500, "حدث خطأ أثناء حفظ المستخدم.");
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            BusinessLogicLayer.User user = BusinessLogicLayer.User.Find(id);
            if (user == null)
                return NotFound("المستخدم غير موجود.");

            if (BusinessLogicLayer.User.DeleteUser(id))
            {
                return Ok(new { message = "تم حذف المستخدم بنجاح." });
            }

            return StatusCode(500, "حدث خطأ أثناء حذف المستخدم.");
        }
    }


}