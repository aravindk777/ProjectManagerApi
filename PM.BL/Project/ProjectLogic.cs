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
    public class ProjectLogic : BaseLogic , IProjectLogic
    {
        private readonly IProjectRepo _projectRepo;

        public ProjectLogic(IProjectRepo projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public Projects CreateProject(Projects project)
        {
            return null;
        }

        public IEnumerable<Projects> GetAllProjects()
        {
            _projectRepo.GetAll().AsViewModel();
        }

        public Projects GetProject(int projId = 0, string projectName = "")
        {
            throw new NotImplementedException();
        }

        public bool Modify(int projId, Projects projectViewModel)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int projId)
        {
            throw new NotImplementedException();
        }
    }
}
