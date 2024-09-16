using System.Collections.Generic;

namespace SafriSoftv1._3.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> UserRoles { get; set; }
        public List<string> UserRoleIds { get; set; }
        public int NumberOfOrders { get; set; }
        public double RandValueSold { get; set; }
        public string UserState { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
    }
}