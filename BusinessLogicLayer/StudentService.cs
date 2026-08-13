using studentDataAccessLayer;
using System.Collections.Generic;

namespace BusinessLogicLayer
{
    public class Student
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        // الخصائص المتوافقة مع قاعدة البيانات الخاصة بمشروعنا
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public decimal? GPA { get; set; }

        // تحويل الخصائص الحالية لـ DTO عشان نبعته لطبقة الـ Data Access
        public StudentDTO SDTO
        {
            get { return new StudentDTO(this.Id, this.UserId, this.FullName, this.Email, this.Department, this.GPA); }
        }

        // Constructor في حالة إضافة طالب جديد
        public Student()
        {
            this.Mode = enMode.AddNew;
        }

        // Private Constructor بيستخدم داخلياً لما بنعمل Find لطالب موجود فعلاً
        private Student(StudentDTO SDTO, enMode cMode = enMode.AddNew)
        {
            this.Id = SDTO.Id;
            this.UserId = SDTO.UserId;
            this.FullName = SDTO.FullName;
            this.Email = SDTO.Email;
            this.Department = SDTO.Department;
            this.GPA = SDTO.GPA;

            this.Mode = cMode;
        }

        // دالة البحث بالـ ID الخاص بالطالب
        public static Student Find(int id)
        {
            // ستحتاج لإضافة دالة GetStudentById في كلاس StudentData في طبقة الـ DAL لكي تعمل هذه الدالة
            StudentDTO SDTO = StudentData.GetStudentById(id);
            if (SDTO != null) return new Student(SDTO, enMode.Update);
            return null;
        }

        
        public static Student FindByUserId(int userId)
        {
            StudentDTO SDTO = StudentData.GetStudentByUserId(userId);
            if (SDTO != null) return new Student(SDTO, enMode.Update);
            return null;
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    this.Id = StudentData.AddStudent(this.SDTO);

         
                    if (this.Id > 0)
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return StudentData.UpdateStudent(this.SDTO);
            }
            return false;
        }

   
        public static List<StudentDTO> GetAllStudents() => StudentData.GetAllStudents();

        public static bool DeleteStudent(int studentId) => StudentData.DeleteStudent(studentId);
    }
}