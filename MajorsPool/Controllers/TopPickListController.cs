using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MajorsPool.Models;

namespace MajorsPool.Controllers
{
    public class TopPickListController : Controller
    {
        private MajorsPoolDb db = new MajorsPoolDb();

        //
        // GET: /TopPickList/

        public ActionResult Index()
        {
            GolferController gc = new GolferController();
            ViewBag.AvailableGolfers = gc.GetGolfersNotInTopPickList();

            //return View(db.TopPickList.ToList());
            return View(from tp in db.TopPickList                        
                        orderby tp.SeqNo
                        select tp);
        }

        //
        // GET: /TopPickList/Details/5

        public ActionResult Details(int id = 0)
        {
            TopPickList toppicklist = db.TopPickList.Find(id);
            if (toppicklist == null)
            {
                return HttpNotFound();
            }
            return View(toppicklist);
        }

        //
        // GET: /TopPickList/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TopPickList/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TopPickList toppicklist)
        {
            if (ModelState.IsValid)
            {
                db.TopPickList.Add(toppicklist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toppicklist);
        }

        //
        // GET: /TopPickList/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TopPickList toppicklist = db.TopPickList.Find(id);
            if (toppicklist == null)
            {
                return HttpNotFound();
            }
            return View(toppicklist);
        }

        //
        // POST: /TopPickList/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TopPickList toppicklist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toppicklist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toppicklist);
        }

        //
        // GET: /TopPickList/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TopPickList toppicklist = db.TopPickList.Find(id);
            if (toppicklist == null)
            {
                return HttpNotFound();
            }
            return View(toppicklist);
        }

        //
        // POST: /TopPickList/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TopPickList toppicklist = db.TopPickList.Find(id);
            db.TopPickList.Remove(toppicklist);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //[HttpPost]
        public JsonResult DeleteTopPicksList()
        {
            foreach (var entity in db.TopPickList)
            {
                db.TopPickList.Remove(entity);
            }
            
            db.SaveChanges();

            return Json(new { data = "X" });
        }

        //[HttpPost]
        public JsonResult SaveTopPicksList(int entrantId, int golferId, int seqNo)
        {
            TopPickList tpItem = new TopPickList();
            tpItem.Entrant = db.Entrants.Find(entrantId);
            tpItem.Golfer = db.Golfers.Find(golferId);
            tpItem.SeqNo = seqNo;

            db.TopPickList.Add(tpItem);
            db.SaveChanges();

            return Json(new { data = "X" });
        }
    }
}