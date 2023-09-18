using DomainModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public interface IPersonService
    {
        Task<long> Create(Persons persons);
        IEnumerable<Persons> GetAll();
        Task<Persons> GetById(long id);
        void Update(long id, Persons persons);
        void Delete(long id);
    }
}
