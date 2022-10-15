using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lawzand.Models;

namespace lawzand.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

       



        public ActionResult Service(decimal? id)
        {
            if (id == null) return RedirectToAction("Index");
            using (lawzandEntities1 dbb=new lawzandEntities1())
            {

                var model = dbb.servicetbls.Where(x => x.serviceId == id).FirstOrDefault();
                if (model != null)
                {
                    string lang = dbb.langtbls.Where(x => x.id == model.lang).FirstOrDefault().language.ToLower();
                    if(lang.Contains("arabic")|| lang.Contains("english"))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return View(model);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
  
        }


        public ActionResult Project(decimal? id)
        {
            if (id == null) return RedirectToAction("Index");

            using (lawzandEntities1 dbb = new lawzandEntities1())
            {

                var model = dbb.projecttbls.Where(x => x.projectId == id).FirstOrDefault();
                if (model != null)
                {
                    string lang = dbb.langtbls.Where(x => x.id == model.lang).FirstOrDefault().language.ToLower();
                    if (lang.Contains("arabic") || lang.Contains("english"))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return View(model);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

        }


        public ActionResult AllServices()
        {
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {
                return View(dbb.servicetbls.Where(x => x.lang == 1).OrderByDescending(x => x.serviceId).ToList());

               }
        }

        public ActionResult AllProjects()
        {
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {
                return View(dbb.projecttbls.Where(x => x.lang == 1).OrderByDescending(x => x.projectId).ToList());

            }
        }



        public ActionResult InvalidInput()
        {
            return View();
        }



        public ActionResult Gallery()
        {
          

            using (lawzandEntities1 dbb = new lawzandEntities1())
            {

                

                return View(dbb.projectviews.Where(x => x.lang == 1).OrderBy(r => Guid.NewGuid()).Take(40).ToList());

            }
        }


        public ActionResult About()
        {
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {
                var model = dbb.infotbls.Where(x => x.lang == 1).FirstOrDefault();
                return View(model);
            }

        }


        public ActionResult Contact()
        {
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {
                var model = dbb.infotbls.Where(x => x.lang == 1).FirstOrDefault();
                return View(model);
            }

        }
    }
}