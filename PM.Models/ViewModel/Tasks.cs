using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PM.Models.ViewModels
{
    public class Task
    {
        public int TaskId { get; set; }
        [Required]
        public string TaskName { get; set; }
        [Required]
        public int Priority { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsParent { get; set; }
        public string ParentTask { get; set; }
        public int? ParentTaskId { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public Guid TaskOwnerId { get; set; }
        public string OwnerFullName { get; set; }
    }
}
