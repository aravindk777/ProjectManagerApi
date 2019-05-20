using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Models.DataModel
{
    [Table("Task")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        public int? ParentTaskId { get; set; }
        public virtual Task ParentTask { get; set; }

        [Required]
        public string TaskName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Priority { get; set; }

        public Guid TaskOwnerId { get; set; }
        public virtual User TaskOwner { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}
