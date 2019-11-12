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
}