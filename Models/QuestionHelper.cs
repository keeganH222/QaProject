using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
    public class QuestionHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public QuestionHelper()
        {
            this.tagList = new List<Tag>();
            this.tagList = db.Tags.ToList();
        }
        public List<Tag> tagList { get; set; }
        public int[] TagIds { get; set; }
        public void setTagIdArray(int[] tagId)
        {
            this.TagIds = tagId;
        }
        public void removeTagArray()
        {
            this.TagIds = null;
        }
        public void setTags()
        {
            this.tagList = db.Tags.ToList();
        }
        public List<Tag> getQuestionTags()
        {
            List<Tag> taglist = new List<Tag>();
            foreach(var tagId in this.TagIds)
            {
                var tag = this.tagList.FirstOrDefault(t => t.Id == tagId);
                if(tag != null)
                {
                    tagList.Add(tag); 
                }
            }
            return taglist;
            
        }
    }
}