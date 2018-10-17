using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VTG.Models;

namespace VTG.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        dbVTGEntities g = new dbVTGEntities();

        // GET: Admin
        public ActionResult Index()
        {
            var db_size = g.Evaluations.ToString();

            ViewBag.db_size = db_size;

            int num_regions = g.TouristRegions.Count();

            ViewBag.num_regions = num_regions;

            int num_plans = g.TourPlans.Count();

            ViewBag.num_plans = num_plans;

            int num_tourist = g.AspNetUsers.Count();

            ViewBag.num_tourist = num_tourist;


            //    var visitors = 0;

            //    if (System.IO.File.Exists("visitors.txt"))
            //    {
            //        string noOfVisitors = System.IO.File.ReadAllText("visitors.txt");
            //        visitors = Int32.Parse(noOfVisitors);
            //    }

            //    ++visitors;

            //    var visit_text = (visitors == 1) ? " view" : " views";
            //    System.IO.File.WriteAllText("visitors.txt", visitors.ToString());

            //    ViewData["visitors"] = visitors;
            //    ViewData["visitors_txt"] = visit_text;

            return View();
        }




        public ActionResult charts()
        {




            var users = db.Users;
            var first = users.Where(a => a.Birthday.Value.Year >= 2000);
            int child = first.Count();
            var second = users.Where(a => a.Birthday.Value.Year > 2000 && a.Birthday.Value.Year < 1980);
            int man = second.Count();
            var third = users.Where(a => a.Birthday.Value.Year <= 1980);
            int older = third.Count();
            int[] old = { child, man, older};
            ViewBag.old = old;

            //int i = 0;
            //foreach(var item in g.TouristRegions)
            //{
            //    int c = g.Organizes.Count(o => o.RegionId == item.Id);

            //}
            //g.Organizes.GroupBy(o => o.RegionId).Count(i => i);
            object[] o = new object[5];
            List<long> bestreagions =  g.Database.SqlQuery<long> ("select TOP (5) e.RegionId from   ( SELECT RegionId,COUNT(RegionId) AS a FROM  Organize GROUP BY RegionId   )as e ORDER BY a DESC",o).ToList();
            List<string>names = new List<string>();
            
            foreach (long i in bestreagions) {
                names.Add( g.TouristRegions.Where(a => a.Id== i).Select(a=>a.Name).Single());

            }

            List<int> countreagions = g.Database.SqlQuery<int>("select TOP (5) e.a from   ( SELECT RegionId,COUNT(RegionId) AS a FROM  Organize GROUP BY RegionId   )as e ORDER BY a DESC", o).ToList();

            
            ViewBag.regnames = names;
            ViewBag.regcount = countreagions;

            

            return View();


       
        }

        public ActionResult tables()
        {
            return View();
        }
    }
}