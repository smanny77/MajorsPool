﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MajorsPool.Models;

namespace MajorsPool.Models
{
    public class MajorsPoolDb : DbContext
    {
        public DbSet<Golfer> Golfers { get; set; }
        public DbSet<Entrant> Entrants { get; set; }
        public DbSet<Pick> Picks { get; set; }
        public DbSet<TopPickList> TopPickList { get; set; }
    }
}