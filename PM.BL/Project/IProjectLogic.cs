using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.BL.Project
{
    public interface IProjectLogic
    {
        Models.ViewModels.Projects CreateProject(Models.ViewModels.Projects project);
        IEnumerable<Models.ViewModels.Projects> GetAllProjects();
        bool Modify(int projId, Models.ViewModels.Projects projectViewModel);
        bool Remove(int projId);
        Models.ViewModels.Projects GetProject(int projId = 0, string projectName = "");
    }
}
