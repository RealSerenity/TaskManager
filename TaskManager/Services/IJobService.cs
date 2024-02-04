using TaskManager.Data.Entities;
using TaskManager.Models;

namespace TaskManager.Services
{
    public interface IJobService
    {
        IEnumerable<Job> GetAllJobs();
        Job GetJobById(int jobId);
        Task<Job> AddJob(Job job);
        Task<bool> ActivateJob(int jobId);
        Task<bool> CompleteJob(int jobId);
        Task<Tuple<bool,Job>> UpdateJob(int jobId, Job job);
        Task<bool> DeleteJob(int jobId);
        Task<int> MustInProgress();
        Job ModelToEntity(JobModel job);
        Task<int> UpdateExpiredJobs();
    }
}
