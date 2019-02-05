using PM.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Repos.Project
{
    public class ProjectRepo : Repository<Projects>, IProjectRepo
    {
        public ProjectRepo(DbContext dbContext) : base(dbContext) { }
    }
}
