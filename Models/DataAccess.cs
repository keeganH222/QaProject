using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QaProject.Models
{
     interface IDataAccess
    {
        List<Tag> getTagList();
    }
    public class DataAccess
    {
    }
}