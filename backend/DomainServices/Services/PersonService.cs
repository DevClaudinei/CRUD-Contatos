using DomainModels.Models;
using DomainServices.Exceptions;
using EntityFrameworkCore.UnitOfWork.Interfaces;
using Infraestructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public class PersonService : BaseService, IPersonService
    {
        public PersonService(
            IUnitOfWork<DataContext> unitOfWork,
            IRepositoryFactory<DataContext> repositoryFactory
        ) : base(unitOfWork, repositoryFactory) { }

        public async Task<long> Create(Persons persons)
        {
            var unitOfWork = UnitOfWork.Repository<Persons>();

            unitOfWork.Add(persons);
            await UnitOfWork.SaveChangesAsync();

            return persons.Id;
        }

        public IEnumerable<Persons> GetAll()
        {
            var repository = RepositoryFactory.Repository<Persons>();
            var query = repository.MultipleResultQuery();

            return repository.Search(query);
        }

        public async Task<Persons> GetById(long id)
        {
            var repository = RepositoryFactory.Repository<Persons>();
            var personFound = repository.SingleResultQuery()
                .AndFilter(x => x.Id.Equals(id));

            return await repository.SingleOrDefaultAsync(personFound);
        }

        private Persons EntityTransformation(Persons personToUpdate, Persons person)
        {
            personToUpdate.FirstName = person.FirstName;
            personToUpdate.LastName = person.LastName;
            personToUpdate.Age = person.Age;
            personToUpdate.Profession = person.Profession;

            return personToUpdate;
        }

        public void Update(long id, Persons persons)
        {
            var unitOfWork = UnitOfWork.Repository<Persons>();
            var personFound = GetById(id).Result;

            if (personFound is null)
                throw new NotFoundException($"Person for Id: {id} not found.");

            var personToUpdate = EntityTransformation(personFound, persons);

            unitOfWork.Update(personToUpdate);
            UnitOfWork.SaveChanges();
        }

        public void Delete(long id)
        {
            var unitOfWork = UnitOfWork.Repository<Persons>();
            var personFound = GetById(id).Result;

            if (personFound is null)
                throw new NotFoundException($"Person for Id: {id} not found.");

            unitOfWork.Remove(x => x.Id.Equals(id));
        }
    }
}
