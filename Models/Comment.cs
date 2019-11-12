using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Votes { get; set; }
        public DateTime PostedOn { get; set; }
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public int? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public int? QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}