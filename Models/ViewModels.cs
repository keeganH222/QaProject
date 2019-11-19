using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
    public class ViewModels
    {
        
    }
    public class TagViewModel
    {
        public int tagId { get; set; }
        [Display(Name = "Tag")]
        public string tagName { get; set; }
        public bool IsChecked { get; set; }
    }
    public class AddingCommentViewModel
    {
        public int itemId { get; set; }
        public string itemType { get; set; }
        public int commentCount { get; set; }
    }
    public class AddingAnswerViewModel
    {
        public int itemId { get; set; }
        public int answerCount { get; set; }
    }
    public class CommentViewModel
    {
        public Comment comment { get; set; }
        public string userName { get; set; }
    }
    public class AnswerViewModel
    {
        public Answer answer { get; set; }
        public string userName { get; set; }
    }
    public class ButtonViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
    }
}