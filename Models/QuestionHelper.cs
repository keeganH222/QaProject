using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace QaProject.Models
{
    public class QuestionHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public QuestionHelper()
        {
            this.tagList = db.Tags.ToList();
            this.incrementCommentCount = 0;
        }
        
        public List<Tag> tagList { get; set; }
        public int[] TagIds { get; set; }
        public int commentCount { get; set; }
        public int incrementCommentCount { get; set; }
        public void resetCommentCount()
        {
            this.commentCount = 0;
        }
        public void SetTagIdArray(int[] tagId)
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
        public int[] getQuestionTags()
        {
            return this.TagIds;
            
        }
        public bool RemoveSpecificTag(string tagName)
        {
            var tag = db.Tags.FirstOrDefault(t => t.Name == tagName);
            if(tagName == null)
            {
                return false;
            }
            int tagId = tag.Id;
            var tagIds = TagIds.ToList();
            if(!tagIds.Remove(tagId))
            {
                return false;
            }
            this.TagIds = tagIds.ToArray();
            return true;
        }
    }
}