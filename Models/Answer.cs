using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
    public class Answer
    {
        public Answer()
        {
            this.UpVotes = new List<UpVote>();
            this.DownVotes = new List<DownVote>();
        }
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public virtual ICollection<UpVote> UpVotes { get; set; }
        public virtual ICollection<DownVote> DownVotes { get; set; }
    }
}