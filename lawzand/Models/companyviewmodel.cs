using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lawzand.Models
{
    public class companyviewmodel
    {


        public int companyId { get; set; }
        public string companyName { get; set; }
    }

    public class FileUpload
    {
        public List<HttpPostedFileBase> Files { get; set; }
        public string Name { get; set; }
    }
}