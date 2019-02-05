using System;
using System.ComponentModel.DataAnnotations;

namespace PM.Models.ViewModels
{
    public class Users : IViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }
        public string FullName { get { return string.Format($"{LastName}, {FirstName}"); } }
        [Required]
        public string UserId { get; set; }
    }
}
