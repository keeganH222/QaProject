using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QaProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;

namespace QaProject.Models
{
    interface IDataAccess
    {
        List<Tag> getTagList(string uid);
        List<Question> getQuestionList(string uid);
        List<Comment> getCommentList(string uid);
        List<Answer> getAnswerList(string uid);
        bool saveTag(Tag tag);
        bool saveQuestion(Question question);
        bool saveAnswer(Answer answer);
        bool saveComment(Comment comment);
        

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
        public bool HandleSaveTagLogica(Tag tag)
        {
            return dta.saveTag(tag);
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

    }
    
    class GeneralDataAccess : IDataAccess
    {
        ApplicationDbContext db = new ApplicationDbContext();
       
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
        public bool saveTag(Tag tag)
        {
            if (!String.IsNullOrEmpty(tag.Name))
            {
                db.Tags.Add(tag);
                db.SaveChanges();
                return true;
            }
            return false;
            
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
    }
    class UserDataAccess : IDataAccess
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
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
        public bool saveTag(Tag tag)
        {
            if (!String.IsNullOrEmpty(tag.Name))
            {
                db.Tags.Add(tag);
                db.SaveChanges();
                return true;
            }
            return false;
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
    }
}