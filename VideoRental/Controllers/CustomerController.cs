using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoRental.Models;

namespace VideoRental.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        //public ActionResult Index()
        //{
        //    return RedirectToAction("About", "Home");
        //}

        #region Demo code only
        [Route("{customer}/{index}/{pageIndex}/{sortBy}")]
        public ActionResult Index(int? pageIndex, string sortBy)
        {
            if (!pageIndex.HasValue) pageIndex = 1;
            if (string.IsNullOrWhiteSpace(sortBy)) sortBy = "CustomerName";

            return Content($"Page Index = {pageIndex} and Sort By = {sortBy}");
        }

        public ActionResult DisplayCustomer()
        {
            var customer = new Customer() { CustomerName = "Alice" };

            return View(customer);
        }
        #endregion Demo code only

        public static List<Customer> customerList = new List<Customer>
        {
            new Customer{CustomerID = 1, CustomerName = "Alice", CustomerPhone = "1234 567 890"},
            new Customer{CustomerID = 2, CustomerName = "Bob", CustomerPhone = "2345 678 901"},
            new Customer{CustomerID = 3, CustomerName = "Paul", CustomerPhone = "3456 789 012"}
        };

        public ActionResult Index()
        {
            var customers = from m in customerList
                         orderby m.CustomerID
                         select m;

            return View(customers);
        }

        // Edit Get + Edit Post to display change and save detail

        //this is the edit get to display selected customer
        public ActionResult Edit(int Id)
        {
            var customer = customerList.Single(m => m.CustomerID == Id);

            return View(customer);
        }

        // the lambda expression in the above method is the same as all of this below
        #region lambda example
        private Customer Lambda(List<Customer> customers, int Id)
        {
            foreach (var customer in customers)
            {
                if (customer.CustomerID == Id)
                {
                    return customer;
                }
            }

            return null;
        }
        #endregion lambda example

        // this is the edit post to update the selected customer
        [HttpPost]  //include HttpPost so that the compiler knows that this isn't the get method
        public ActionResult Edit(int Id, FormCollection collection)
        {
            try
            {
                var customer = customerList.Single(m => m.CustomerID == Id);
                if (TryUpdateModel(customer))
                    return RedirectToAction("Index");

                return View(customer);
            }
            catch
            {
                return View();
            }
        }

        // this is the Create Get to give an empty customer form
        public ActionResult Create()
        {
            return View();
        }

        //this is the Create Post to save the customer model
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                Customer customer = new Customer();

                //basically, this bit finds the 
                customer.CustomerID = (customerList.Count == 0) ? 1 :
                                customerList.Max(m => m.CustomerID) + 1;

                customer.CustomerName = collection["CustomerName"];
                customer.CustomerPhone = collection["CustomerPhone"];
                customerList.Add(customer);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(int Id)
        {
            var customer = customerList.Single(m => m.CustomerID == Id);

            return View(customer);
        }

        public ActionResult Delete(int Id)
        {
            var customer = customerList.Single(m => m.CustomerID == Id);

            return View(customer);
        }

        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                var customer = customerList.Single(m => m.CustomerID == Id);
                customerList.Remove(customer);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}