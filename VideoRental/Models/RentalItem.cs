using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VideoRental.Models
{
    public class RentalItem
    {
        public int RentalItemId { get; set; }
        public int RentalId { get; set; }
        public int MovieId { get; set; }
        public IEnumerable<SelectListItem> Movies { get; set; }
    }
}