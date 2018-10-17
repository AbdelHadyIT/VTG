using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VTG.Models;

namespace VTG.Controllers
{
    public class LandmarksController : Controller
    {
        private dbVTGEntities db = new dbVTGEntities();

        // GET: Landmarks
        public ActionResult Index(long? regionId)
        {
            TouristRegion touristRegion = db.TouristRegions.Find(regionId);
            if(touristRegion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var landmarks = db.Landmarks.Include(l => l.TouristRegion).Where(l => l.RegionId == regionId);
            ViewBag.RegionId = regionId;
            return View(landmarks.ToList());
        }

        // GET: Landmarks/Details/5
        public ActionResult Details(long? id , long? regionId)
        {
            if (id == null || regionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Landmark landmark = db.Landmarks.Find(new object[] { id , regionId});
            if (landmark == null)
            {
                return HttpNotFound();
            }
            return View(landmark);
        }

        // GET: Landmarks/Create
        public ActionResult Create(long? regionId)
        {
            TouristRegion touristRegion = db.TouristRegions.Find(regionId);
            if (touristRegion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RegionId = regionId;
            return View();
        }

        // POST: Landmarks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Landmark landmark)
        {
            if (ModelState.IsValid)
            {
                db.Landmarks.Add(landmark);
                db.SaveChanges();
                return RedirectToAction("Index",new { regionId = landmark.RegionId});
            }
            //ViewBag.RegionId = new SelectList(db.TouristRegions, "Id", "Name", landmark.RegionId);
            return View(landmark);
        }

        // GET: Landmarks/Edit/5
        public ActionResult Edit(long? id ,long? regionId)
        {
            if (id == null || regionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Landmark landmark = db.Landmarks.Find(new object[] { id , regionId});
            if (landmark == null)
            {
                return HttpNotFound();
            }
            return View(landmark);
        }

        // POST: Landmarks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Landmark landmark)
        {
            if (ModelState.IsValid)
            {
                db.Entry(landmark).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new { regionId = landmark.RegionId });
            }
            return View(landmark);
        }

        // GET: Landmarks/Delete/5
        public ActionResult Delete(long? id , long? regionId)
        {
            if (id == null || regionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Landmark landmark = db.Landmarks.Find(new object[] { id , regionId });
            if (landmark == null)
            {
                return HttpNotFound();
            }
            return View(landmark);
        }

        // POST: Landmarks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id , long regionId)
        {
            Landmark landmark = db.Landmarks.Find(new object[] { id , regionId });
            db.Landmarks.Remove(landmark);
            db.SaveChanges();
            return RedirectToAction("Index" , new { regionId = landmark.RegionId});
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
