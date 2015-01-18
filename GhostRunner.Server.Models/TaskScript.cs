using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.Models
{
    public class TaskScript
    {
        [Required]
        public long ID { get; set; }

        public String Content { get; set; }

        public ScriptType Type { get; set; }

        public String Log { get; set; }

        [Required]
        public int Position { get; set; }

        public virtual Task Task { get; set; }

        public virtual ICollection<TaskScriptParameter> TaskScriptParameters { get; set; }

        public String GetHTMLFormattedContent()
        {
            if (!String.IsNullOrEmpty(Content)) return Content.Replace(Environment.NewLine, "<br/>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            else return String.Empty;
        }

        public String GetHTMLFormattedLogScript()
        {
            if (!String.IsNullOrEmpty(Log)) return Log.Replace(Environment.NewLine, "<br/>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            else return String.Empty;
        }
    }
}
