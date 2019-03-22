using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoRental.DAL;
using VideoRental.Models;
using VideoRental.ViewModels;

namespace VideoRental.Controllers
{
    public class RentalsController : Controller
    {
        private VideoContext db = new VideoContext();

        // GET: Rentals
        public ActionResult Index()
        {

            /* the initial var declared here will hold the results of the LINQ query below it
             * r is instantiated as a CustomerRentalsViewModel
             * the viewmodel contains properties for RentalID and DateRented
             * the next 2 lines pass the values from the rental model to the viewmodel
             * the Customer name however is stored in the Customer table and is referenced
             * via the CustomerID value we already have and the value is passed to the viewmodel
             * from the customers model (or table)
             * db.Customers.Where(c => c.CustomerID == r.CustomerId)
             *             .Select(u => u.CustomerName).FirstOrDefault()
             * this code passes each Customer record into the 'c' and then the CustomerID
             * of each is compared to CustomerID we have from the selected Rentals record
             * we've already found. when a match is found the .Select returns only the CustomerName
             * and only selects the first if there are 2 records (if it selected 2 records there will be an error)
             */

            /*
             * This method is used to pull selected matching values from 2 tables or models
             * to construct a new 'virtual' representation of the data in a form not already
             * presented byt the base models. 
             * This means we don't have to modify the original models and risk corupting data
             * if a new need for a collection of data arises.
             */

            /*
             * it appears as if lambda's allow you to iterate over a collection without any need for loops
             */

            var customerRentalsViewModel = db.Rentals.Select(
                r => new CustomerRentalsViewModel
                {
                    RentalId = r.RentalId,
                    DateRented = r.DateRented,
                    CustomerName = db.Customers.Where(c => c.CustomerID == r.CustomerId)
                                               .Select(u => u.CustomerName).FirstOrDefault()
                }).OrderByDescending(o => o.DateRented).ToList();

            return View(customerRentalsViewModel);
        }

        // Edit - Get
        public ActionResult Edit(int Id)
        {
            var rental = db.Rentals.Single(r => r.RentalId == Id);
            rental.Customers = GetCustomers();
            // viewmodel.property = model.property where (condition)
            var rentedMovies = db.RentalItems.Where(r => r.RentalId == Id)
                                    .Select(m => new CustomerMoviesViewModel
                                    {
                                        RentalItemId = m.RentalItemId,
                                        RentalId = m.RentalId,
                                        MovieName = db.Movies.Where(c => 
                                                                    c.MovieID == m.MovieId)
                                                                    .Select(f => f.Name)
                                                                    .FirstOrDefault()
                                    }).ToList();
            // RentedMovies is list of all movies rented in a single rental record
            rental.RentedMovies = rentedMovies;
            return View(rental);
        }

        //Edit - Post
        [HttpPost]

        public ActionResult Edit(int Id, FormCollection collection)
        {
            try
            {
                var rental = db.Rentals.Single(r => r.RentalId == Id);
                if (TryUpdateModel(rental))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(rental);
            }
            catch
            {
                return View();
            }
        }

        // Create - Get
        public ActionResult Create()
        {
            var rental = new Rental();

            /*
             * the code below uses a ternary operator to count the total number of rentals
             * if the number is equal to 0, the new rentalId will be 1, if there are already 
             * existing rentals, the highest number is found via .Max and the new rental is asigned
             * a RentalId of this number + 1
             */

            rental.RentalId = (db.Rentals.Count<Rental>() == 0) ?
                                1 : db.Rentals.Max(r => r.RentalId) + 1;
            rental.DateRented = DateTime.Now;
            rental.Customers = GetCustomers();
            //rental.Customers = GetCustomers();
            rental.RentedMovies = new List<CustomerMoviesViewModel>();

            return View(rental);
        }

        // Create - Post
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                Rental rental = new Rental();
                rental.RentalId = (db.Rentals.Count<Rental>() == 0) ?
                                    1 : db.Rentals.Max(r => r.RentalId) + 1;
                // Parses the user input to an integer via the FormCollection
                rental.CustomerId = int.Parse(collection["CustomerId"]);
                rental.DateRented = DateTime.Now;
                db.Rentals.Add(rental);
                db.SaveChanges();


