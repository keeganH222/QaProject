using Microsoft.AspNet.Identity;
using PagedList;
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
        private static QuestionHelper questionHelper = new QuestionHelper();
        private QALogic qaLogic;
        public HomeController()
        {
            qaLogic = new QALogic(new GeneralDataAccess(db));
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
        public ActionResult createQuestion([Bind(Include = "Title")] Question question, string Description)
        {
            if (ModelState.IsValid)
            {
                question.Description = Description;
                question.OwnerId = User.Identity.GetUserId();
                question.PostedOn = DateTime.Now;
                //Ajax requests have saved user select tags, must retrieve them from an object helper
                var tagsToAdd = questionHelper.getQuestionTags();
                questionHelper.removeTagArray();
                qaLogic.HandleSaveQuestionLogic(question);
                qaLogic.HandleAddTagsToQuestion(question, tagsToAdd);
                return RedirectToAction("Question", new { id = question.Id });
            }
            ViewBag.Description = "";
            return View(question);
        }
        public ActionResult AddComment(int? itemId, string type)
        {
            AddingCommentViewModel viewModel = new AddingCommentViewModel { itemId = (int)itemId, itemType = type, commentCount = questionHelper.commentCount };        
            questionHelper.commentCount++;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<ActionResult> AddComment(int itemid, string type, string content)
        {
            Comment comment = new Comment { Content = content};
            comment.OwnerId = User.Identity.GetUserId();
            int commentToDisplayId = -12;
            await Task.Run(() =>
            {
                if (type == "answer")
                {
                    var answer = qaLogic.HandleGetAnswer(itemid);
                    if (answer != null)
                    {
                        comment.AnswerId = itemid;
                    }
                }
                else if (type == "question")
                {
                    var question = qaLogic.HandleGetQuestion(itemid);
                    if (question != null)
                    {
                        comment.PostedOn = DateTime.Now;
                        comment.QuestionId = itemid;
                    }
                }
                comment.PostedOn = DateTime.Now;
                qaLogic.HandleSaveCommentLogic(comment);
                commentToDisplayId = comment.Id;
            });
            Task.WaitAll();
            CommentViewModel viewModel = new CommentViewModel { comment = comment, userName = User.Identity.Name };
            return View("Comment", viewModel);
        }
        public ActionResult AddAnswer(int itemId, int answerCount)
        {
            AddingAnswerViewModel viewModel = new AddingAnswerViewModel { itemId = itemId, answerCount = answerCount };
            return View(viewModel);
        }
        [HttpPost]
        [ActionName("addAnswer")]
        public async Task<ActionResult> postAnswer(int itemId,string content)
        {
            Answer answer = new Answer { QuestionId = itemId, Content = content };
            await Task.Run(() =>
            {
                if (ModelState.IsValid)
                {
                    
                    answer.OwnerId = User.Identity.GetUserId();
                    answer.PostedOn = DateTime.Now;
                    qaLogic.HandleSaveAnswerLogic(answer);
                }
            });
            AnswerViewModel viewModel = new AnswerViewModel { answer = answer, userName = User.Identity.Name };
            return View("Answer", viewModel);
        }
        //getting list of tag for a user to select
        public ActionResult addTag()
        {
            var tagList = db.Tags.ToList();
            List<TagViewModel> tags = new List<TagViewModel>();
            foreach (Tag tag in tagList)
            {
                if (questionHelper.TagIds != null)
                {
                    if (questionHelper.TagIds.Contains(tag.Id))
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
            if (arrayOfIds != null)
            {
                int[] tagIds = arrayOfIds.ToArray();
                await Task.Run(() => questionHelper.SetTagIdArray(tagIds));
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult Question(int id)
        {
            var question = db.Questions.FirstOrDefault(q => q.Id == id);
            ViewBag.VoteCount = question.UpVotes.Count() - question.DownVotes.Count();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var upVote = question.UpVotes.FirstOrDefault(v => v.userId == userId);
                var downVote = question.DownVotes.FirstOrDefault(v => v.userId == userId);
                if (upVote == null && downVote == null)
                {
                    ViewBag.userId = userId;
                }
            }
            questionHelper.resetCommentCount();
            return View(question);
        }
        [HttpPost]
        public async Task<JsonResult> removeTag(string TagName)
        {
            var task = await Task.Run(() => questionHelper.RemoveSpecificTag(TagName));
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult MainPage(int? page)
        {
            ViewBag.url = "MainPage";
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var questionList = qaLogic.HandleQuestionListLogic("retrieve list").OrderByDescending(q => q.PostedOn);
            return View(questionList.ToPagedList(pageNumber, pageSize));
            
        }
        public ActionResult FilterNewestQuestions(int? page)
        {
            ViewBag.url = "FilterNewestQuestions";
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var questionList = qaLogic.HandleQuestionListLogic("retrieve list");
            var questionsToDisplay = questionList.OrderByDescending(q => q.PostedOn.Year)
                .ThenByDescending(q => q.PostedOn.Month)
                .ThenByDescending(q => q.PostedOn.Day).ToList();
            return View("MainPage",questionsToDisplay.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult FilterAnsweredQuestions(int? page)
        {
            ViewBag.url = "FilterAnsweredQuestions";
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var questionList = qaLogic.HandleQuestionListLogic("retrieve list");
            var questionToDisplay = questionList.OrderByDescending(q => q.Answers.Count()).ToList();
            return View("MainPage", questionToDisplay.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult FilterTopQuestions(int? page)
        {
            ViewBag.url = "FilterTopQuestions";
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var questionList = qaLogic.HandleQuestionListLogic("retrieve list");
            var questionToDisplay = questionList.Where(q => q.PostedOn.Day == DateTime.Now.Day && q.PostedOn.Month == DateTime.Now.Month && q.PostedOn.Year == DateTime.Now.Year)
                .OrderByDescending(q => q.Answers.Count()).ToList();
            return View("MainPage", questionToDisplay.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult TagList()
        {
            var tagList = qaLogic.HandleTagListLogic("Retrive list").ToList();
            return View(tagList);
        }
        public ActionResult ListOfTagsQuestions(string tagName, int? page)
        {
            var tag = qaLogic.HandleGetTag(tagName);
            if (tag == null)
            {
                return HttpNotFound();
            }
            ViewBag.url = "ListOfTagsQuestions";
            ViewBag.tagName = tagName;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View("MainPage", tag.Questions.ToPagedList(pageNumber, pageSize));
        }
        public async Task<JsonResult> UpVoteItem(string itemType, string itemId, string userId)
        {
            await Task.Run(() =>
            {
                UpVote upVote = null;
                string originalQuestionUserId = "";
                if (itemType == "question")
                {
                    var question = qaLogic.HandleGetQuestion(Convert.ToInt32(itemId));
                    if (question != null)
                    {
                        upVote = new UpVote { questionId = Convert.ToInt32(itemId), userId = userId };
                        originalQuestionUserId = question.OwnerId;
                    }
                }
                else
                {
                    var answer = qaLogic.HandleGetAnswer(Convert.ToInt32(itemId));
                    if (answer != null)
                    {
                        upVote = new UpVote { answerId = Convert.ToInt32(itemId), userId = userId };
                        originalQuestionUserId = answer.OwnerId;
                    }
                }
                qaLogic.HandleSaveUpVote(upVote);
                qaLogic.HandleUpdateReputation(originalQuestionUserId, 5);
            });
            return Json("UpVote", JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> DownVoteItem(string itemType, string itemId, string userId)
        {
            await Task.Run(() =>
            {
                DownVote downVote = null;
                string originalQuestionUserId = "";
                if (itemType == "question")
                {
                    var question = qaLogic.HandleGetQuestion(Convert.ToInt32(itemId));

                    if (question != null)
                    {
                        downVote = new DownVote { questionId = Convert.ToInt32(itemId), userId = userId };
                        originalQuestionUserId = question.OwnerId;
                    }
                }
                else
                {
                    var answer = qaLogic.HandleGetAnswer(Convert.ToInt32(itemId));
                    if (answer != null)
                    {
                        downVote = new DownVote { answerId = Convert.ToInt32(itemId), userId = userId };
                        originalQuestionUserId = answer.OwnerId;
                    }
                }
                qaLogic.HandleSaveDownVote(downVote);
                qaLogic.HandleUpdateReputation(originalQuestionUserId, -5);
            });
            return Json("DownVote", JsonRequestBehavior.AllowGet);
        }
    }
}