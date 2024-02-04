using System.Collections.Generic;
using TaskManager.Data.Enums;

namespace TaskManager.Data.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = null;
        public string Description { get; set; } = null;
        public int ResponsibleId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public JobType JobType { get; set; }
        public JobStatus JobStatus { get; set; }
        public Priority Priority { get; set; }
        public string AcceptanceCriteria { get; set; } = null;
        public DateTime CompletedAt { get; set; }
    }
}
