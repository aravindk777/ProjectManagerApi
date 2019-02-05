using System;

namespace PM.Models.ViewModels
{
    public class Users : IViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Format($"{LastName}, {FirstName}"); } }
        public string UserId { get; set; }
    }
}
