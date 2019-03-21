using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VideoRental.Models;

namespace VideoRental.ViewModels
{
    public class CustomerRentalDetailsViewModel
    {
        public Rental Rental { get; set; }
        public string CustomerName { get; set; }
        public List<CustomerMoviesViewModel> RentedMovies { get; set; }
    }
}