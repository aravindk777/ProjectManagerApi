using PM.Data.Entities;
using PM.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PM.Data.Repos.Tasks
{
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(PMDbContext context) : base(context) { }
    }
}
