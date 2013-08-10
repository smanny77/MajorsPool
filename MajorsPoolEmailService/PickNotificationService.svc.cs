using System;
using System.Collections.Generic;
//using System.Data.Entity;
//using System.Data.Entity.DbContext;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SharedServiceInterface;
using MajorsPool.Models;
using MajorsPool.DataLayer;

namespace MajorsPoolEmailService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PickNotificationService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PickNotificationService.svc or PickNotificationService.svc.cs at the Solution Explorer and start debugging.
    public class PickNotificationService : IPickNotificationService
    {
        private MajorsPoolDb db = new MajorsPoolDb();

        public string SendEmail(int value, string message)
        {
            //return string.Format("You entered: {0} - {1}", value, message);
            Pick pick = db.Picks.Find(3);

            if (pick == null)
            {
                return "not found";
            }

            //GolferController hc = new GolferController();

            //pick.Golfer = hc.GetGolfer(pick.Golfer.GolferId);
            return pick.Golfer.FirstName;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }

            return composite;
        }
    }
}
