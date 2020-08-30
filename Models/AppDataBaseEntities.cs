namespace AppWebServer.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class AppDataBaseEntities : DbContext
    {
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if ((entry.Entity.GetType() == typeof(EventPage) || entry.Entity.GetType() == typeof(Banner))&&
                    (entry.State==EntityState.Added||entry.State==EntityState.Deleted||entry.State==EntityState.Modified))
                {
                    Cache cache = Cache.Find("HomePageJSON");
                    if (cache != null)
                    {
                        cache.expired = false;
                        Entry(cache).State = EntityState.Modified;
                    }
                }
                else if ((entry.Entity.GetType() == typeof(StoreType) &&
                    (entry.State == EntityState.Added || entry.State == EntityState.Deleted || entry.State == EntityState.Modified)))
                {
                    Cache cache = Cache.Find("StoreType");
                    if (cache != null)
                    {
                        cache.expired = false;
                        Entry(cache).State = EntityState.Modified;
                    }
                }
                else if ((entry.Entity.GetType() == typeof(store)  &&
                    (entry.State == EntityState.Added || entry.State == EntityState.Deleted || entry.State == EntityState.Modified)))
                {
                    Cache cache = Cache.Find("StoreList");
                    if (cache != null)
                    {
                        cache.expired = false;
                        Entry(cache).State = EntityState.Modified;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}
