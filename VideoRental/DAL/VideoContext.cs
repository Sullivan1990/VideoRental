using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using VideoRental.Models;

namespace VideoRental.DAL
{
    public class VideoContext : DbContext
    {
        // appears to be creating tables based on the models we have defined
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalItem> RentalItems { get; set; }

        /* to fix the error of EnitityFramework not firing the VideoInitializer, create this constructor and force the VideoInitializer to fire.
        *  Also, in the VideoInitializer, change th instance from DropCreateDatabaseIfModelChanges to DropCreateDatabaseAlways
        *  For the initial run only, then change back to update if change
        *  this was a workaround to get the database to initially create, and then react to changes.
        */
        public VideoContext() : base("VideoRental")
        {
            Database.SetInitializer(new VideoInitializer());
        }

        // prevents Table mismatch by removing pluralisation from Model data
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}