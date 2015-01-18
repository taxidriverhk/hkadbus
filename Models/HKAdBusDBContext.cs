using HKAdBus.Models.DomainModels;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HKAdBus.Models
{
    public class HKAdBusDBContext : DbContext
    {
        public HKAdBusDBContext() : base("HKAdBusDBContext")
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UpdateLog> Updates { get; set; }
        public DbSet<BusModel> BusModels { get; set; }
        public DbSet<GuestBookEntry> GuestBookEntries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}