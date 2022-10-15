
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using lawzand.Models;
using lawzand.Utility;

using System.Web.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace lawzand.Controllers
{
   


   

   
    public class AdminpanelController : Controller
    {


        public string random()
        {
            Random random = new Random();
            int length = 10;
            var rString = "";
            for (var i = 0; i < length; i++)
            {
                rString += ((char)(random.Next(1, 26) + 64)).ToString().ToLower();
            }
            return rString;
        }


        lawzandEntities1 dbb = new lawzandEntities1();


        public static Image ResizeImageKeepAspectRatio(Image source, int width, int height)
        {
            Image result = null;

            try
            {
                if (source.Width != width || source.Height != height)
                {
                    // Resize image
                    float sourceRatio = (float)source.Width / source.Height;

                    using (var target = new Bitmap(width, height))
                    {
                        using (var g = System.Drawing.Graphics.FromImage(target))
                        {
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            // Scaling
                            float scaling;
                            float scalingY = (float)source.Height / height;
                            float scalingX = (float)source.Width / width;
                            if (scalingX < scalingY) scaling = scalingX; else scaling = scalingY;

                            int newWidth = (int)(source.Width / scaling);
                            int newHeight = (int)(source.Height / scaling);

                            // Correct float to int rounding
                            if (newWidth < width) newWidth = width;
                            if (newHeight < height) newHeight = height;

                            // See if image needs to be cropped
                            int shiftX = 0;
                            int shiftY = 0;

                            if (newWidth > width)
                            {
                                shiftX = (newWidth - width) / 2;
                            }

                            if (newHeight > height)
                            {
                                shiftY = (newHeight - height) / 2;
                            }

                            // Draw image
                            g.DrawImage(source, -shiftX, -shiftY, newWidth, newHeight);
                        }

                        result = new Bitmap(target);
                    }
                }
                else
                {
                    // Image size matched the given size
                    result = new Bitmap(source);
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }


        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
      
        public static Bitmap ResizeImage(Image image, int width, int height)
        {


            //  Bitmap bitmap = new Bitmap(width, height);
            //   using (Graphics graphics = Graphics.FromImage(bitmap))
            // {
            //  graphics.DrawImage(image, 0, 0, width, height);
            // }
            Bitmap IMG = (Bitmap)ResizeImageKeepAspectRatio(image, width, height);
        
            return IMG;
        }


        //public static Bitmap ResizeImage(Image image, int width, int height)
        //{

        //    var dest_Rect = new Rectangle(0, 0, width, height);
        //    var dest_Image = new Bitmap(width, height);

        //    dest_Image.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(dest_Image))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.Default;
        //        graphics.InterpolationMode = InterpolationMode.Default;
        //        graphics.SmoothingMode = SmoothingMode.Default;
        //        graphics.PixelOffsetMode = PixelOffsetMode.Default;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, dest_Rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }
        //    return dest_Image;
        //}



        // GET: Adminpanel


        public ActionResult SendNotification()
           {
            return View();
           }


        public ActionResult Info(int? id)
        {
            infotbl f = new infotbl();
            ViewBag.Title = "Info";
            if (id < 4)
            {
                var model = dbb.infotbls.Where(x => x.lang == id).FirstOrDefault();
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    
                    f.lang =int.Parse( id.ToString());
                    f.Id= int.Parse(id.ToString());

                    return View(f);
                }


            }
            else if(id>3)
            {
                return RedirectToAction("Info", "Home");
            }
            f.lang = 0;
          

            return View(f);
            

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Info(infotbl mode)
        {

            try
            {
                if(mode.Id>0 && mode.Id < 4)
                {

                    var rec = dbb.infotbls.Where(x => x.Id == mode.Id).FirstOrDefault();
                    if (rec != null)
                    {
                      
                        rec.About = mode.About;
                        rec.adress = mode.adress;
                        rec.contactdetail = mode.contactdetail;
                    }
                    else
                    {
                        infotbl f = new infotbl();
                        f.Id = mode.Id;
                      f.adress=  mode.adress;
                        f.About = mode.About;
                        f.lang = mode.lang;
                        f.contactdetail = mode.contactdetail;
                        dbb.infotbls.Add(f);
                    }
                    dbb.SaveChanges();
                }

              
            }
            catch
            {

            }

            return RedirectToAction("Info", new { id = "" });
        }


        public JsonResult deleteinfo(int lang)
        {
            string result = "";
            try
            {


                var rec = dbb.infotbls.Where(x => x.lang == lang).FirstOrDefault();
                if (rec != null)
                {
                    rec.adress = "";
                    rec.About = "";
                    rec.contactdetail = "";
                    rec.About = "";
                }
                dbb.SaveChanges();
                result = "Ok";

                
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return Json(result);
        }





        public ActionResult contact()
        {
           
                var model = dbb.contacttbls.FirstOrDefault();
                if (model != null)
                {
                    return View(model);
                }
               
             return View();
              


            


        }

        [HttpPost]
       
        public ActionResult contact(contacttbl model)
        {

            try
            {
              var rec=  dbb.contacttbls.FirstOrDefault();
                if (rec==null)
                {
                    contacttbl c = new contacttbl();
                    c.ID = 1;
                    c.email = model.email;
                    c.phone = model.phone;
                    c.fb = model.fb;
                    c.youtube = model.youtube;
                    c.othersocial = model.othersocial;
                    dbb.contacttbls.Add(c);
                }
                else
                {
                    rec.email = model.email;
                    rec.phone = model.phone;
                    rec.fb = model.fb;
                    rec.youtube = model.youtube;
                    rec.othersocial = model.othersocial;

                }
                dbb.SaveChanges();

            }
            catch(Exception ex)
            {
                //return RedirectToAction(ex.Message);
            }

            return RedirectToAction("contact");
        }









        public ActionResult Index()
        {
            ViewBag.Title = "Index";
            return View();

        }





        public ActionResult Projects()
        {
            ViewBag.Title = "Projects";
            return View();

        }


        public ActionResult Addproject(decimal? id)
        {
            if (id == null)
            {

                ViewBag.result = "";
                projecttbl model = new projecttbl();
                model.projectId = decimal.Parse(db.getmax("projecttbl", "projectId"));
                ViewBag.Title = "Add-Project";
                return View(model);
            }
            else
            {
                var model = dbb.projecttbls.Where(x => x.projectId == id).FirstOrDefault();
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    ViewBag.result = "";
                    projecttbl model2 = new projecttbl();
                    model2.projectId = decimal.Parse(db.getmax("projecttbl", "projectId"));
                    ViewBag.Title = "Add-Project";
                    return View(model2);

                }
            }

            //List<companyviewmodel> company = new List<companyviewmodel>();
            //foreach(companytbl aa in dbb.companytbls.ToList())
            //{
            //    company.Add(new companyviewmodel { companyId = aa.companyId, companyName = aa.companyname });
            //}

            //ViewBag.company = company;

           
        }


        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Addproject(projecttbl model, FormCollection fr,decimal?id)
        {
            ViewBag.result = "";
            try
            {

                model.lang = int.Parse(fr["lang"]);
                string rand = random();
                if (id == null)
                {
                   
                        model.company = int.Parse(fr["lang"]);
                        projecttbl tbl = new projecttbl();
                        tbl.projectId = model.projectId;
                        tbl.lang = model.lang;
                        tbl.company = model.company;
                        tbl.projectname = model.projectname;
                        tbl.projectcontent = model.projectcontent;
                        HttpPostedFileBase postedFile = Request.Files["projectpic"];

                    string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };


                    if (ext.Contains(Path.GetExtension(postedFile.FileName)))
                    {

                        string path = Server.MapPath("~/Content/images/project/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string path2 = Server.MapPath("~/Content/images/project/thumb/");
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);
                        }







                        WebImage org = new WebImage(postedFile.InputStream);
                        WebImage img = org;
                        if (org.Width > 1500)
                        {
                            org.Resize(1700, 1000);
                        }
                      
                        if (img.Width > 1000)
                        {
                            img.Resize(950, 950);
                        }
                        org.Save(path + model.projectId+DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName));
                        img.Save((path2 + model.projectId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName)));


                        model.projectpic = "project/" + model.projectId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);
                        model.thumb = "project/thumb/" + model.projectId+DateTime.Now.DayOfWeek+ rand + Path.GetExtension(postedFile.FileName);

                        dbb.projecttbls.Add(model);
                        dbb.SaveChanges();

                        ViewBag.result = "Ok";
                    }

                    else
                    {
                        ViewBag.result = "You Must select File";
                    }
                }


                else
                {


                    HttpPostedFileBase postedFile = Request.Files["projectpic"];

                    string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };

                    var rec = dbb.projecttbls.Where(x => x.projectId == id).FirstOrDefault();
                    if (rec != null)
                    {
                        rec.projectname = model.projectname;
                        rec.projectcontent = model.projectcontent;
                        rec.lang = model.lang;
                        if (model.projectpic != null && ext.Contains(Path.GetExtension(postedFile.FileName)))
                        {
                            string fullPath = Request.MapPath("~/Content/images/" + rec.projectpic);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }

                            string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
                            if (System.IO.File.Exists(fullPath2))
                            {
                                System.IO.File.Delete(fullPath2);
                            }



                            string path = Server.MapPath("~/Content/images/project/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string path2 = Server.MapPath("~/Content/images/project/thumb/");
                            if (!Directory.Exists(path2))
                            {
                                Directory.CreateDirectory(path2);
                            }



                            WebImage org = new WebImage(postedFile.InputStream);
                            WebImage img = org;
                            if (org.Width > 1500)
                            {
                                org.Resize(1500, 1000);
                            }

                            if (img.Width > 1000)
                            {
                                img.Resize(950, 950);
                            }
                            org.Save(path + model.projectId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName));
                            img.Save((path2 + model.projectId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName)));


                            model.projectpic = "project/"+model.projectId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);
                            model.thumb = "project/thumb/"+model.projectId+DateTime.Now.DayOfWeek + DateTime.Now.Minute  + Path.GetExtension(postedFile.FileName);

                            rec.projectpic = model.projectpic;
                            rec.thumb = model.thumb;

                        }
                        else
                        {
                            model.projectpic = rec.projectpic;
                            model.thumb = rec.thumb;
                        }

                        dbb.SaveChanges();
                        ViewBag.result = "Ok";
                    }



                }


            }
            catch (Exception ex)
            {
                ViewBag.result = ex.Message;
            }

            //List<companyviewmodel> company = new List<companyviewmodel>();
            //foreach (companytbl aa in dbb.companytbls.ToList())
            //{
            //    company.Add(new companyviewmodel { companyId = aa.companyId, companyName = aa.companyname });
            //}

            //ViewBag.company = company;

            if (ViewBag.result == "Ok")
            {
                System.Threading.Thread.Sleep(4000);
                return RedirectToAction("Uploadtoproject", new { id = model.projectId.ToString("G29") });
            }
            else
                return View(model);
        }


        
        [HttpPost]
        public async Task<JsonResult> deleteproject(decimal pid)
        {

            string result = "";
            try
            {
                var rec = dbb.projecttbls.Where(x => x.projectId == pid).FirstOrDefault();
                if (rec != null)
                {
                    dbb.projecttbls.Remove(rec);
                }



                await dbb.SaveChangesAsync();
                string fullPath = Request.MapPath("~/Content/images/" + rec.projectpic);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
                if (System.IO.File.Exists(fullPath2))
                {
                    System.IO.File.Delete(fullPath2);
                }


          foreach(gallerytbl rec2 in dbb.gallerytbls.Where(x => x.projectId == pid).ToList()) {

                    if (rec2 != null)
                    {
                        dbb.gallerytbls.Remove(rec2);
                    }



                    await dbb.SaveChangesAsync();
                    string fullPath33 = Request.MapPath("~/Content/images/" + rec2.pic);
                    if (System.IO.File.Exists(fullPath33))
                    {
                        System.IO.File.Delete(fullPath33);
                    }
                }
               


                result = "deleted succefully";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }





            return Json(result);
        }


        public async Task<ActionResult> Uploadtoproject(decimal? id)
        {
            if (id == null)
            {
                return RedirectToAction("Projects");
            }
            ViewBag.result = "";
            projecttbl model =await dbb.projecttbls.Where(x => x.projectId == id).FirstOrDefaultAsync();
            
            return View(model);
        }



        public ActionResult Filesave(FileUpload fileupload)
        {
            try
            {
                if (fileupload.Files.Count() > 0)
                {
                    foreach (var file in fileupload.Files)
                    {
                        string filename = file.FileName;
                        var ext = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
                        if (ext.ToLower() == "jpeg" || ext.ToLower() == "jpg" || ext.ToLower() == "png")
                        {

                            string path1 = Server.MapPath("~/Content/images/project/gallery/"+fileupload.Name+"/");
                            if (!Directory.Exists(path1))
                            {
                                Directory.CreateDirectory(path1);
                            }
                            WebImage org = new WebImage(file.InputStream);
                            if (org.Width > 1900)
                            {
                                org.Resize(1700, 1200);
                            }
                            string path = Server.MapPath("~/Content/images/project/gallery/");
                            org.Save(path + fileupload.Name + "/" + filename );


                           // string path = Server.MapPath("~/Content/images/project/gallery/" + fileupload.Name + "/" + filename);
                           // file.SaveAs(path);
                            // Todo: for database 
                            string uploadedBy = fileupload.Name;
                            string FilePath = path;
                            //Save fields to database
                            //

                            gallerytbl model = new gallerytbl();
                            model.galleryId = decimal.Parse(db.getmax("gallerytbl", "galleryId"));
                            model.projectId =decimal.Parse(fileupload.Name);
                            model.pic = "project/gallery/" + fileupload.Name + "/" + filename;
                            dbb.gallerytbls.Add(model);
                            dbb.SaveChanges();



                        }
                    }
                    return Json("data saved");
                }
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }

            return Json("something went wrong");
        }


        [HttpPost]
        public async Task<JsonResult> deletegall(decimal gid)
        {

            string result = "";
            try
            {
                var rec = dbb.gallerytbls.Where(x => x.galleryId == gid).FirstOrDefault();
                if (rec != null)
                {
                    dbb.gallerytbls.Remove(rec);
                }



                await dbb.SaveChangesAsync();
                string fullPath = Request.MapPath("~/Content/images/" + rec.pic);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

              



                result = "deleted succefully";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }





            return Json(result);
        }





        public ActionResult Service()
        {
            ViewBag.Title = "Service";
            return View();

        }


        public ActionResult Addservice(decimal?id)
        {
            if (id == null)
            {
                ViewBag.result = "";
                servicetbl model = new servicetbl();
                model.serviceId = int.Parse(db.getmax("servicetbl", "serviceId"));

                ViewBag.Title = "Add-Service";
                return View(model);
            }
            else
            {
                var model = dbb.servicetbls.Where(x => x.serviceId == id).FirstOrDefault();
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    ViewBag.result = "";
                    var model2 = new servicetbl();
                    model2.serviceId = int.Parse(db.getmax("servicetbl", "serviceId"));

                    ViewBag.Title = "Add-Service";
                    return View(model2);

                }
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Addservice(servicetbl model, FormCollection fr,decimal?id)
        {
            //ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            //// Create an Encoder object based on the GUID  
            //// for the Quality parameter category.  
            //System.Drawing.Imaging.Encoder myEncoder =
            //    System.Drawing.Imaging.Encoder.Quality;

            //// Create an EncoderParameters object.  
            //// An EncoderParameters object has an array of EncoderParameter  
            //// objects. In this case, there is only one  
            //// EncoderParameter object in the array.  
            //EncoderParameters myEncoderParameters = new EncoderParameters(1);

            //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 28L);
            //myEncoderParameters.Param[0] = myEncoderParameter;


            ViewBag.result = "";
            try
            {
                model.lang = int.Parse(fr["lang"]);
                string rand = random();
                if (id == null)
                {
                    servicetbl tbl = new servicetbl();
                    tbl.serviceId = model.serviceId;
                    tbl.lang = model.lang;
                    tbl.servicename = model.servicename;
                    tbl.servicecontent = model.servicecontent;
                    HttpPostedFileBase postedFile = Request.Files["servicepic"];
                    string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };


                    if (ext.Contains(Path.GetExtension(postedFile.FileName)))
                    {


                        string path = Server.MapPath("~/Content/images/service/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string path2 = Server.MapPath("~/Content/images/service/thumb/");
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);
                        }



                        WebImage org = new WebImage(postedFile.InputStream);
                        WebImage img = org;
                        if (org.Width > 1500)
                        {
                            org.Resize(1500, 1000);
                        }

                        if (img.Width > 1000)
                        {
                            img.Resize(950, 950);
                        }
                        //Bitmap bmp = new Bitmap(postedFile.InputStream);
                        //Bitmap thumb = bmp;
                        //if (bmp.Width > 1200)
                        //{
                        //    bmp = ResizeImage(bmp, 1200, bmp.Height);
                        //}

                        //if (thumb.Width > 750)
                        //{
                        //    thumb = ResizeImage(thumb, 700, bmp.Height);
                        //}

                        //bmp.Save(path + model.serviceId + Path.GetExtension(postedFile.FileName));
                        //thumb.Save((path2 + model.serviceId + Path.GetExtension(postedFile.FileName)));


                        //bmp.Save(path + model.serviceId + Path.GetExtension(postedFile.FileName), jpgEncoder, myEncoderParameters);
                        //  thumb.Save((path2 + model.serviceId + Path.GetExtension(postedFile.FileName)), jpgEncoder, myEncoderParameters);
                        org.Save(path + model.serviceId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName));
                          img.Save((path2 + model.serviceId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName)));


                        model.servicepic = "service/" + model.serviceId + DateTime.Now.DayOfWeek + rand+  Path.GetExtension(postedFile.FileName);
                        model.thumb = "service/thumb/" + model.serviceId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);

                        dbb.servicetbls.Add(model);
                        dbb.SaveChanges();
                        ViewBag.result = "Ok";



                    }

                    else
                    {
                        ViewBag.result = "You Must select File";
                    }
                }
                else
                {


                    HttpPostedFileBase postedFile = Request.Files["servicepic"];

                    string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };

                    var rec = dbb.servicetbls.Where(x => x.serviceId == id).FirstOrDefault();
                    if (rec != null)
                    {
                        rec.servicename = model.servicename;
                        rec.servicecontent = model.servicecontent;
                        rec.lang = model.lang;
                        if (model.servicepic != null && ext.Contains(Path.GetExtension(postedFile.FileName)))
                        {
                            string fullPath = Request.MapPath("~/Content/images/" + rec.servicepic);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }

                            string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
                            if (System.IO.File.Exists(fullPath2))
                            {
                                System.IO.File.Delete(fullPath2);
                            }



                            string path = Server.MapPath("~/Content/images/service/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string path2 = Server.MapPath("~/Content/images/service/thumb/");
                            if (!Directory.Exists(path2))
                            {
                                Directory.CreateDirectory(path2);
                            }



                            WebImage org = new WebImage(postedFile.InputStream);
                            WebImage img = org;
                            if (org.Width > 1500)
                            {
                                org.Resize(1500, 1000);
                            }

                            if (img.Width > 1000)
                            {
                                img.Resize(950, 950);
                            }

                            org.Save(path + model.serviceId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName));
                            img.Save((path2 + model.serviceId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName)));


                            model.servicepic = "service/" + model.serviceId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);
                            model.thumb = "service/thumb/" + model.serviceId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);

                            rec.servicepic = model.servicepic;
                            rec.thumb = model.thumb;

                        }
                        else
                        {
                            model.servicepic = rec.servicepic;
                            model.thumb = rec.thumb;
                        }

                        dbb.SaveChanges();
                        ViewBag.result = "Ok";
                    }



                }



            }
            catch (Exception ex)
            {
                ViewBag.result = ex.Message;
            }
            if (ViewBag.result == "Ok")
                return RedirectToAction("Service", "Adminpanel");
            else
                return View(model);
        }


        [Route("uplader")]
        [HttpPost]
        public ActionResult uploadckeditor(HttpPostedFile file)
        {


            //string path2 = Server.MapPath("~/Content/images/Editor/");
            //if (!Directory.Exists(path2))
            //{
            //    Directory.CreateDirectory(path2);
            //}
            //WebImage img = new WebImage(file.InputStream);
            //if (img.Width > 1000)
            //{
            //    img.Resize(750, 750);
            //}

            //img.Save(path2 + file.FileName + Path.GetExtension(file.FileName));
            return  Json(new { path = "/Content/images/Editor/" + file.FileName + Path.GetExtension(file.FileName) });

        }


        [HttpPost]
       
        public async Task<JsonResult> deleteservice(decimal sid)
        {

            string result = "";
            try
            {
                var rec = dbb.servicetbls.Where(x => x.serviceId == sid).FirstOrDefault();
                if (rec != null)
                {
                    dbb.servicetbls.Remove(rec);
                }



                await dbb.SaveChangesAsync();
                string fullPath = Request.MapPath("~/Content/images/" + rec.servicepic);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
                if (System.IO.File.Exists(fullPath2))
                {
                    System.IO.File.Delete(fullPath2);
                }



                result = "deleted succefully";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }





            return Json(result);
        }







        //public ActionResult Company()
        //{
        //    ViewBag.Title = "Company";
        //    return View();

        //}

        //public ActionResult Addcompany(int? id)
        //{
        //    if (id == null)
        //    {
        //        ViewBag.result = "";
        //        companytbl model = new companytbl();
        //        model.companyId = int.Parse(db.getmax("companytbl", "companyId"));

        //        ViewBag.Title = "Add-Company";
        //        return View(model);
        //    }
        //    else
        //    {
        //        var model = dbb.companytbls.Where(x => x.companyId == id).FirstOrDefault();
        //        if (model != null)
        //        {
        //            return View(model);
        //        }
        //        else
        //        {
        //            ViewBag.result = "";
        //            companytbl model2 = new companytbl();
        //            model2.companyId = int.Parse(db.getmax("companytbl", "companyId"));

        //            ViewBag.Title = "Add-Company";
        //            return View(model2);

        //        }

        //    }
        //}


        //[ValidateInput(false)]
        //[HttpPost]
        //public ActionResult Addcompany(companytbl model, FormCollection fr, int? id)
        //{
        //    ViewBag.result = "";
        //    try
        //    {
        //        model.lang= int.Parse(fr["lang"]); 

        //        if (id == null)
        //        {



                   
        //                companytbl tbl = new companytbl();
        //                tbl.companyId = model.companyId;
        //                tbl.lang = int.Parse(fr["lang"]);
        //                tbl.companyname = model.companyname;
        //                tbl.companycontent = model.companycontent;
        //                HttpPostedFileBase postedFile = Request.Files["companypic"];

        //            string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };
                   

        //            if(ext.Contains(Path.GetExtension(postedFile.FileName))) { 





        //                string path = Server.MapPath("~/Content/images/company/");
        //                if (!Directory.Exists(path))
        //                {
        //                    Directory.CreateDirectory(path);
        //                }

        //                string path2 = Server.MapPath("~/Content/images/company/thumb/");
        //                if (!Directory.Exists(path2))
        //                {
        //                    Directory.CreateDirectory(path2);
        //                }





        //                postedFile.SaveAs(path + model.companyId + Path.GetExtension(postedFile.FileName));
        //                model.companypic = "company/" + model.companyId + Path.GetExtension(postedFile.FileName);


        //                WebImage img = new WebImage(postedFile.InputStream);
        //                img.Resize(500, 500);
        //                img.Save((path2 + model.companyId + Path.GetExtension(postedFile.FileName)));

        //                model.thumb = "company/thumb/" + model.companyId + Path.GetExtension(postedFile.FileName);

        //                dbb.companytbls.Add(model);
        //                dbb.SaveChanges();
        //                ViewBag.result = "Ok";
        //            }

        //            else
        //            {
        //                ViewBag.result = "You Must select Picture";
        //            }
        //        }

        //        else
        //        {


        //            HttpPostedFileBase postedFile = Request.Files["companypic"];

        //            string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };

        //            var rec = dbb.companytbls.Where(x => x.companyId == id).FirstOrDefault();
        //            if (rec != null)
        //            {
        //                rec.companyname = model.companyname;
        //                rec.companycontent = model.companycontent;
        //                rec.lang = model.lang;
        //                if (model.companypic != null && ext.Contains(Path.GetExtension(postedFile.FileName)))
        //                {
        //                    string fullPath = Request.MapPath("~/Content/images/" + rec.companypic);
        //                    if (System.IO.File.Exists(fullPath))
        //                    {
        //                        System.IO.File.Delete(fullPath);
        //                    }

        //                    string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
        //                    if (System.IO.File.Exists(fullPath2))
        //                    {
        //                        System.IO.File.Delete(fullPath2);
        //                    }



        //                    string path = Server.MapPath("~/Content/images/company/");
        //                    if (!Directory.Exists(path))
        //                    {
        //                        Directory.CreateDirectory(path);
        //                    }

        //                    string path2 = Server.MapPath("~/Content/images/company/thumb/");
        //                    if (!Directory.Exists(path2))
        //                    {
        //                        Directory.CreateDirectory(path2);
        //                    }





        //                    postedFile.SaveAs(path + model.companyId + Path.GetExtension(postedFile.FileName));
        //                    model.companypic = "company/" + model.companyId + Path.GetExtension(postedFile.FileName);


        //                    WebImage img = new WebImage(postedFile.InputStream);
        //                    img.Resize(500, 500);
        //                    img.Save((path2 + model.companyId + Path.GetExtension(postedFile.FileName)));

        //                    model.thumb = "company/thumb/" + model.companyId + Path.GetExtension(postedFile.FileName);

        //                    rec.companypic = model.companypic;
        //                    rec.thumb = model.thumb;

        //                }
        //                else
        //                {
        //                    model.companypic =  rec.companypic;
        //                    model.thumb =  rec.thumb;
        //                }

        //                dbb.SaveChanges();
        //            }


                  
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.result = ex.Message;
        //    }
        //    if (ViewBag.result == "Ok")
        //        return RedirectToAction("Addcompany", "Adminpanel");
        //    else
        //        return View(model);
        //}


        //[HttpPost]

        //public async Task<JsonResult> deletecompany(int cid)
        //{

        //    string result = "";
        //    try
        //    {
        //        var rec = dbb.companytbls.Where(x => x.companyId == cid).FirstOrDefault();
        //        if (rec != null)
        //        {
        //            dbb.companytbls.Remove(rec);
        //        }



        //        await dbb.SaveChangesAsync();
        //        string fullPath = Request.MapPath("~/Content/images/" + rec.companypic);
        //        if (System.IO.File.Exists(fullPath))
        //        {
        //            System.IO.File.Delete(fullPath);
        //        }

        //        string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
        //        if (System.IO.File.Exists(fullPath2))
        //        {
        //            System.IO.File.Delete(fullPath2);
        //        }



        //        result = "deleted succefully";
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //    }





        //    return Json(result);
        //}






        public ActionResult partners()
        {
            ViewBag.Title = "Partners";
            return View();
           
        }


        public ActionResult Addpartners(decimal? id)
        {
            

            if (id == null)
            {
                ViewBag.result = "";
                partnertbl model = new partnertbl();
                model.partnerId = decimal.Parse(db.getmax("partnertbl", "partnerId"));

                ViewBag.Title = "Add-Partner";
                return View(model);
            }
            else
            {
                var model = dbb.partnertbls.Where(x => x.partnerId == id).FirstOrDefault();
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    ViewBag.result = "";
                    partnertbl model2 = new partnertbl();
                    model2.partnerId = decimal.Parse(db.getmax("partnertbl", "partnerId"));

                    ViewBag.Title = "Add-Partner";
                    return View(model2);

                }

            }



          


        }

        [HttpPost]
        public ActionResult Addpartners(partnertbl model, FormCollection fr,decimal?id)
        {
            ViewBag.result = "";
            try
            {
                model.lang = int.Parse(fr["lang"]);
                string rand = random();
                if (id == null)
                {




                    partnertbl tbl = new partnertbl();
                    tbl.partnerId = model.partnerId;
                    tbl.lang = int.Parse(fr["lang"]);
                    tbl.partnername = model.partnername;
                    tbl.partnerlink = model.partnerlink;
                    HttpPostedFileBase postedFile = Request.Files["partnerpic"];

                    string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };


                    if (ext.Contains(Path.GetExtension(postedFile.FileName)))
                    {





                        string path = Server.MapPath("~/Content/images/partners/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string path2 = Server.MapPath("~/Content/images/partners/thumb/");
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);
                        }

                        WebImage org = new WebImage(postedFile.InputStream);
                        WebImage img = org;
                        if (org.Width > 1500)
                        {
                            org.Resize(1500, 1000);
                        }

                        if (img.Width > 1000)
                        {
                            img.Resize(950, 950);
                        }

                        org.Save(path + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName));
                        img.Save((path2 + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName)));


                        model.partnerpic = "partners/" + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);


                      
                        
                        model.thumb = "partners/thumb/" + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);

                        dbb.partnertbls.Add(model);
                        dbb.SaveChanges();
                        ViewBag.result = "Ok";
                    }

                    else
                    {
                        ViewBag.result = "You Must select Picture";
                    }
                }

                else
                {


                    HttpPostedFileBase postedFile = Request.Files["partnerpic"];

                    string[] ext = { ".jpg", ".JPG", ".bmp", ".png", ".jpeg", ".gif" };

                    var rec = dbb.partnertbls.Where(x => x.partnerId == id).FirstOrDefault();
                    if (rec != null)
                    {
                        rec.lang = model.lang;
                        rec.partnername = model.partnername;
                        rec.partnerlink = model.partnerlink;
                        if (model.partnerpic != null && ext.Contains(Path.GetExtension(postedFile.FileName)))
                        {
                            string fullPath = Request.MapPath("~/Content/images/" + rec.partnerpic);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }

                            string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
                            if (System.IO.File.Exists(fullPath2))
                            {
                                System.IO.File.Delete(fullPath2);
                            }



                            string path = Server.MapPath("~/Content/images/partners/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string path2 = Server.MapPath("~/Content/images/partners/thumb/");
                            if (!Directory.Exists(path2))
                            {
                                Directory.CreateDirectory(path2);
                            }



                            WebImage org = new WebImage(postedFile.InputStream);
                            WebImage img = org;
                            if (org.Width > 1500)
                            {
                                org.Resize(1500, 1000);
                            }

                            if (img.Width > 1000)
                            {
                                img.Resize(950, 950);
                            }

                            org.Save(path + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName));
                            img.Save((path2 + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName)));



                            model.partnerpic = "partners/" + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);


                           
                            model.thumb = "partners/thumb/" + model.partnerId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);

                            rec.partnerpic = model.partnerpic;
                            rec.thumb = model.thumb;

                        }
                        else
                        {
                            model.partnerpic = rec.partnerpic;
                            model.thumb = rec.thumb;
                        }

                        dbb.SaveChanges();
                        ViewBag.result = "Ok";
                    }



                }

            }
            catch (Exception ex)
            {
                ViewBag.result = ex.Message;
            }
            if (ViewBag.result == "Ok")
                return RedirectToAction("Partners", "Adminpanel");
            else
                return View(model);
        }


        [HttpPost]
      
        public async Task<JsonResult> deletepartner(decimal pid)
        {

            string result = "";
            try
            {
                var rec = dbb.partnertbls.Where(x => x.partnerId == pid).FirstOrDefault();
                if (rec != null)
                {
                    dbb.partnertbls.Remove(rec);
                }



                await dbb.SaveChangesAsync();
                string fullPath = Request.MapPath("~/Content/images/" + rec.partnerpic);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                string fullPath2 = Request.MapPath("~/Content/images/" + rec.thumb);
                if (System.IO.File.Exists(fullPath2))
                {
                    System.IO.File.Delete(fullPath2);
                }



                result = "deleted succefully";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }





            return Json(result);
        }







        public ActionResult Slider()
        {
            ViewBag.Title = "Slider";
            return View();

        }


        public ActionResult Addslider()
        {
            ViewBag.result = "";
            slidertbl model = new slidertbl();
            model.sliderId = int.Parse(db.getmax("slidertbl", "sliderId"));

            ViewBag.Title = "Add-Slider";
            return View(model);
        }



        [HttpPost]
        public ActionResult Addslider(slidertbl model,FormCollection fr)
        {
            ViewBag.result = "";
            try
            {
                string rand = random();
                if (Request.Files.Count > 0)
                {
                    slidertbl tbl = new slidertbl();
                    tbl.sliderId = model.sliderId;
                    tbl.lang = int.Parse(fr["lang"]);
                    tbl.slidercontent = model.slidercontent;
                    tbl.link = model.link;
                    HttpPostedFileBase postedFile = Request.Files["sliderpic"];



                    string path = Server.MapPath("~/Content/images/sliders/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }


                    WebImage org = new WebImage(postedFile.InputStream);
                    //if (org.Width > 2000)
                    //{
                    //    org.Resize(2000,1200);
                    //}
                  
                    org.Save(path + model.sliderId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName));

                   
                    model.sliderpic = "sliders/" + model.sliderId + DateTime.Now.DayOfWeek + rand + Path.GetExtension(postedFile.FileName);
                    dbb.slidertbls.Add(model);
                    dbb.SaveChanges();
                    ViewBag.result = "Ok";
                }
           
                else
                {
                    ViewBag.result = "You Must select File";
                }

            }
            catch (Exception ex)
            {
                ViewBag.result =ex.Message;
            }
            if (ViewBag.result == "Ok")
                return RedirectToAction("Slider", "Adminpanel");
            else
                return View(model);
        }



        [HttpPost]
      
        // [ValidateAntiForgeryToken]
        public async Task<JsonResult> deleteslider(int slider)
        {

            string result = "";
            try
            {
                var rec = dbb.slidertbls.Where(x => x.sliderId == slider).FirstOrDefault();
                if (rec != null)
                {
                    dbb.slidertbls.Remove(rec);
                }

               

              await  dbb.SaveChangesAsync();
                string fullPath = Request.MapPath("~/Content/images/" + rec.sliderpic);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }



                result = "deleted succefully";
            }catch(Exception ex)
            {
                result = ex.Message;
            }





            return Json(result);
        }





    }
}