using Microsoft.EntityFrameworkCore;
using TechnicalTest.ClassLibrary.Interfaces;


namespace TechnicalTest.Data.Entities
{
    public class TechnicalTestDataContext : DbContext
    {
        public TechnicalTestDataContext(DbContextOptions<TechnicalTestDataContext>options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DisableCascadingDelete(modelBuilder);

            base.OnModelCreating(modelBuilder);

        }
        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var relationship in relationships)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }



        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        public int SaveAllChanges()
        {
            return base.SaveChanges();
        }
        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IEntityDelete && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));

            foreach (var entity in entities)
            {
                DateTime DateNow = DateTime.Now;
                if (entity.State == EntityState.Added)
                {
                    ((IEntityDelete)entity.Entity).Created = DateNow;
                }
                ((IEntityDelete)entity.Entity).Updated = DateNow;
                if (entity.State == EntityState.Deleted)
                {
                    ((IEntityDelete)entity.Entity).Deleted = DateNow;
                    entity.State = EntityState.Modified;
                }
            }

        }
    }
}
