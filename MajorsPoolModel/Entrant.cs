using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MajorsPool.Models
{
    public class Entrant
    {
        public Entrant()
        {
            Email = String.Empty;
        }

        [Display(Name = "ID")]
        public int EntrantId { get; set; }
        
        [Display(Name = "First")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last")]
        public string LastName { get; set; }        
        
        public string DisplayName {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Code")]
        public string SecurityCode { get; set; }

        [Display(Name = "Paid")]
        public bool PaidStatus { get; set; }
    }
}