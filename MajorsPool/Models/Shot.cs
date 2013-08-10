using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MajorsPool.Models
{
    public class Shot
    {
        public int ShotId { get; set; }

        //[Display(Name = "Seq #")]
        //public int SeqNo { get; set; }

        //public int? EntrantId { get; set; }

        //[ForeignKey("EntrantId")]
        //public virtual Entrant Entrant { get; set; }

        public int Distance { get; set; }

        public int WindSpeed { get; set; }

        public int WindDirection { get; set; }

        public int Temperature { get; set; }

        public int Elevation { get; set; }

        public int Power { get; set; }
    }
}