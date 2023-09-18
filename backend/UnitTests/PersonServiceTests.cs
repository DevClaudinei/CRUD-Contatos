using DomainServices.Services;
using EntityFrameworkCore.UnitOfWork.Interfaces;
using Moq;
using System.Threading.Tasks;
using UnitTests.Fixture;
using DomainModels.Models;
using EntityFrameworkCore.QueryBuilder.Interfaces;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq.Expressions;
using System;
using DomainServices.Exceptions;
using Infraestructure.Data;

namespace UnitTests
{
    public class PersonServiceTests
    {
        private readonly PersonService _personService;
        private readonly Mock<IUnitOfWork<DataContext>> _mockUnitOfWork;
        private readonly Mock<IRepositoryFactory<DataContext>> _mockRepositoryFactory;

        public PersonServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork<DataContext>>();
            _mockRepositoryFactory = new Mock<IRepositoryFactory<DataContext>>();
            _personService = new PersonService(_mockUnitOfWork.Object, _mockRepositoryFactory.Object);
        }

        [Fact]
        public async Task Should_RegisterPerson_ReturnStatusCodeCreated()
        {
            // Arrange
            var personFake = PersonFixture.CreatePerson();

            _mockUnitOfWork.Setup(x => x.Repository<Persons>().Add(It.IsAny<Persons>()))
                .Returns(personFake);

            // Act
            var personId = await _personService.Create(personFake);

            // Assert
            Assert.Equal(personFake.Id, personId);

            _mockUnitOfWork.Verify(x => x.Repository<Persons>().Add(It.IsAny<Persons>()), Times.Once());
        }

        [Fact]
        public void Should_ReturnAllPersons_WhenExecuteGetAll()
        {
            // Arrange
            var personFake = PersonFixture.CreatePersons(2);

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().MultipleResultQuery())
                .Returns(It.IsAny<IMultipleResultQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .Search(It.IsAny<IMultipleResultQuery<Persons>>()))
                .Returns((IList<Persons>)personFake);

            // Act
            var personsFound = _personService.GetAll();

            // Assert
            personsFound.Should().HaveCount(2);

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().MultipleResultQuery(), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .Search(It.IsAny<IMultipleResultQuery<Persons>>()), Times.Once());
        }

        [Fact]
        public void Should_ReturnEmpty_WhenExecuteGetAll()
        {
            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().MultipleResultQuery())
                .Returns(It.IsAny<IMultipleResultQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .Search(It.IsAny<IMultipleResultQuery<Persons>>()))
                .Returns(new List<Persons>());

            // Act
            var personsFound = _personService.GetAll();

            // Assert
            personsFound.Should().BeEmpty();

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().MultipleResultQuery(), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .Search(It.IsAny<IMultipleResultQuery<Persons>>()), Times.Once());
        }

        [Fact]
        public void Should_ReturnPerson_WhenExecuteGetById()
        {
            // Arrange
            var personFake = PersonFixture.CreatePerson();

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>())).Returns(It.IsAny<IQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default)).ReturnsAsync(personFake);

            // Act
            var personFound = _personService.GetById(personFake.Id);

            // Assert
            personFound.Id.Equals(personFake.Id);

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default), Times.Once());
        }

        [Fact]
        public async void Should_ReturnNull_WhenExecuteGetById()
        {
            // Arrange
            var personFake = PersonFixture.CreatePerson();

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()))
                .Returns(It.IsAny<IQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default));

            // Act
            var personFound = await _personService.GetById(personFake.Id);

            // Assert
            personFound.Should().BeNull();

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default), Times.Once());
        }

        [Fact]
        public void Should_UpdatePerson_WhenExecuteUpdate()
        {
            // Arrange
            var personFake = PersonFixture.CreatePerson();

            _mockUnitOfWork.Setup(x => x.Repository<Persons>().Update(It.IsAny<Persons>()));

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .Any(It.IsAny<Expression<Func<Persons, bool>>>()));

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()))
                .Returns(It.IsAny<IQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default))
                .ReturnsAsync(personFake);

            // Act
            _personService.Update(personFake.Id, personFake);

            // Assert
            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default), Times.Once());

            _mockUnitOfWork.Verify(x => x.Repository<Persons>().Update(It.IsAny<Persons>()), Times.Once());

            _mockUnitOfWork.Verify(x => x.SaveChanges(true, false), Times.Once());
        }

        [Fact]
        public void Should_ThrowNotFoudException_WhenExecuteUpdate()
        {
            // Arrange
            var personFake = PersonFixture.CreatePerson();

            _mockUnitOfWork.Setup(x => x.Repository<Persons>().Update(It.IsAny<Persons>()));

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .Any(It.IsAny<Expression<Func<Persons, bool>>>()));

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()))
                .Returns(It.IsAny<IQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default));

            // Act
            Action act = () => _personService.Update(personFake.Id, personFake);

            // Assert
            act.Should().ThrowExactly<NotFoundException>($"Person for Id: {personFake.Id} not found.");

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default), Times.Once());

            _mockUnitOfWork.Verify(x => x.Repository<Persons>().Update(It.IsAny<Persons>()), Times.Never());

            _mockUnitOfWork.Verify(x => x.SaveChanges(true, false), Times.Never());
        }

        [Fact]
        public void Should_DeletePerson_WhenExecuteDelete()
        {
            // Arrange
            var personFake = PersonFixture.CreatePerson();

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .Any(It.IsAny<Expression<Func<Persons, bool>>>()));

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()))
                .Returns(It.IsAny<IQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default))
                .ReturnsAsync(personFake);

            _mockUnitOfWork.Setup(x => x.Repository<Persons>()
                .Remove(It.IsAny<Expression<Func<Persons, bool>>>()));

            // Act
            _personService.Delete(personFake.Id);

            // Assert
            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default), Times.Once());

            _mockUnitOfWork.Verify(x => x.Repository<Persons>()
                .Remove(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Once());
        }

        [Fact]
        public void Should_ThrowNotFoundException_WhenExecuteDelete()
        {
            // Arrange
            var personFake = PersonFixture.CreatePerson();

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>())).Returns(It.IsAny<IQuery<Persons>>());

            _mockRepositoryFactory.Setup(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default));

            _mockUnitOfWork.Setup(x => x.Repository<Persons>().Remove(It.IsAny<Expression<Func<Persons, bool>>>()));

            // Act
            Action act = () => _personService.Delete(personFake.Id);

            // Assert
            act.Should().ThrowExactly<NotFoundException>($"Person for Id: {personFake.Id} not found.");

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>().SingleResultQuery()
                .AndFilter(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Once());

            _mockRepositoryFactory.Verify(x => x.Repository<Persons>()
                .SingleOrDefaultAsync(It.IsAny<IQuery<Persons>>(), default), Times.Once());

            _mockUnitOfWork.Verify(x => x.Repository<Persons>()
                .Remove(It.IsAny<Expression<Func<Persons, bool>>>()), Times.Never());
        }
    }
}
