using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QaProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace QaProject.Models
{
    interface IDataAccess
    {
        List<Tag> getTagList(string uid);
        List<Question> getQuestionList(string uid);
        List<Comment> getCommentList(string uid);
        List<Answer> getAnswerList(string uid);
        void addTagsToQuestion(Question question, int[] tags);
        void saveTag(string tag);
        bool saveQuestion(Question question);
        bool saveAnswer(Answer answer);
        bool saveComment(Comment comment);
        void saveUpVote(UpVote upVote);
        void saveDownVote(DownVote downVote);
        Tag getSpecificTag(string TagName);
        Question getQuestion(int id);
        Answer getAnswer(int id);
        void updateUserReputation(string userId, int repToAdd);
        Comment getComment(int id);
    }
    class QALogic
    {
        IDataAccess dta;
        public QALogic(IDataAccess injectedDTA)
        {
            dta = injectedDTA;
        }
        public List<Tag> HandleTagListLogic(string uid)
        {
            return dta.getTagList(uid);

        }
        public List<Question> HandleQuestionListLogic(string uid)
        {
            return dta.getQuestionList(uid);
        }
        public List<Comment> HandleCommentListLogic(string uid)
        {
            return dta.getCommentList(uid);
        }
        public List<Answer> HandleAnswerListLogic(string uid)
        {
            return dta.getAnswerList(uid);
        }
        public bool HandleSaveTagLogica(string tag)
        {
            try
            {
                dta.saveTag(tag);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public bool HandleSaveQuestionLogic(Question question)
        {
            return dta.saveQuestion(question);
        }
        public bool HandleSaveCommentLogic(Comment comment)
        {
            return dta.saveComment(comment);
        }
        public bool HandleSaveAnswerLogic(Answer answer)
        {
            return dta.saveAnswer(answer);
        }
        public Tag HandleGetTag(string tagName)
        {
            return dta.getSpecificTag(tagName);
        }
        public Question HandleGetQuestion(int id)
        {
            return dta.getQuestion(id);
        }
        public Answer HandleGetAnswer(int id)
        {
            return dta.getAnswer(id);
        }
        public void HandleSaveUpVote(UpVote upVote)
        {
            dta.saveUpVote(upVote);
        }
        public void HandleSaveDownVote(DownVote downVote)
        {
            dta.saveDownVote(downVote);
        }
        public void HandleAddTagsToQuestion(Question question, int[] tags)
        {
            dta.addTagsToQuestion(question, tags);
        }
        public void HandleUpdateReputation(string userId, int repToAdd)
        {
            dta.updateUserReputation(userId, repToAdd);
        }
        public Comment HandleGetComment(int id)
        {
            return dta.getComment(id);
        }
    }

    class GeneralDataAccess : IDataAccess
    {
        ApplicationDbContext db;
        public GeneralDataAccess(ApplicationDbContext db)
        {
            this.db = db;
        }

        public List<Question> getQuestionList(string uid)
        {
            return db.Questions.ToList();
        }
        public List<Tag> getTagList(string uid)
        {
            return db.Tags.ToList();
        }
        public List<Answer> getAnswerList(string uid)
        {
            return db.Answers.ToList();
        }
        public List<Comment> getCommentList(string uid)
        {
            return db.Comments.ToList();
        }

        public bool saveQuestion(Question question)
        {
            if (!String.IsNullOrEmpty(question.Description))
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public void saveTag(string tag)
        {
            if (!String.IsNullOrEmpty(tag))
            {
                var tagNames = db.Tags.Select(t => t.Name).Distinct().ToList();
                int index = tagNames.IndexOf(tag);
                if(index == -1)
                {
                    Tag tagToSave = new Tag { Name = tag };
                    db.Tags.Add(tagToSave);
                    db.SaveChanges();
                }
            }
            throw new KeyNotFoundException("Empty Tag Name was entered");

        }
        public bool saveAnswer(Answer answer)
        {
            if (!String.IsNullOrEmpty(answer.Content))
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public bool saveComment(Comment comment)
        {
            if (!String.IsNullOrEmpty(comment.Content))
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public Tag getSpecificTag(string tagName)
        {
            var tag = db.Tags.FirstOrDefault(t => t.Name == tagName);
            return tag;
        }

        public Question getQuestion(int id)
        {
            var question = db.Questions.FirstOrDefault(q => q.Id == id);
            return question;
        }

        public Answer getAnswer(int id)
        {
            var answer = db.Answers.FirstOrDefault(a => a.Id == id);
            return answer;
        }
        public Comment getComment(int id)
        {
            var comment = db.Comments.FirstOrDefault(a => a.Id == id);
            return comment;
        }
        public void saveUpVote(UpVote upVote)
        {
            db.UpVotes.Add(upVote);
            db.SaveChanges();
        }
        public void saveDownVote(DownVote downVote)
        {
            db.DownVotes.Add(downVote);
            db.SaveChanges();
        }

        public void addTagsToQuestion(Question question, int[] tags)
        {
            if(tags != null)
            {
                foreach (var tagId in tags)
                {
                    Tag tag = db.Tags.FirstOrDefault(t => t.Id == tagId);
                    if (tag != null)
                    {
                        question.Tags.Add(tag);
                    }
                }
                db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void updateUserReputation(string userId, int repToAdd)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if(user != null)
            {
                user.Reputation += repToAdd;
            }
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
    class UserDataAccess : IDataAccess
    {
        ApplicationDbContext db;
        public UserDataAccess(ApplicationDbContext db)
        {
            this.db = db;
        }

        public List<Question> getQuestionList(string uid)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            var uQList = user.Questions.ToList();
            return uQList;


        }
        public List<Tag> getTagList(string uid)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            var questionList = db.Questions.Where(x => x.OwnerId == uid).ToList();
            var tagList = new List<Tag>();
            foreach (var question in questionList)
            {

                var userTagList = question.Tags.ToList();
                foreach (var tag in userTagList)
                {
                    tagList.Add(tag);
                }

            }
            return tagList;
        }
        public List<Answer> getAnswerList(string uid)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            var uQList = user.Answers.ToList();
            return uQList;
        }
        public List<Comment> getCommentList(string uid)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            var uQList = user.Comments.ToList();
            return uQList;
        }

        public bool saveQuestion(Question question)
        {
            if (!String.IsNullOrEmpty(question.Description))
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public void saveTag(string tag)
        {
            if (!String.IsNullOrEmpty(tag))
            {
                var tagNames = db.Tags.Select(t => t.Name).Distinct().ToList();
                int index = tagNames.IndexOf(tag);
                if (index == -1)
                {
                    Tag tagToSave = new Tag { Name = tag };
                    db.Tags.Add(tagToSave);
                    db.SaveChanges();
                }
            }
            throw new KeyNotFoundException("Empty Tag Name was entered");
        }
        public bool saveAnswer(Answer answer)
        {
            if (!String.IsNullOrEmpty(answer.Content))
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public bool saveComment(Comment comment)
        {
            if (!String.IsNullOrEmpty(comment.Content))
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public Tag getSpecificTag(string tagName)
        {
            var tag = db.Tags.FirstOrDefault(t => t.Name == tagName);
            return tag;
        }
        public Question getQuestion(int id)
        {
            var question = db.Questions.FirstOrDefault(q => q.Id == id);
            return question;
        }

        public Answer getAnswer(int id)
        {
            var answer = db.Answers.FirstOrDefault(a => a.Id == id);
            return answer;
        }
        public Comment getComment(int id)
        {
            var comment = db.Comments.FirstOrDefault(a => a.Id == id);
            return comment;
        }
        public void saveUpVote(UpVote upVote)
        {
            db.UpVotes.Add(upVote);
            db.SaveChanges();
        }
        public void saveDownVote(DownVote downVote)
        {
            db.DownVotes.Add(downVote);
            db.SaveChanges();
        }
        public void addTagsToQuestion(Question question, int[] tags)
        {
            foreach (var tagId in tags)
            {
                Tag tag = db.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tag != null)
                {
                    question.Tags.Add(tag);
                }
            }
            db.Entry(question).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void updateUserReputation(string userId, int repToAdd)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Reputation += repToAdd;
            }
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}