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
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalItem> RentalItems { get; set; }

        // to fix the error of EF not firing the VideoInitializer, create this constructor and force the VideoInitializer to fire.
        // Also, in the VideoInitializer, change the Instance from DropCreateDatabaseIfModelChanges to DropCreateDatabaseAlways
        public VideoContext() : base("VideoRental")
        {
            Database.SetInitializer(new VideoInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}