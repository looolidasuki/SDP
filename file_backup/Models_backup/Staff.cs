using System;

namespace Sales_user.Models
{
    public class Staff
    {
        public long StaffID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime EmployDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
