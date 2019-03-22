using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VideoRental.DAL;
using VideoRental.Models;

namespace VideoRental.Controllers
{
    public class CustomerController : Controller
    {
        #region Demo code only
        //[Route("{customer}/{index}/{pageIndex}/{sortBy}")]
        //public ActionResult Index(int? pageIndex, string sortBy)
        //{
        //    if (!pageIndex.HasValue) pageIndex = 1;
        //    if (string.IsNullOrWhiteSpace(sortBy)) sortBy = "CustomerName";

        //    return Content($"Page Index = {pageIndex} and Sort By = {sortBy}");
        //}

        //public ActionResult DisplayCustomer()
        //{
        //    var customer = new Customer() { CustomerName = "Alice" };

        //    return View(customer);
        //}
        #endregion Demo code only
   
        // database is instantiated
        private VideoContext db = new VideoContext();


        public ActionResult Index()
        {
            /*  Retrieves all of the customers from the database
             *  Adds the customers to a list
             *  pass the list to the view
             */  
            return View(db.Customers.ToList());
        }

        // Edit Get + Edit Post to display change and save detail

        //this is the edit get to display selected customer
        public ActionResult Edit(int Id)
        {
            /*  This method is passed an integer that represents the Id of the 
             *  item that the edit button that was clicked is next too.
             *  
             *  The line below creates a customer variable and then 
             *  queries the database for a single record referred too as 'm' in this context
             *  'm' here represents each seperate customer record in the database
             *  'm' will be returned as a record to the customer variable
             *  only if the CustomerID of the record being checked matches the Id variable passed in 
             */

            var customer = db.Customers.Single(m => m.CustomerID == Id);

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
                /*
                 * 
                 */

                var customer = db.Customers.Single(m => m.CustomerID == Id);

                /*
                 * The If statement below will proceed if the results of trying to update the model
                 * with the model data passed in
                 * are sucessful, this can fail for multiple reasons, which is why it is a good idea
                 * to have this contained within a try-catch
                 */
                if (TryUpdateModel(customer))
                {
                    /*  If the model can be updated sucessfully,
                     *  Save the changes to the database and return
                     *  the user to the Index page
                     */
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                
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
        /* the FormCollection here represents all of the data entered into the webpage
        *  through the relevant inputs. it is passed into this Post function to allow us
        *  to access the data the user has entered
        */
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                Customer customer = new Customer();

                //basically, this bit finds the 
                //customer.CustomerID = (customerList.Count == 0) ? 1 :
                //                customerList.Max(m => m.CustomerID) + 1;


                /*
                 * form data can be accessed via: collection["x"] 
                 * where x = the name of the input object you wish to access
                 */
                customer.CustomerName = collection["CustomerName"];
                customer.CustomerPhone = collection["CustomerPhone"];
                db.Customers.Add(customer);
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
            var customer = db.Customers.Single(m => m.CustomerID == Id);

            return View(customer);
        }

        public ActionResult Delete(int Id)
        {
            var customer = db.Customers.Single(m => m.CustomerID == Id);

            return View(customer);
        }

        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                var customer = db.Customers.Single(m => m.CustomerID == Id);
                db.Customers.Remove(customer);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}