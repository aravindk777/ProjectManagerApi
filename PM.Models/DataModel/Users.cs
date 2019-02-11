using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Models.DataModel
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [MaxLength(15, ErrorMessage = "UserId cannot be more than 15 character length")]
        public string UserId { get; set; }

    }
}
