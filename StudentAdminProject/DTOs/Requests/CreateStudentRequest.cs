namespace StudentAdminProject.Requests
{
    public class CreateStudentRequest
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public decimal? GPA { get; set; }
    }
}