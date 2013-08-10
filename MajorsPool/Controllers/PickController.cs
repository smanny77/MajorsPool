using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;
//using SharedServiceInterface;
using MajorsPool.Models;
//using MajorsPoolEmailService;
using System.ServiceModel;

namespace MajorsPool.Controllers
{
    public class PickController : Controller
    {
        private MajorsPoolDb db = new MajorsPoolDb();
        private const int HOURS_TO_PICK = 12;        
        private const string EMPTY = "";
       
        //
        // GET: /Pick/
        
        public ActionResult Index(int Admin = 0, string Password = EMPTY)
        {
            string xxx = string.Empty;

            try
            {
                xxx += "1";
                Session["Admin"] = (Convert.ToBoolean(Session["Admin"]) == true || Admin == 318) ? true : false;
                xxx += "1";
                ViewBag.UserVerified = VerifyUser(Password);
                xxx += "1";
                UpdateExpiredUsers();
                xxx += "1";
                UpdatePickEligibles();
                xxx += "1";
                // Test reference to http://localhost:58249/PickNotificationService.svc
                //ServiceReference1.PickNotificationServiceClient clientRef = new ServiceReference1.PickNotificationServiceClient();
                //ViewBag.Blah = clientRef.SendEmail(5, "X");
                //MajorsPoolServiceReference.PickNotificationServiceClient serviceRef = new MajorsPoolServiceReference.PickNotificationServiceClient();
                //string response = serviceRef.SendEmail(3, "5");
                //ViewBag.Blah = response;

                //MajorsPoolCloudServiceReference.PickNotificationServiceClient serviceRef = new MajorsPoolCloudServiceReference.PickNotificationServiceClient();
                //string response = serviceRef.SendEmail(4, "5");
                //ViewBag.Blah = response;

                return View(from p in db.Picks
                            orderby p.SeqNo
                            select p);
            }
            catch (Exception ex)
            {
                ViewBag.Blah = ex.Message + xxx + db.Database.Connection.ConnectionString + "end"; //ex.Message;
                return View();
            }
        }

        private bool VerifyUser(string password)
        {
            return true;
        }

        private string UpdateExpiredUsers()
        {
            Pick lastPersonToPick;
            Pick checkForExpirationPick;

            string returnString;

            try
            {

                do
                {
                    lastPersonToPick = (from p in db.Picks.AsEnumerable() where p.PickTime != null orderby p.SeqNo select p).LastOrDefault();

                    checkForExpirationPick = null;

                    if (lastPersonToPick != null)
                    {
                        if (lastPersonToPick.PickTime != null) // Shouldn't this always be true?
                        {
                            // If it's been more than 12 hours since the most recent pick.
                            if (new TimeSpan(DateTime.Now.Ticks - Convert.ToDateTime(lastPersonToPick.PickTime).Ticks).TotalHours > HOURS_TO_PICK)
                            {
                                // Get the pick that is next up.
                                checkForExpirationPick = (from p in db.Picks.AsEnumerable() where p.PickTime == null orderby p.SeqNo select p).FirstOrDefault();

                                if (checkForExpirationPick != null)
                                {
                                    // Assign a pick time, even though there is no golfer associated with the pick.
                                    checkForExpirationPick.PickTime = Convert.ToDateTime(lastPersonToPick.PickTime).AddHours(HOURS_TO_PICK);

                                    db.Entry(checkForExpirationPick).State = EntityState.Modified;

                                    db.SaveChanges();

                                    SendNextEmailNotification();
                                }
                            }
                        }
                    }
                } while (checkForExpirationPick != null);
            }
            catch (Exception ex)
            {
                returnString = ex.Message;
            }

            return "good";
        }

