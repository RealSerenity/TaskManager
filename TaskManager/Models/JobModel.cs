using System;
using TaskManager.Data.Enums;

namespace TaskManager.Models
{
    public class JobModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int ResponsibleId { get; set; }
        public DateTime StartTime { get; set; }
        public JobType JobType { get; set; }
        public Priority Priority { get; set; }
        public string? AcceptanceCriteria { get; set; }
    }
}
