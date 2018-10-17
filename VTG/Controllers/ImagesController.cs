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
    public class ImagesController : Controller
    {
        private dbVTGEntities db = new dbVTGEntities();

        // GET: Images
        public ActionResult Index(long? landmarkId ,long? regionId)
        {
            Landmark landmark = db.Landmarks.Find(new object[] { landmarkId , regionId}); 
            if(landmark == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }
            ViewBag.LandmarkId = landmarkId;
            ViewBag.RegionId = regionId;           
            var images = db.Images.Include(i => i.Landmark).Where(i => i.LandmarkId == landmarkId && i.RegionId == regionId);
            return View(images.ToList());
        }

        // GET: Images/Details/5
        public ActionResult Details(long? imageId , long? landmarkId, long? regionId)
        {
            if (imageId == null || landmarkId == null || regionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(new object[] { imageId , landmarkId , regionId });
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // GET: Images/Create
        public ActionResult Create(long? landmarkId , long? regionId)
        {
            Landmark landmark = db.Landmarks.Find(new object[] { landmarkId, regionId });
            if(landmark == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.LandmarkId = landmarkId;
            ViewBag.RegionId = regionId;
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="LandmarkId,RegionId")]Image image , List<HttpPostedFileBase> photos)
        {
            if (ModelState.IsValid)
            {
                string path,fileName,fileExtension;

                foreach(var photo in photos)
                {
                    if(photo != null)
                    {
                        
                        db.Images.Add(image);
                        db.SaveChanges();
                        fileName = image.Id.ToString();
                        fileExtension = Path.GetExtension(photo.FileName);
                        path = Path.Combine(Server.MapPath("~/photos"), fileName + fileExtension);
                        photo.SaveAs(path);
                        image.Photo = "~/photos/" + fileName + fileExtension;
                        db.Entry(image).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index", new { landmarkId = image.LandmarkId , regionId = image.RegionId });
            }

            ViewBag.LandmarkId = new SelectList(db.Landmarks, "Id", "Name", image.LandmarkId);
            return View(image);
        }

        // GET: Images/Edit/5
        public ActionResult Edit(long? imageId , long? landmarkId ,long? regionId)
        {
            if (imageId == null || landmarkId == null || regionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(new object[] { imageId , landmarkId , regionId });
            if (image == null)
            {
                return HttpNotFound();
            }
            //oldPhoto = Server.MapPath(image.Photo);
            ViewBag.LandmarkId = new SelectList(db.Landmarks, "Id", "Name", image.LandmarkId);
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Image image ,HttpPostedFileBase photo)
        {
            string path, newFileExtension;
            if (ModelState.IsValid)
            {
                if (photo != null)
                {
                    string oldPhoto = Server.MapPath(image.Photo);
                    if (System.IO.File.Exists(oldPhoto))
                    {
                        System.IO.File.Delete(oldPhoto);
                    }
                    newFileExtension = Path.GetExtension(photo.FileName);
                    path = Path.Combine(Server.MapPath("~/photos"), image.Id.ToString() + newFileExtension);
                    photo.SaveAs(path);
                    image.Photo = "~/photos/" + image.Id.ToString() + newFileExtension;
                    db.Entry(image).State = EntityState.Modified;
                    db.SaveChanges();                    
                }
                return RedirectToAction("Index", new { landmarkId = image.LandmarkId , regionId = image.RegionId });
            }
            return View(image);
        }

        // GET: Images/Delete/5
        public ActionResult Delete(long? imageId ,long? landmarkId ,long? regionId)
        {
            if (imageId == null || landmarkId == null || regionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(new object[] { imageId, landmarkId, regionId });
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long imageId, long landmarkId, long regionId)
        {
            Image image = db.Images.Find(new object[] { imageId, landmarkId, regionId });
            string oldPhoto = Server.MapPath(image.Photo);
            db.Images.Remove(image);
            db.SaveChanges();
            if (System.IO.File.Exists(oldPhoto))
            {
                System.IO.File.Delete(oldPhoto);
            }
            return RedirectToAction("Index", new { landmarkId = image.LandmarkId, regionId = image.RegionId });
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
