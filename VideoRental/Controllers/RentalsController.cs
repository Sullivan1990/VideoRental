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
            rental.Customers = GetCustomers();

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

                return RedirectToAction("Index");
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

        #endregion Helper methods
    }
}