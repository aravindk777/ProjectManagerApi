using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Models.DataModel
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime? ProjectStart { get; set; }
        public DateTime? ProjectEnd { get; set; }
        public Guid ManagerId { get; set; }
        public virtual User Manager { get; set; }
        public int Priority { get; set; }
    }
}
