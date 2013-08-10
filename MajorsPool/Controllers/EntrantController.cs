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
    public class EntrantController : Controller
    {
        private MajorsPoolDb db = new MajorsPoolDb();
        private const string EMPTY = "";

        //
        // GET: /Entrant/

        public ActionResult Index(int Admin = 0, string Password = EMPTY)
        {
            Session["Admin"] = (Convert.ToBoolean(Session["Admin"]) == true || Admin == 318) ? true : false;

            return View(db.Entrants.ToList());
        }

        //
        // GET: /Entrant/Details/5

        public ActionResult Details(int id = 0)
        {
            Entrant entrant = db.Entrants.Find(id);
            if (entrant == null)
            {
                return HttpNotFound();
            }
            return View(entrant);
        }

        //
        // GET: /Entrant/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Entrant/Create

        [HttpPost]
        public ActionResult Create(Entrant entrant)
        {
            if (ModelState.IsValid)
            {
                GenerateSecurityKey(entrant);
                db.Entrants.Add(entrant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(entrant);
        }

        private static void GenerateSecurityKey(Entrant entrant)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            entrant.SecurityCode = random.Next(99999).ToString();
        }

        //
        // GET: /Entrant/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Entrant entrant = db.Entrants.Find(id);
            if (entrant == null)
            {
                return HttpNotFound();
            }
            return View(entrant);
        }

        //
        // POST: /Entrant/Edit/5

        [HttpPost]
        public ActionResult Edit(Entrant entrant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entrant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entrant);
        }

        //
        // GET: /Entrant/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Entrant entrant = db.Entrants.Find(id);
            if (entrant == null)
            {
                return HttpNotFound();
            }
            return View(entrant);
        }

        //
        // POST: /Entrant/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Entrant entrant = db.Entrants.Find(id);
            db.Entrants.Remove(entrant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public List<Entrant> GetEntrants()
        {
            return (from e in db.Entrants
                    where !(from p in db.Picks select p.EntrantId).Contains(e.EntrantId)
                    orderby e.FirstName
                    select e).ToList();
        }

        public Entrant GetEntrant(int id)
        {
            Entrant entrant = db.Entrants.Find(id);
            return entrant;
        }

        public int GetEntrantId(string password)
        {
            Entrant verifiedUser = (from e in db.Entrants
                    where e.SecurityCode.Equals(password)
                    select e).FirstOrDefault();

            return (verifiedUser == null) ? -1 : verifiedUser.EntrantId;
        }
    }
}