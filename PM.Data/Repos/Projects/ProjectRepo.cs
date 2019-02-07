using PM.Data.Entities;

namespace PM.Data.Repos.Projects
{
    public class ProjectRepo : Repository<Models.DataModel.Project>, IProjectRepo
    {
        public ProjectRepo(PMDbContext dbContext) : base(dbContext) { }
    }
}
