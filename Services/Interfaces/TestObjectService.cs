using BusinessObjects;
using Repositories.Generics;
using Repositories.Interfaces;
using Services.Implementations;

namespace Services.Interfaces;

public class TestObjectService(IUnitOfWork uow) : ITestObjectService
{
    public async Task<List<TestObject>> GetAllTestObjects()
    {
        var testObjectRepository = uow.Repository<TestObject>();
        await uow.SaveChangesAsync();
        var list = testObjectRepository.Get();
        return list.ToList();
    }
}