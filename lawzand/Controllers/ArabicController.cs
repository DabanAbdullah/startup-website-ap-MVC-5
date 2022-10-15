using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lawzand.Models;

namespace lawzand.Controllers
{
    public class ArabicController : Controller
    {
        // GET: Arabic
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Service(decimal? id)
        {
            string CurrentURL = Request.Url.AbsoluteUri;
            if (id == null) return RedirectToAction("Index");
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {

                var model = dbb.servicetbls.Where(x => x.serviceId == id).FirstOrDefault();
                if (model != null)
                {

                    if (CurrentURL.ToLower().Contains(dbb.langtbls.Where(x => x.id == model.lang).FirstOrDefault().language.ToLower()))
                    {

                    }
                    else
                    {

                        return RedirectToAction("Index", "Arabic");
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
            string CurrentURL = Request.Url.AbsoluteUri;
            if (id == null) return RedirectToAction("Index");

            using (lawzandEntities1 dbb = new lawzandEntities1())
            {

                var model = dbb.projecttbls.Where(x => x.projectId == id).FirstOrDefault();
                 if (model != null)
                    {

                        if (CurrentURL.ToLower().Contains(dbb.langtbls.Where(x => x.id == model.lang).FirstOrDefault().language.ToLower()))
                        {

                        }
                        else
                        {

                            return RedirectToAction("Index", "Arabic");
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
                return View(dbb.servicetbls.Where(x => x.lang == 3).OrderByDescending(x => x.serviceId).ToList());

            }
        }

        public ActionResult AllProjects()
        {
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {
                return View(dbb.projecttbls.Where(x => x.lang == 3).OrderByDescending(x => x.projectId).ToList());

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



                return View(dbb.projectviews.Where(x => x.lang == 3).OrderBy(r => Guid.NewGuid()).Take(40).ToList());

            }
        }


        public ActionResult About()
        {
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {
                var model = dbb.infotbls.Where(x => x.lang == 3).FirstOrDefault();
                return View(model);
            }

        }

        public ActionResult Contact()
        {
            using (lawzandEntities1 dbb = new lawzandEntities1())
            {
                var model = dbb.infotbls.Where(x => x.lang == 3).FirstOrDefault();
                return View(model);
            }

        }
    }
}