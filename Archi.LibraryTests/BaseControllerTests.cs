using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APILibrary.Test.Mock;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ArchiAPI.Controllers;
using System;

namespace Archi.LibraryTests
{
    public class Tests
    {

        private MockDbContext _contextSub;
        private IUriService _uriService;
        private ProductsController _controllerP;

        //TODO fix uri 
        [SetUp]
        public void Setup()
        {
   
            var uri = _uriService.GetPageUri("", "");
            _contextSub = MockDbContext.GetDbContext();
            _controllerP = new ProductsController(_contextSub, uri);
        }


        [Test]
        public async Task Test1()
        {
            var actionResult = await _controllerP.GetAll("", "", "", "", "", "", "");
            var result = actionResult.Result as ObjectResult;
            var values = ((IEnumerable<object>)(result).Value);

            Assert.AreEqual((int)System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(_contextSub.Products.Count(), values.Count());
        }
    }
}
