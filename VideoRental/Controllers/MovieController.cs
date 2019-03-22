using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoRental.DAL;
using VideoRental.Models;

namespace VideoRental.Controllers
{
    public class MovieController : Controller
    {
        // GET: Movie
        //public ActionResult Index()
        //{
        //    return RedirectToAction("About", "Home");
        //}

        //public static List<Movie> movieList = new List<Movie>
        //{
        //    new Movie{MovieID = 1, Name = "The Avengers"},
        //    new Movie{MovieID = 2, Name = "Star Wars"},
        //    new Movie{MovieID = 3, Name = "The Matrix"}
        //};

        #region Demo code only
        [Route("{movie}/{index}/{pageIndex}/{sortBy}")]
        public ActionResult Index(int? pageIndex, string sortBy)
        {
            if (!pageIndex.HasValue) pageIndex = 1;
            if (string.IsNullOrWhiteSpace(sortBy)) sortBy = "Name";

            return Content($"Page Index = {pageIndex} and Sort By = {sortBy}");
        }

        public ActionResult DisplayVideo()
        {
            var movie = new Movie() { Name = "The Avengers" };

            return View(movie);
        }
        #endregion Demo code only

        private VideoContext db = new VideoContext();

        public ActionResult Index()
        {
            return View(db.Movies.ToList());
        }

        // Edit Get + Edit Post to display change and save detail

        //this is the edit get to display selected movie
        public ActionResult Edit(int Id)
        {
            var movie = db.Movies.Single(m => m.MovieID == Id);

            return View(movie);
        }

        // the lambda expression in the above method is the same as all of this below
        #region lambda example
        private Movie Lambda(List<Movie> movies, int Id)
        {
            foreach (var movie in movies)
            {
                if (movie.MovieID == Id)
                {
                    return movie;
                }
            }

            return null;
        }
        #endregion lambda example

        // this is the edit post to update the selected movie
        [HttpPost]  //include HttpPost so that the compiler knows that this isn't the get method
        public ActionResult Edit(int Id, FormCollection collection)
        {
            try
            {
                var movie = db.Movies.Single(m => m.MovieID == Id);
                if (TryUpdateModel(movie))
                {
                    db.SaveChanges();
                    /*  This is a call to the Alertify package, used to display a nice little
                    *   pop-up window in the corner of the webpage
                    */
                    TempData["SuccessMessage"] = "Movie updated successfully.";
                    return RedirectToAction("Index");
                }

                return View(movie);
            }
            catch
            {
                return View();
            }
        }

        // this is the Create Get to give an empty movie form
        public ActionResult Create()
        {
            return View();
        }

        //this is the Create Post to save the movie model
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                Movie movie = new Movie();

                //basically, this bit finds the 
                //movie.MovieID = (movieList.Count == 0) ? 1 : 
                //                movieList.Max(m => m.MovieID) + 1;

                movie.Name = collection["Name"];
                db.Movies.Add(movie);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Movie created successfully.";

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(int Id)
        {
            var movie = db.Movies.Single(m => m.MovieID == Id);

            return View(movie);
        }

        public ActionResult Delete(int Id)
        {
            var movie = db.Movies.Single(m => m.MovieID == Id);

            return View(movie);
        }

        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                var movie = db.Movies.Single(m => m.MovieID == Id);
                db.Movies.Remove(movie);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Movie deleted successfully.";

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}