using System.Collections.Generic;
using System.Linq;
using PM.BL.Common;
using PM.Data.Repos.Projects;
using PM.Models.ViewModels;

namespace PM.BL.Projects
{
    public class ProjectLogic : IProjectLogic
    {
        private readonly IProjectRepo _projectRepo;

        public ProjectLogic(IProjectRepo projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public Project CreateProject(Models.ViewModels.Project project)
        {
            return _projectRepo.Create(project.AsDataModel()).AsViewModel();
        }

        public IEnumerable<Models.ViewModels.Project> GetAllProjects()
        {
            return _projectRepo.GetAll().AsViewModel();
        }

        public Project GetProject(int projId = 0, string projectName = "")
        {
            return _projectRepo.GetById(projId).AsViewModel();
        }

        public IEnumerable<Project> GetUserProjects(string userId)
        {
            var result = _projectRepo.Search(p => p.Manager.UserId == userId);
            return result.AsViewModel();
            //var finaldata = result.AsViewModel();
            //return _projectRepo.GetAll().Where(usr => usr.Manager.UserId == userId).AsViewModel();
        }

        public bool Modify(int projId, Models.ViewModels.Project projectViewModel)
        {
            return _projectRepo.Update(projectViewModel.AsDataModel());
        }

        public bool Remove(int projId)
        {
            return _projectRepo.Delete(_projectRepo.GetById(projId));
        }
    }
}
