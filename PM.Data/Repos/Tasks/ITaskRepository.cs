using PM.Models.DataModel;

namespace PM.Data.Repos.Tasks
{
    public interface ITaskRepository : IRepository<Task>
    {
        bool EndTask(int taskId);
    }
}
