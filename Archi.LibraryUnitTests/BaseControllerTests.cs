using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archi.Library.Controllers;
using Archi.Library.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Archi.LibraryUnitTests
{
    public class BaseControllerTests
    {
        private readonly Mock<DbContext> contextStub = new();
        private readonly Mock<IUriService> uriServiceStub = new();
        private readonly Mock<ModelBase> modelStub = new();

        private readonly Random rand = new();





        [Fact]
        public async Task GetContent_WithUnexistingItem_ReturnsNotFound()
        {
            // Arrange
            contextStub.Setup(repo => repo.GetContent(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new BaseController(contextStub.Object, uriServiceStub.Object, modelStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }









    }
}
