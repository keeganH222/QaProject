using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
    public class Tag
    {
        public Tag()
        {
            this.Questions = new List<Question>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}