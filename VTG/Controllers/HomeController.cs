using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using VTG.Models;

namespace VTG.Controllers
{
    public class HomeController : Controller
    {
        public dbVTGEntities db = new dbVTGEntities();
        public class TempOrganize
        {
            public int rank { get; set; }
            public long regionId { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public string type { get; set; }
            public string geoType { get; set; }
        }

        //POST:OrganizePlan
        [Authorize(Roles = "Tourists")]
        [HttpPost]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult OrganizePlanAddRegion(List<long> list, long tourPlanId)
        {
            string currentUser = User.Identity.GetUserId();
            TourPlan tourPlan = db.TourPlans.Find(new object[] { tourPlanId, currentUser });
            tourPlan.Organizes.Clear();
            if(tourPlan != null && list != null)
            {
                int rank = 1;
                foreach (var regionId in list)
                {
                    if (regionId != 0)
                    {
                        tourPlan.Organizes.Add(new VTG.Models.Organize { Rank = rank, RegionId = regionId });
                    }
                    rank++;
                }
            }            
            db.SaveChanges();

            List<TempOrganize> tempOrganizes = new List<TempOrganize>();
            foreach (var item in tourPlan.Organizes)
            {
                var touristRegion = db.TouristRegions.Find(item.RegionId);
                tempOrganizes.Add(new TempOrganize
                {
                    rank = item.Rank,
                    regionId = item.RegionId,
                    lat = touristRegion.Lat,
                    lng = touristRegion.Lng,
                    name = touristRegion.Name,
                    address = touristRegion.Address,
                    type = touristRegion.Type,
                    geoType = touristRegion.GeoType
                });
            }
            return Json(tempOrganizes);
        }
        
        
        //GET:_imageSlider partial view
        [Authorize(Roles = "Tourists")]
        [HttpGet]
        public PartialViewResult _ImagesSlider(long? regionId)
        {            
            TouristRegion touristRegion = db.TouristRegions.Find(regionId);
            return PartialView(touristRegion);
        }
        [Authorize(Roles = "Tourists")]
        [HttpGet]
        public PartialViewResult _TouristRegionsSlider(long tourPlanId, string search)
        {
            List<TouristRegion> touristRegions;
            if (search == null || search == "Search" || search == "")
            {
                touristRegions = db.TouristRegions.ToList();
            } else
            {
                touristRegions = db.TouristRegions.Where(t => t.Address.Contains(search)
                || t.Name.Contains(search) || t.PhoneNumber.Contains(search)
                || t.Type.Contains(search) || t.GeoType.Contains(search)
                || t.Details.Contains(search)).ToList();
            }
            string currentUserId = User.Identity.GetUserId();
            TourPlan tourPlan = db.TourPlans.Find(new object[] { tourPlanId, currentUserId });
            List<string> check = new List<string>(); ;int i = 0;
            foreach (var item in touristRegions) {
                check.Add(tourPlan.Organizes.Where(o => o.RegionId == item.Id).FirstOrDefault() != null ? "checked" : "");
            }
            ViewBag.check = check.ToArray();
            ViewBag.tourPlanId = tourPlanId;
            return PartialView(touristRegions);
        }
        // GET: Home
        [Authorize(Roles = "Tourists")]
        [HttpGet]
        public PartialViewResult _ClientEvaluations(long? regionId)
        {
            ViewBag.regionId = regionId;
            List<Evaluation> evaluations = db.Evaluations.Where(e => e.RegionId == regionId).ToList();
            return PartialView(evaluations);
        }
        class tempEvaluate
        {
            public string Text { get; set; }
            public string Date { get; set; }
            public string TouristName { get; set; }
            public string Country { get; set; }
            public bool newEvaluate { get; set; }
        }
        [Authorize(Roles = "Tourists")]
        [HttpPost]
        public JsonResult AddEvaluate(string text,long? regionId)
        {
            if (text != null && regionId != null)
            {
                var currentUser = User.Identity.GetUserId();
                Evaluation evaluate = db.Evaluations.Find(new object[] { currentUser, regionId });
                if (evaluate == null)
                {
                    evaluate = new Evaluation { TouristId = currentUser, RegionId = (long)regionId, Text = text };
                    db.Evaluations.Add(evaluate);
                    db.SaveChanges();
                    evaluate = db.Evaluations.Find(new object[] { currentUser, regionId });

                    return Json(true);
                }
                evaluate.Text = text;
                evaluate.OccuredAt = DateTime.Now;
                db.Entry(evaluate).State = EntityState.Modified;
                db.SaveChanges();
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [Authorize(Roles = "Tourists")]
        [HttpGet]
        public ActionResult OrganizePlan(long? tourPlanId)
        {
            string currentUserId = User.Identity.GetUserId();
            TourPlan tourPlan = db.TourPlans.Find(new object[] { tourPlanId, currentUserId });
            if (tourPlan == null)
            {
                return RedirectToAction("Plans","Home");
            }
            return View(tourPlan);
        }

        

        //GET: Index
        [HttpGet]
        public ActionResult Index(string searchText)
        {
            if (searchText == null || searchText == "" || searchText == "Search")
            {
                List<TouristRegion> touristRegions = db.TouristRegions.ToList();
                return View(touristRegions);
            }else
            {
                var result = db.TouristRegions.Where(t => t.Address.Contains(searchText) 
                || t.Name.Contains(searchText) || t.PhoneNumber.Contains(searchText)
                || t.Type.Contains(searchText) || t.GeoType.Contains(searchText));
                return View(result);
            }
        }
        //GET: Plans
        [Authorize(Roles = "Tourists")]
        [HttpGet]
        public ActionResult Plans(string searchText)
        {
            string currentUserId = User.Identity.GetUserId();
            List<TourPlan> tourPlans = new List<TourPlan>();
            if (searchText == null || searchText == "" || searchText == "Search")
            {
                tourPlans = db.TourPlans.Where(t => t.TouristId == currentUserId).ToList();
            }
            else
            {
                tourPlans = db.TourPlans.Where(t => t.TouristId == currentUserId && t.Name.Contains(searchText)
                || t.Country.Contains(searchText)).ToList();
            }
            return View(tourPlans);
        }

        //GET: Contact
        
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        
        public ActionResult Contact(string name, string email, string subject, string message)
        {
            var mail = new MailMessage();
            var loginfo = new NetworkCredential("m.refaee123580@gmail.com", "123580123");
            mail.From = new MailAddress(email);
            mail.To.Add("m.refaee123580@gmail.com");
            mail.Subject = subject;
            mail.Body = message;
            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = loginfo;
            smtp.Send(mail);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Tourists")]
        [HttpGet]
        public ActionResult CreateTourPlan()
        {            
            return View();
        }

        [Authorize(Roles = "Tourists")]
        [HttpPost]
        public ActionResult CreateTourPlan(TourPlan tourPlan ,HttpPostedFileBase planImage)
        {
            tourPlan.TouristId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                string fileName, fileExtension, path;
                fileName = "~/PlanImages/PlanImage.jpg";
                tourPlan.PlanImage = fileName;
                db.TourPlans.Add(tourPlan);
                db.SaveChanges();
                if (planImage != null)
                {
                    fileExtension = Path.GetExtension(planImage.FileName);
                    fileName = tourPlan.Id.ToString() + fileExtension;
                    path = Path.Combine(Server.MapPath("~/PlanImages"), fileName);
                    planImage.SaveAs(path);
                    tourPlan.PlanImage = "~/PlanImages/" + fileName;
                    db.Entry(tourPlan).State = EntityState.Modified;
                    db.SaveChanges();
                }                               
                return RedirectToAction("Plans","Home");
            }
            return View(tourPlan);
        }

        [Authorize(Roles = "Tourists")]
        public ActionResult EditTourPlan(long? tourPlanId)
        {
            string currentUserId = User.Identity.GetUserId();
            if(tourPlanId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourPlan tourPlan = db.TourPlans.Find(new object[] { tourPlanId , currentUserId });
            if(tourPlan == null)
            {
                return HttpNotFound();
            }
            return View(tourPlan);
        }
        [Authorize(Roles = "Tourists")]
        [HttpPost]
        public ActionResult EditTourPlan(TourPlan tourPlan ,HttpPostedFileBase planImage)
        {
            tourPlan.TouristId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                string newFileName, newFileExtension, path;
                if(planImage != null)
                {
                    string oldPlanImage = Server.MapPath(tourPlan.PlanImage);
                    if (!tourPlan.PlanImage.Equals("~/PlanImages/PlanImage.jpg") && System.IO.File.Exists(oldPlanImage))
                    {
                        System.IO.File.Delete(oldPlanImage);
                    }
                    newFileExtension = Path.GetExtension(planImage.FileName);
                    newFileName = tourPlan.Id.ToString() + newFileExtension;
                    path = Path.Combine(Server.MapPath("~/PlanImages"), newFileName);
                    planImage.SaveAs(path);
                    tourPlan.PlanImage = "~/PlanImages/" + newFileName;
                }
                db.Entry(tourPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Plans","Home");
            }
            return View(tourPlan);
        }

        [Authorize(Roles ="Tourists")]
        public ActionResult DeleteTourPlan(long? tourPlanId)
        {
            string curretUserId = User.Identity.GetUserId();
            if (tourPlanId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourPlan tourPlan = db.TourPlans.Find(new object[] { tourPlanId, curretUserId });
            if (tourPlan == null)
            {
                return HttpNotFound();
            }
            return View(tourPlan);
        }

        [Authorize(Roles ="Tourists")]
        [HttpPost]
        public ActionResult DeleteTourPlanConfirmed(long tourPlanId)
        {
            string currentUserId = User.Identity.GetUserId(); 
            TourPlan tourPlan = db.TourPlans.Find(new object[] { tourPlanId, currentUserId });
            string oldPlanImage = Server.MapPath(tourPlan.PlanImage);
            db.TourPlans.Remove(tourPlan);
            db.SaveChanges();
            if (!tourPlan.PlanImage.Equals("~/PlanImages/PlanImage.jpg") && System.IO.File.Exists(oldPlanImage))
            {
                System.IO.File.Delete(oldPlanImage);
            }
            return RedirectToAction("Plans","Home");
        }
    }
}
