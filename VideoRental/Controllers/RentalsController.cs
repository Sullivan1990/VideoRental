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
            rental.RentedMovies = rentedMovies;
            //var customer = GetCustomers();
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
                rental.CustomerId = int.Parse(collection["CustomerId"]);
                rental.DateRented = DateTime.Now;
                db.Rentals.Add(rental);
                db.SaveChanges();

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
            {
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