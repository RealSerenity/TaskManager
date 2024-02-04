using Microsoft.EntityFrameworkCore;
using System.Collections;
using TaskManager.Context;
using TaskManager.Data.Enums;
using TaskManager.Models;
using Job = TaskManager.Data.Entities.Job;

namespace TaskManager.Services.Impl
{
    public class JobServiceImpl : IJobService
    {
        private readonly ApplicationDbContext _context;
        public JobServiceImpl(ApplicationDbContext context)
        {
            _context = context;

        }

        public IEnumerable<Job> GetAllJobs()
        {
            return _context.Jobs.ToList();
        }

        public Job GetJobById(int jobId)
        {
            return _context.Jobs.Find(jobId);
        }

        /*
         *         Priority1 = 1,Priority2 = 2,Priority3 = 3,
         *         Daily=0,Weekly = 1,Monthly = 2
         *         Committed=0,InProgress = 1,Completed =2,Expired=3
         *  
         *        CreateTime ve JobStatus özelliklerini job oluştururken manuel olarak ekledim, bunlar başlangıç için default değerler oldukları için
         */
        public async Task<Job> AddJob(Job job)
        {
            // JobType da tanımlanan değerlerin dışında bir değer girildiğinde hata döndürür
            if (!job.JobType.IsValidValue())
            {
                throw new Exception("Invalid job tpye!");
            }

            job.CreateTime = DateTime.Now;
            // job tpye günlük ise ve start time bugün ise jobstatus committed değil direkt olarak inprogress şeklinde kayıtedilir.
            if (job.JobType == JobType.Daily)
            {
                if (job.StartTime.Day == job.CreateTime.Day) job.JobStatus = JobStatus.InProgress;
            }
            else
            {
                job.JobStatus = JobStatus.Committed;
            }


            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<Tuple<bool, Job>> UpdateJob(int jobId, Job job)
        {
            Job oldJob = GetJobById(jobId);
            oldJob.Title = job.Title;
            oldJob.Description = job.Description;
            oldJob.StartTime = job.StartTime;
            oldJob.Priority = job.Priority;
            job.Deadline = job.StartTime + TimeSpan.FromDays((int)job.JobType);
            oldJob.ResponsibleId = job.ResponsibleId;
            oldJob.JobType = job.JobType;
            oldJob.AcceptanceCriteria = job.AcceptanceCriteria;

            Job updatedJob = _context.Jobs.Update(oldJob).Entity;
            return Tuple.Create(await _context.SaveChangesAsync() > 0, updatedJob);
        }

        public async Task<bool> DeleteJob(int jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
                return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> CompleteJob(int jobId)
        {
            // get methodu async olduğu için date'i öncesinde aldım
            DateTime completeTime = DateTime.Now;
            var job = GetJobById(jobId);


            // eğer jobstatus inprogress ise günceller,
            // tamamlanmamış olarak etiketlenmiş yani süresi geçmiş job'ları daha sonradan tamamlandı oalrak etiketlemeyi engeller
            // ayrıca tamamnlanmış bir job'u tekrar tamamlamayı engeller
            if (job.JobStatus == Data.Enums.JobStatus.InProgress)
            {
                job.CompletedAt = completeTime;
                job.JobStatus = Data.Enums.JobStatus.Completed;
                _context.Jobs.Update(GetJobById(jobId));
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        // ActivateJob methodu waiting JobStatus'unde olan job'u aktive etmek için kullanılır. from JobStatus.Waiting to JobStatus.InProgress
        public async Task<bool> ActivateJob(int jobId)
        {
            var job = GetJobById(jobId);

            // eğer jobstatus inprogress ise günceller, tamamlanmamış olarak etiketlenmiş yani süresi geçmiş job'ları daha sonradan tamamlandı oalrak etiketlemeyi engeller ayrıca tamamnlanmış bir job'u tekrar tamamlamayı engeller
            if (job.JobStatus == Data.Enums.JobStatus.Committed)
            {
                job.JobStatus = Data.Enums.JobStatus.InProgress;
                _context.Jobs.Update(GetJobById(jobId));
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        // starttime zamanı gelmiş job'ların jobstatus'unu inprogress' çevirir TimerService içerisinde otomatik olarak kullanılır 
        public async Task<int> MustInProgress()
        {
            var mustBeInProgressList = await _context.Jobs.Where(j => j.JobStatus == JobStatus.Committed).Where(j => j.StartTime.Day == DateTime.Now.Day).ToListAsync();
            foreach (var j in mustBeInProgressList)
            {
                j.JobStatus = JobStatus.InProgress;
                _context.Jobs.Update(j);
                await _context.SaveChangesAsync();
            }
            return mustBeInProgressList.Count;
        }

        // deadline zamanı gelmiş job'ları expire eder. TimerService içerisinde otomatik olarak kullanılır
        public async Task<int> UpdateExpiredJobs()
        {
            var expiredJobList = await _context.Jobs.Where(j => j.JobStatus != JobStatus.Completed && j.JobStatus != JobStatus.Expired).Where(j => j.Deadline.Day <= DateTime.Now.Day).ToListAsync();
            foreach (var j in expiredJobList)
            {
                j.JobStatus = JobStatus.Expired;
                _context.Jobs.Update(j);
                await _context.SaveChangesAsync();
            }
            return expiredJobList.Count;

        }



        // controllerda job creation ve update işlemlerini gerçekleştirirken gereksiz inputları çıkarmak için JobModel kullandım
        // JobService Job classını kullanıyor bu yüzden JobModel'den Job classına değişimi birden fazla yerde kullandığım için bu işlemi bir method haline getirmek istedim
        public Job ModelToEntity(JobModel job)
        {
            return new Job
            {
                Title = job.Title,
                Description = job.Description,
                ResponsibleId = job.ResponsibleId,
                JobType = job.JobType,
                StartTime = job.StartTime,
                Priority = job.Priority,
                AcceptanceCriteria = job.AcceptanceCriteria
            };
        }
    }
}
