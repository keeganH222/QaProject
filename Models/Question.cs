using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
    public class Question
    {
        public Question()
        {
            this.Answers = new List<Answer>();
            this.Comments = new List<Comment>();
            this.Tags = new List<Tag>();
            this.UpVotes = new List<UpVote>();
            this.DownVotes = new List<DownVote>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedOn { get; set; }
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UpVote> UpVotes { get; set; }
        public virtual ICollection<DownVote> DownVotes { get; set; }
    }
}