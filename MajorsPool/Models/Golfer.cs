using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MajorsPool.Models
{
    public class Golfer
    {
        public Golfer()
        {
            GolferId = -1;
        }

        public int GolferId { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        [Display(Name = "Name")]
        public string DisplayName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}