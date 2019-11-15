using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
    public class UpVote
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public virtual ApplicationUser user { get; set; }
        public int? questionId { get; set; }
        public virtual Question question { get; set; }
        public int? answerId { get; set; }
        public virtual Answer answer { get; set; }
    }
}