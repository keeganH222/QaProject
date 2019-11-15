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
        private QALogic qaLogic;
        public HomeController()
        {
            qaLogic = new QALogic(new GeneralDataAccess());
        }
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
        //Create question
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
                //Ajax requests have saved user select tags, must retrieve them from an object helper
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
        //getting list of tag for a user to select
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
        //to let the user create a tag
        [HttpPost]
        public JsonResult CreateTag(String tagName)
        {
            Tag tag = null;
            if (!string.IsNullOrEmpty(tagName))
            {
                qaLogic.HandleSaveTagLogica(tagName);
                tag = qaLogic.HandleGetTag(tagName);
            }
            return Json(tag.Id, JsonRequestBehavior.AllowGet);
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
        [HttpPost]
        public async Task<JsonResult> removeTag(string TagName)
        {
            var task = await Task.Run(() => qh.RemoveSpecificTag(TagName));
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult MainPage()
        {
            var questionList = qaLogic.HandleQuestionListLogic("retieve list");
            return View(questionList.OrderByDescending(q => q.PostedOn));
        }
        public ActionResult TagList()
        {
            var tagList = qaLogic.HandleTagListLogic("Retrive list").ToList();
            return View(tagList);
        }
        public ActionResult ListOfTagsQuestions(string tagName)
        {
            var tag = qaLogic.HandleGetTag(tagName);
            if(tag == null)
            {
                return HttpNotFound();
            }
            return View("MainPage", tag.Questions);
        }
        public async Task<JsonResult> UpVoteItem(string itemType, int itemId, string userId)
        {
            var result = Task.Run(() =>
            {
                UpVote upVote = null;
                if (itemType == "question")
                {
                    var question = qaLogic.HandleGetQuestion(itemId);
                    if (question != null)
                    {
                        upVote = new UpVote { questionId = itemId, userId = userId };
                    }
                }
                else
                {
                    var answer = qaLogic.HandleGetAnswer(itemId);
                    if (answer != null)
                    {
                        upVote = new UpVote { answerId = itemId, userId = userId };
                    }
                }
                qaLogic.HandleSaveUpVote(upVote);
            });
            return Json(JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> DownVoteItem(string itemType, int itemId, string userId)
        {
            var result = Task.Run(() =>
            {
                UpVote upVote = null;
                if (itemType == "question")
                {
                    var question = qaLogic.HandleGetQuestion(itemId);
                    if (question != null)
                    {
                        upVote = new UpVote { questionId = itemId, userId = userId };
                    }
                }
                else
                {
                    var answer = qaLogic.HandleGetAnswer(itemId);
                    if (answer != null)
                    {
                        upVote = new UpVote { answerId = itemId, userId = userId };
                    }
                }
                qaLogic.HandleSaveUpVote(upVote);
            });
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}