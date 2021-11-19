using APILibrary.Test.Mock.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Archi.API.Data;
using System.Globalization;

namespace APILibrary.Test.Mock
{
    public class MockDbContext : ArchiDbContext
    {
        public MockDbContext(DbContextOptions options) : base(options)
        {
        }

        public static MockDbContext GetDbContext(bool withData = true)
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase("dbtest").Options;
            var db = new MockDbContext(options);

            if (withData)
            {
                string dateTime = "2019-09-09T00:00:00";
                DateTime dt = DateTime.ParseExact(dateTime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                db.Products.Add(new ProductMock { ID = 1, Name = "Pizza 1", Type = "pizza", Price = 10, Rating = 4, Date = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 2, Name = "Pizza 2", Type = "pizza", Price = 10, Rating = 4, Date = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 3, Name = "Pizza 3", Type = "pizza", Price = 10, Rating = 4, Date = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 4, Name = "Pizza 4", Type = "pizza", Price = 10, Rating = 4, Date = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 5, Name = "Pizza 5", Type = "pizza", Price = 10, Rating = 4, Date = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 6, Name = "Pizza 6", Type = "pizza", Price = 10, Rating = 4, Date = dt, Active = true });

                db.SaveChanges();
            }

            return db;
        }
    }
}
