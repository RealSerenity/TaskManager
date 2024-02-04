using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Data.Entities;
using TaskManager.Data.Enums;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Services.Impl;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // GET: api/jobs
        [HttpGet("getAll")]
        public ActionResult<List<Job>> GetAllJobs()
        {
            return _jobService.GetAllJobs().ToList();
        }

        // GET: api/job/5
        [HttpGet("getById/{id}")]
        public ActionResult<Job> GetJobById(int id)
        {
            var job = _jobService.GetJobById(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        // POST: api/job
        [HttpPost("create")]
        public async Task<ActionResult<Job>> CreateJob(JobModel jobModel)
        {
            var addedJob = await _jobService.AddJob(_jobService.ModelToEntity(jobModel));
            return Ok(addedJob);
        }

        // PUT: api/job/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateJob(int id, JobModel jobModel)
        {
            var result = await _jobService.UpdateJob(id, _jobService.ModelToEntity(jobModel));
            if (!result.Item1)
            {
                throw new Exception("Someting went wrong!");
            }
            return Ok(result.Item2);

        }

        // DELETE: api/job/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var result = await _jobService.DeleteJob(id);
            if (!result)
            {
                return NotFound();
            }

            return Ok("Deleted");
        }


        [HttpPost("activate/{id}")]
        public async Task<IActionResult> ActivateJob(int jobId)
        {
            bool result = await _jobService.ActivateJob(jobId);
            if (result)
            {
                return Ok("Job activated successfully.");
            }
            else
            {
                return BadRequest("Failed to activate job.");
            }
        }

        [HttpPost("complete/{id}")]
        public async Task<IActionResult> CompleteJob(int jobId)
        {
            bool result = await _jobService.CompleteJob(jobId);
            if (result)
            {
                return Ok("Job completed successfully.");
            }
            else
            {
                return BadRequest("Failed to complete job.");
            }
        }
    }
}
