using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.Models
{
    public class Task
    {
        [Required]
        public int ID { get; set; }

        [Required, MaxLength(38)]
        public String ExternalId { get; set; }

        [Required(ErrorMessage = " * Required")]
        public String Name { get; set; }

        public String Description { get; set; }

        [Required]
        public String Content { get; set; }

        [Required]
        public Status Status { get; set; }

        public String Log { get; set; }

        public String PhantomScript { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public virtual Script Script { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<TaskParameter> TaskParameters { get; set; }
    }
}
