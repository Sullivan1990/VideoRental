using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using VideoRental.Models;

namespace VideoRental.DAL
{
    public class VideoInitializer : DropCreateDatabaseIfModelChanges<VideoContext>
    {
        protected override void Seed(VideoContext context)
        {
            var movies = new List<Movie>
            {
                new Movie{MovieID = 1, Name = "The Avengers"},
                new Movie{MovieID = 2, Name = "Star Wars"},
                new Movie{MovieID = 3, Name = "The Matrix"}
            };

            movies.ForEach(m => context.Movies.Add(m));
            context.SaveChanges();

            var customers = new List<Customer>
            {
                new Customer{CustomerID = 1, CustomerName = "Alice", CustomerPhone = "1234 567 890"},
                new Customer{CustomerID = 2, CustomerName = "Bob", CustomerPhone = "2345 678 901"},
                new Customer{CustomerID = 3, CustomerName = "Paul", CustomerPhone = "3456 789 012"}
            };


            customers.ForEach(c => context.Customers.Add(c));
            context.SaveChanges();

            var rentals = new List<Rental>
            {
                new Rental{RentalId = 1, CustomerId = 1, DateRented = DateTime.Parse("01/01/2017")},
                new Rental{RentalId = 2, CustomerId = 1, DateRented = DateTime.Parse("01/01/2015")},
                new Rental{RentalId = 3, CustomerId = 2, DateRented = DateTime.Parse("01/01/2018")}
            };

            rentals.ForEach(r => context.Rentals.Add(r));
            context.SaveChanges();

            var rentalItems = new List<RentalItem>
            {
                new RentalItem{RentalItemId = 1, RentalId = 1, MovieId = 1},
                new RentalItem{RentalItemId = 2, RentalId = 1, MovieId = 2}
            };

            rentalItems.ForEach(ri => context.RentalItems.Add(ri));
            context.SaveChanges();
        }
    }
}