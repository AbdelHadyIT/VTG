using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VTG.Models;

namespace VTG.Controllers
{
    [Authorize(Roles ="Admins")]
    public class TouristRegionsController : Controller
    {
        private dbVTGEntities db = new dbVTGEntities();

        // GET: TouristRegions
        public ActionResult Index()
        {
            return View(db.TouristRegions.ToList());
        }

        // GET: TouristRegions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TouristRegion touristRegion = db.TouristRegions.Find(id);
            if (touristRegion == null)
            {
                return HttpNotFound();
            }
            return View(touristRegion);
        }

        // GET: TouristRegions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TouristRegions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TouristRegion touristRegion , HttpPostedFileBase regionImage)
        {
            if (ModelState.IsValid)
            {
                
                string fileName, fileExtension, path;
                fileName = "~/RegionImages/syria.gif";
                touristRegion.RegionImage = fileName;
                db.TouristRegions.Add(touristRegion);
                db.SaveChanges();
                if (regionImage != null)
                {
                    fileExtension = Path.GetExtension(regionImage.FileName);
                    fileName = touristRegion.Id.ToString() + fileExtension;
                    path = Path.Combine(Server.MapPath("~/RegionImages"), fileName);
                    regionImage.SaveAs(path);
                    touristRegion.RegionImage = "~/RegionImages/" + fileName;
                    db.Entry(touristRegion).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(touristRegion);
        }

        // GET: TouristRegions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TouristRegion touristRegion = db.TouristRegions.Find(id);
            if (touristRegion == null)
            {
                return HttpNotFound();
            }
            return View(touristRegion);
        }

        // POST: TouristRegions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TouristRegion touristRegion , HttpPostedFileBase regionImage)
        {
            if (ModelState.IsValid)
            {
                string newFileName, newFileExtension, path;
                if(regionImage != null)
                {
                    string oldImage = Server.MapPath(touristRegion.RegionImage);
                    if (System.IO.File.Exists(oldImage) && !touristRegion.RegionImage.Equals("~/RegionImages/syria.gif"))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                    newFileExtension = Path.GetExtension(regionImage.FileName);
                    newFileName = touristRegion.Id.ToString() + newFileExtension;
                    path = Path.Combine(Server.MapPath("~/RegionImages"), newFileName);
                    regionImage.SaveAs(path);
                    touristRegion.RegionImage = "~/RegionImages/" + newFileName;
                }
                db.Entry(touristRegion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(touristRegion);
        }

        // GET: TouristRegions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TouristRegion touristRegion = db.TouristRegions.Find(id);
            if (touristRegion == null)
            {
                return HttpNotFound();
            }
            return View(touristRegion);
        }

        // POST: TouristRegions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {

            TouristRegion touristRegion = db.TouristRegions.Find(id);
            string oldImage = Server.MapPath(touristRegion.RegionImage);
            if(!touristRegion.RegionImage.Equals("~/RegionImages/syria.gif") && System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            db.TouristRegions.Remove(touristRegion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
