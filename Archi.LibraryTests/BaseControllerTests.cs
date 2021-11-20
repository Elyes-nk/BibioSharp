using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APILibrary.Test.Mock;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using Archi.Library.Controllers;
using ArchiAPI.Controllers;
using Archi.LibraryTests.Mock;

namespace Archi.LibraryTests
{
    public class Tests
    {
        private MockDbContext _contextSub;
        private MockUriService _uriService;
        private ProductsController _controllerP;

        [SetUp]
        public void Setup()
        {
            _contextSub = MockDbContext.GetDbContext();
            _controllerP = new ProductsController(_contextSub, _uriService);
        }

        [Test]
        public async Task GetAll()
        {
            var actionResult = await _controllerP.GetAll("", "", "", "", "", "");
            var result = actionResult.Result as ObjectResult;
            var values = ((IEnumerable<object>)(result).Value);

            Assert.AreEqual((int)System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(_contextSub.Products.Count(), values.Count());
        }
    }
}
