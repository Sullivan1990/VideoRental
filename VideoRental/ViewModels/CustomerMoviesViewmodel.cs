﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoRental.ViewModels
{
    public class CustomerMoviesViewModel
    {
        public int RentalItemId { get; set; }
        public int RentalId { get; set; }
        public string MovieName { get; set; }
    }
}