        private void UpdatePickEligibles()
        {
            int nextUpUsersPickId;

            try
            {
                nextUpUsersPickId = (from x in db.Picks.AsEnumerable() where x.PickTime == null orderby x.SeqNo select x.PickId).FirstOrDefault();
                
                foreach (var pick in db.Picks.AsEnumerable())
                {
                    // Entrant hasn't picked, and is next in line...OR
                    // Entrant hasn't picked, but he has been assigned a picktime (implying that his time has expired)
                    pick.PickEligible = ((pick.GolferId == null && pick.PickId == nextUpUsersPickId) ||
                                         (pick.GolferId == null && pick.PickTime != null));

                    db.Entry(pick).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
            catch (Exception)
            {
                //nextUpUsersPickId = -1;
            }
        }

        //
        // GET: /Pick/Details/5
        
        public ActionResult Details(int id = 0)
        {
            Pick pick = db.Picks.Find(id);
            
            if (pick == null)
            {
                return HttpNotFound();
            }

            GolferController hc = new GolferController();

            pick.Golfer = hc.GetGolfer(pick.Golfer.GolferId);

            return View(pick);
        }

        //
        // GET: /Pick/Create
        
        public ActionResult Create()
        {
            EntrantController ec = new EntrantController();
            GolferController gc = new GolferController();
            
            ViewBag.EntrantList = ec.GetEntrants();
            ViewBag.GolferList = gc.GetGolfers();

            return View();
        }

        //
        // POST: /Pick/Create

        [HttpPost]
        public ActionResult Create(Pick pick)
        {
            if (ModelState.IsValid)
            {
                pick.Entrant = db.Entrants.Find(pick.Entrant.EntrantId);

                db.Picks.Add(pick);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private void ConstructAndSendEmail(string emailTo, /*int pickId,*/ int entrantId)
        {
            var from = new MailAddress("noreply@majorspool.com");
            var to = new MailAddress[] { new MailAddress(emailTo) };
            var cc = new MailAddress[] { };
            var bcc = new MailAddress[] { new MailAddress("scott.m.manny@gmail.com") };
            var subject = "Majors Pool - Your Turn";

            EntrantController ec = new EntrantController();            
            
            var securityCode = ec.GetEntrant(entrantId).SecurityCode;

            var expirationTime = DateTime.Now.AddHours(HOURS_TO_PICK);
            
            expirationTime = DateTime.SpecifyKind(expirationTime, DateTimeKind.Utc);

            // Convert UTC time to central.
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(expirationTime, cstZone);

            var html = 
                    "<body style=\"font-family: Calibri\">" +
                    "    <div style=\"font-size: 16pt; color: darkgreen\">" +
                    "        Majors Pool" + 
                    "    </div>" + 
                    "    <div style=\"color: #333333; font-size: 12pt\">" +
                    "        <span>" +
                    "            Hi " + ec.GetEntrant(entrantId).FirstName + "," +
                    "            <br/>" +
                    "            <br/>" +
                    "            Your security code is <b>" + securityCode + "</b>.  You will need this to make your pick." +
                    "        </span>" +
                    "        <br />" +
                    "        <br />" + 
                    "        <span>" +
                    "            You have until <b>" + cstTime.ToShortDateString() + " " + cstTime.ToShortTimeString() + "</b> to make your pick from the following link:  <a href=\"http://majorspool.azurewebsites.net/Pick\">Majors Pool Draft</a>" +
                    "        </span>" + 
                    "        <br />" + 
                    "        <br />" + 
                    "        <div style=\"font-size: 12pt; color: darkgreen\">" + 
                    "            <b>Consequences</b>" + 
                    "            <br />" + 
                    "            <span style=\"color: #333333\">" + 
                    "                If your time expires, the draft selection will become unblocked allowing " + 
                    "                the next person in line to make their selection.  You will still be able to make your pick at anytime, but keep in mind that the longer you wait, the fewer golfers will be available." +
                    "            </span>" + 
                    "        </div>" + 
                    "    </div>" + 
                    "</body>";
            
            var text = "You have until " + cstTime.ToShortDateString() + " " + cstTime.ToShortTimeString() + " to make your pick from the following link:  http://majorspool.azurewebsites.net/Pick\">Majors Pool Draft</a>";

            SendGrid pickNotification = SendGrid.GetInstance(from, to, cc, bcc, subject, html, text);

            var username = "azure_dcc2b608e977cca58a411ed82cbe6da3@azure.com";
            var pswd = "ppq4k4jq";

            var credentials = new NetworkCredential(username, pswd);
            
            // Get REST instance for sending email.
            var transportREST = SendGridMail.Transport.SMTP.GetInstance(credentials); //, "smtp.sendgrid.net");

            // Send the email.
            transportREST.Deliver(pickNotification);
        }

        //
        // GET: /Pick/Edit/5

        public ActionResult Edit(int id = 0, int admin = 0, string password = EMPTY)
        {
            Pick pick = db.Picks.Find(id);
        
            if (pick == null)
            {
                return HttpNotFound();
            }
            else 
            {
                EntrantController ec = new EntrantController();
                GolferController gc = new GolferController();
                
                if (ec.GetEntrantId(password).Equals(pick.EntrantId))
                {
                    ViewBag.EntrantList = ec.GetEntrants();
                    ViewBag.GolferList = gc.GetGolfers();
                }
                else
                {
                    Redirect("http://google.com");
                }
            }

            return View(pick);
        }

        //
        // POST: /Pick/Edit/5

        [HttpPost]
        public ActionResult Edit(Pick pick)
        {
            if (ModelState.IsValid)
            {
                if (pick.Golfer != null && pick.Golfer.GolferId != -1)
                {
                    pick.Golfer = db.Golfers.Find(pick.Golfer.GolferId);
                    pick.GolferId = pick.Golfer.GolferId;

                    pick.Entrant = db.Entrants.Find(pick.Entrant.EntrantId);
                    pick.EntrantId = pick.Entrant.EntrantId;

                    if (pick.PickTime == null)
                    {
                        pick.PickTime = DateTime.Now;
                    }

                    pick.PickEligible = false; // We've made the pick, now we are not allowed to change.

                    db.Entry(pick).State = EntityState.Modified;

                    db.SaveChanges();

                    SendNextEmailNotification();

                    return RedirectToAction("Index");
                }
            }            

            return RedirectToAction("Index");
        }

        private void SendNextEmailNotification()
        {
            Pick nextPicker = GetNextPick();

            EntrantController ec = new EntrantController();

            if (nextPicker != null)
            {
                string emailTo = nextPicker.Entrant.Email;

                if (!String.IsNullOrEmpty(emailTo))
                {
#if DEBUG
                               // Don't send an email
#else
                    ConstructAndSendEmail(emailTo, /*nextPicker.PickId,*/ Convert.ToInt32(nextPicker.EntrantId));
#endif
                }
            }
        }

        private Pick GetNextPick()
        {
            return (from p in db.Picks
                    where p.PickTime == null
                    orderby p.SeqNo
                    select p).FirstOrDefault();
        }

        //
        // GET: /Pick/Delete/5

        public ActionResult Delete(int id = 0, int admin = 0)
        {
            Pick pick = db.Picks.Find(id);
            if (pick == null)
            {
                return HttpNotFound();
            }

            return View(pick);
        }

        //
        // POST: /Pick/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, int admin = 0)
        {
            Pick pick = db.Picks.Find(id);
            db.Picks.Remove(pick);
            db.SaveChanges();
            
            return RedirectToAction("Index", new { admin = 318 });
        }



        



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

//AutomaticMigrationsEnabled = true;
  //          AutomaticMigrationDataLossAllowed = true;