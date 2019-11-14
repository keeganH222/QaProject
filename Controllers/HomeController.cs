using Microsoft.AspNet.Identity;
using QaProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace QaProject.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static QuestionHelper qh = new QuestionHelper();
        private 
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize]
        public ActionResult postQuestion()
        {
            ViewBag.Description = "";
            return View();
        }
        [HttpPost]
        [ActionName("postQuestion")]
        public ActionResult CreateQuestion([Bind(Include = "Title")] Question question, string Description)
        {
            if (ModelState.IsValid)
            {
                question.Description = Description;
                question.OwnerId = User.Identity.GetUserId();
                question.PostedOn = DateTime.Now;
                question.Tags = qh.getQuestionTags();
                qh.removeTagArray();
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Question", new { id = question.Id });
            }
            ViewBag.Description = "";
            return View(question);
        }
        [HttpPost]
        public ActionResult AddComment(int id, string type, [Bind (Include = "Content")]Comment comment)
        {
            comment.OwnerId = User.Identity.GetUserId();
            if (type == "answer")
            {
                var answer = db.Answers.FirstOrDefault(a => a.Id == id);
                if(answer != null)
                {
                    comment.PostedOn = DateTime.Now;
                    comment.AnswerId = id;
                    db.Comments.Add(comment);
                    db.SaveChanges();
                }
            }
            else if(type == "question")
            {
                var question = db.Questions.FirstOrDefault(q => q.Id == id);
                if(question != null)
                {
                    comment.PostedOn = DateTime.Now;
                    comment.QuestionId = id;
                    db.Comments.Add(comment);
                    db.SaveChanges();
                }
            }
            return View();
        }
        public ActionResult postAnswer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult postAnswer(int id, [Bind (Include = "Content")] Answer answer)
        {
            if(ModelState.IsValid)
            {
                var question = db.Questions.FirstOrDefault(q => q.Id == id);
                answer.OwnerId = User.Identity.GetUserId();
                if(question != null)
                {
                    answer.QuestionId = id;
                    answer.PostedOn = DateTime.Now;
                    db.Answers.Add(answer);
                    db.SaveChanges();
                }
            }
            return View();
        }
        
        public ActionResult addTag()
        {
            var tagList = db.Tags.ToList();
            List<TagViewModel> tags = new List<TagViewModel>();
            foreach(Tag tag in tagList)
            {
                if(qh.TagIds != null)
                {
                    if (qh.TagIds.Contains(tag.Id))
                    {
                        tags.Add(new TagViewModel { tagId = tag.Id, tagName = tag.Name, IsChecked = true });
                    }
                    else
                    {
                        tags.Add(new TagViewModel { tagId = tag.Id, tagName = tag.Name });
                    }
                }
                else
                {
                    tags.Add(new TagViewModel { tagId = tag.Id, tagName = tag.Name });
                }
            }
            return View(tags);
        }
        [HttpPost]
        public async Task<JsonResult> SaveTag(IEnumerable<int> arrayOfIds)
        {
            if(arrayOfIds != null)
            {
                int[] tagIds = arrayOfIds.ToArray();
                await qh.SetTagIdArray(tagIds);
            }
            return Json( JsonRequestBehavior.AllowGet);
        }
        public ActionResult Question(int id)
        {
            var question = db.Questions.FirstOrDefault(q => q.Id == id);
            return View(question);
        }
        public async Task<JsonResult> removeTag(string name)
        {

        }
    }
}