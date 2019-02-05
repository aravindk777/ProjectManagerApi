using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.BL.Common;
using PM.Data.Repos.Project;
using PM.Models.ViewModels;

namespace PM.BL.Project
{
    public class ProjectLogic : IProjectLogic
    {
        private readonly IProjectRepo _projectRepo;

        public ProjectLogic(IProjectRepo projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public Projects CreateProject(Projects project)
        {
            return _projectRepo.Create(project.AsDataModel()).AsViewModel();
        }

        public IEnumerable<Projects> GetAllProjects()
        {
            return _projectRepo.GetAll().AsViewModel();
        }

        public Projects GetProject(int projId = 0, string projectName = "")
        {
            return _projectRepo.GetById(projId).AsViewModel();
        }

        public bool Modify(int projId, Projects projectViewModel)
        {
            return _projectRepo.Update(projectViewModel.AsDataModel());
        }

        public bool Remove(int projId)
        {
            return _projectRepo.Delete(_projectRepo.GetById(projId));
        }
    }
}
