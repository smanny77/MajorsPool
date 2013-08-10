using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MajorsPool.Models
{
    public class Pick
    {
        public int PickId { get; set; }

        [Display(Name = "Seq #")]
        public int SeqNo { get; set; }
        
        public int? EntrantId { get; set; }

        public int? GolferId { get; set; }
        
        [ForeignKey("EntrantId")]
        public virtual Entrant Entrant { get; set; }

        [ForeignKey("GolferId")]
        public virtual Golfer Golfer { get; set; }

        public bool PickEligible { get; set; }

        // PickTime is saved in UTC and read back in UTC
        private DateTime? pickTime;
        public DateTime? PickTime 
        { 
            //get
            //{
            //    if (pickTime != null)
            //    {
            //        // Convert UTC time to central.
            //        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            //        DateTime pickTimeWithKind = DateTime.SpecifyKind(Convert.ToDateTime(pickTime), DateTimeKind.Utc);

            //        DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(pickTime), cstZone);
                
            //        return cstTime;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //set
            //{
            //    if (value != null)
            //    {
            //        TimeZoneInfo zone = TimeZoneInfo.Utc;
            //        DateTime now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            //        pickTime = TimeZoneInfo.ConvertTimeToUtc(now, zone);
            //    }
            //    else
            //    {
            //        pickTime = null;
            //    }
                
            //    //pickTime = value;
            //}
            get
            {
                return pickTime;
            }
            set
            {
                pickTime = value;
            }
            //set
            //{
            //    if (value != null)
            //    {
            //        TimeZoneInfo zone = TimeZoneInfo.Utc;
            //        DateTime now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            //        pickTime = TimeZoneInfo.ConvertTimeToUtc(now, zone);
            //    }
            //    else
            //    {
            //        pickTime = null;
            //    }
            //}
        }
    }
}