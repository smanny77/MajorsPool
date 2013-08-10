using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MajorsPool.Models
{
    public class TopPickList // Top Pick List
    {
        [Display(Name = "ID")]
        public int TopPickListId { get; set; }

        public TopPickList()
        { 
        }

        public int? EntrantId { get; set; }

        public int? GolferId { get; set; }

        public int SeqNo { get; set; }

        [ForeignKey("EntrantId")]
        public virtual Entrant Entrant { get; set; }

        [ForeignKey("GolferId")]
        public virtual Golfer Golfer { get; set; }
    }
}