                /* allows the user of the website to create multiple Rentals
                *  without having to go backwards and forwards
                */
                int count = db.RentalItems.Where(r => r.RentalId == rental.RentalId)
                                                        .Count<RentalItem>();
                if(count == 0)
                {
                    return RedirectToAction("Edit", new { Id = rental.RentalId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        // Delete - Get
        public ActionResult Delete(int Id)
        {
            Rental rental = db.Rentals.Single(r => r.RentalId == Id);
            return View(rental);
        }

        // Delete - Post
        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                var rental = db.Rentals.Single(r => r.RentalId == Id);
                db.Rentals.Remove(rental);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(int Id)
        {
            //var rental = db.Rentals.Single(r => r.RentalId == Id);
            //return View(rental);
            var customerRentalDetails = db.Rentals.Where(c1 => c1.RentalId == Id).Select(r => new CustomerRentalDetailsViewModel
            {
                Rental = r,
                CustomerName = db.Customers.Where(c1 => c1.CustomerID == r.CustomerId).Select(cu => cu.CustomerName).FirstOrDefault(),
                RentedMovies = r.RentalItems.Select(
                    ri => new CustomerMoviesViewModel
                    {
                        RentalId = ri.RentalId,
                        MovieName = db.Movies.Where(c2 => c2.MovieID == ri.MovieId).Select(m => m.Name).FirstOrDefault()
                    }).ToList()
            }).Single();
            return View(customerRentalDetails);
        }

        // AddMovies - Get
        public ActionResult AddMovies(int RentalId)
        {
            var rentalItem = new RentalItem();
            var movies = GetMovies();
            rentalItem.RentalId = RentalId;
            rentalItem.Movies = movies;

            return View(rentalItem);
        }

        // AddMovies - Post
        [HttpPost]
        public ActionResult AddMovies(FormCollection collection)
        {
            int Id = 0;

            try
            {
                RentalItem rentalItem = new RentalItem();
                rentalItem.MovieId = int.Parse(collection["MovieId"].ToString());
                rentalItem.RentalId = int.Parse(collection["RentalId"].ToString());
                Id = rentalItem.RentalId;
                db.RentalItems.Add(rentalItem);
                db.SaveChanges();

                return RedirectToAction("Edit", new { Id });
            }
            catch(Exception e)
            {   // think this needs to be 'return Action' instead of 'return View'
                return View("No record associated to rental can be found." +
                    "\nMake sure to submit the rental details before" +
                    "adding movies");
            }
        }

        // EditRentedMovie - get
        public ActionResult EditRentedMovie(int Id)
        {
            var rentalItem = db.RentalItems.Single(r => r.RentalItemId == Id);
            rentalItem.Movies = GetMovies();

            return View(rentalItem);
        }

        // EditRentedMovie - post
        [HttpPost]
        public ActionResult EditRentedMovie(int Id, RentalItem rentalItem)
        {
            try
            {
                var rentalIem = db.RentalItems.Single(r => r.RentalItemId == Id);
                Id = rentalItem.RentalId;
                if (TryUpdateModel(rentalItem))
                {
                    db.SaveChanges();

                    return RedirectToAction("Edit", new { Id });
                }

                return View(rentalItem);
            }
            catch
            {
                return View();
            }
        }

        #region Helper methods

        public IEnumerable<SelectListItem> GetCustomers()
        {
            /* AsNoTracking() prevents the data being cached from the query
            *  - saves overhead in processing these queries
            *  
            *  the Code below returns a list ordered by the customers name
            *  and matched in sets to the relevant customerID
            *  the SelectListItem is used on webpages as a drop down list
            */
            List<SelectListItem> customers = db.Customers.AsNoTracking()
                                                .OrderBy(o => o.CustomerName)
                                                .Select(c => new SelectListItem
                                                {
                                                    Value = c.CustomerID.ToString(),
                                                    Text = c.CustomerName
                                                }).ToList();

            return new SelectList(customers, "Value", "Text");
        }

        public IEnumerable<SelectListItem> GetMovies()
        {
            List<SelectListItem> movies = db.Movies.AsNoTracking()
                                                .OrderBy(o => o.Name)
                                                .Select(m => new SelectListItem
                                                {
                                                    Value = m.MovieID.ToString(),
                                                    Text = m.Name
                                                }).ToList();

            return new SelectList(movies, "Value", "Text");
        }

        #endregion Helper methods
    }
}