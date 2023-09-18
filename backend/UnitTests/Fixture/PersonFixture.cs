using Bogus;
using DomainModels.Models;
using System.Collections.Generic;

namespace UnitTests.Fixture
{
    public static class PersonFixture
    {
        public static Persons CreatePerson()
        {
            var faker = new Faker<Persons>()
                .RuleFor(p => p.Id, f => f.Random.Long())
                .RuleFor(p => p.FirstName, f => f.Person.FirstName)
                .RuleFor(p => p.LastName, f => f.Person.LastName)
                .RuleFor(p => p.Age, f => f.Random.Int(18, 200))
                .RuleFor(p => p.Profession, f => f.Random.String());

            return faker.Generate();
        }

        public static IEnumerable<Persons> CreatePersons(int quantity)
        {
            var faker = new Faker<Persons>()
                .RuleFor(p => p.Id, f => f.IndexFaker)
                .RuleFor(p => p.FirstName, f => f.Person.FirstName)
                .RuleFor(p => p.LastName, f => f.Person.LastName)
                .RuleFor(p => p.Age, f => f.Random.Int(18, 200))
                .RuleFor(p => p.Profession, f => f.Random.String())
                .Generate(quantity);

            return faker;
        }
    }
}
