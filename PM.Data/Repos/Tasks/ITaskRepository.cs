using PM.Models.DataModel;

namespace PM.Data.Repos.Tasks
{
    public interface ITaskRepository : IRepository<Task>
    {
        Task GetTaskByName(string name);
        bool EndTask(int taskId);
    }
}
