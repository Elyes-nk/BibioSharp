using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archi.API.Model;
using Archi.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Archi.API.Data
{
    public class ArchiDbContext : DbContext
    {
        public ArchiDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSucces, CancellationToken cancellationToken)
        {
            ChangeAddedState();
            ChangeDeleteState();
            return base.SaveChangesAsync(acceptAllChangesOnSucces, cancellationToken);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeAddedState();
            ChangeDeleteState();
            return base.SaveChangesAsync(cancellationToken);
        }

        //----------------------  delete on database -----------------------

        private void ChangeDeleteState()
        {
            var entites = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);
            foreach (var item in entites)
            {
                item.State = EntityState.Modified;
                if (item.Entity is ModelBase)
                {
                    ((ModelBase)item.Entity).Active = false;
                }
            }
        }

        //---------------------- Add on database ---------------------------
        private void ChangeAddedState()
        {
            var entites = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
            foreach (var item in entites)
            {
                if (item.Entity is ModelBase)
                {
                    ((ModelBase)item.Entity).Active = true;
                }
            }
        }
        //------------------------------------------------------------------

        public DbSet<Product> Products { get; set; }
    }
}

