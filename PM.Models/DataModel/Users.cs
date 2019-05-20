using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Models.DataModel
{
    [Table("Users")]
    public class User
    {
        public User()
        {
            //_createdValue = DateTime.Now;
            Created = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(255)]
        [Required]
        public string LastName { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [MaxLength(15, ErrorMessage = "UserId cannot be more than 15 character length")]
        public string UserId { get; set; }

        [DefaultValue("getdate()")]
        //public DateTime Created { get; set; }
        public DateTime Created
        {
            get { return _createdValue == DateTime.MinValue ? DateTime.Now : _createdValue; }
            set { _createdValue = value; }
        }
        private DateTime _createdValue;

        public DateTime? EndDate { get; set; }
    }
}
