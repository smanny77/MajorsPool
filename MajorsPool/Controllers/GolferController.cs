using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MajorsPool.Models;
//using MajorsPool.DataLayer;

namespace MajorsPool.Controllers
{
    public class GolferController : Controller
    {
        private MajorsPoolDb db = new MajorsPoolDb();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(db.Golfers.ToList().OrderBy(golfer => golfer.LastName));
            //return View(GetGolfers());
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(int id = 0)
        {
            Golfer golfer = db.Golfers.Find(id);
            if (golfer == null)
            {
                return HttpNotFound();
            }
            return View(golfer);
        }

        //
        // GET: /Home/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(Golfer golfer)
        {
            if (ModelState.IsValid)
            {
                db.Golfers.Add(golfer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(golfer);
        }

        //
        // GET: /Home/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Golfer golfer = db.Golfers.Find(id);
            if (golfer == null)
            {
                return HttpNotFound();
            }
            return View(golfer);
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        public ActionResult Edit(Golfer golfer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(golfer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(golfer);
        }

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Golfer golfer = db.Golfers.Find(id);
            if (golfer == null)
            {
                return HttpNotFound();
            }
            return View(golfer);
        }

        //
        // POST: /Home/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Golfer golfer = db.Golfers.Find(id);
            db.Golfers.Remove(golfer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public Golfer GetGolfer(int id)
        {
            Golfer golfer = db.Golfers.Find(id);
            return golfer;
        }

        public List<Golfer> GetGolfers()
        {
            return (from g in db.Golfers
                    where !(from p in db.Picks select p.GolferId).Contains(g.GolferId)
                    orderby g.FirstName
                    select g).ToList();
        }

        public List<Golfer> GetGolfersNotInTopPickList(/*entrantId*/)
        {
            return (from g in db.Golfers
                    where !(from p in db.Picks select p.GolferId).Contains(g.GolferId) 
                    && !(from tp in db.TopPickList select tp.GolferId).Contains(g.GolferId)                    
                    orderby g.FirstName
                    select g).ToList();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                List<Golfer> golfersToDelete = (from g in db.Golfers
                        where !(from p in db.Picks select p.GolferId).Contains(g.GolferId)
                        orderby g.FirstName
                        select g).ToList();

                foreach (Golfer g in golfersToDelete)
                {
                    db.Golfers.Remove(g);
                    db.SaveChanges();
                }

                System.IO.StreamReader sr = new System.IO.StreamReader(file.InputStream);
                
                string golfer;

                while (sr.Peek() > 0)
                {
                    golfer = sr.ReadLine();
                    
                    Golfer g1 = new Golfer();
                    
                    g1.FirstName = golfer.Split(" "[0])[0];
                    g1.LastName = golfer.Split(" "[0])[1];

                    List<Golfer> gList = (from g in db.Golfers
                                          where g.FirstName.Trim().ToLower() == g1.FirstName.Trim().ToLower() 
                                             && g.LastName.Trim().ToLower() == g1.LastName.Trim().ToLower()
                            select g).ToList();

                    if (gList.Count == 0)
                    {
                        db.Golfers.Add(g1);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}