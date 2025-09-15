using BusinessObjects;
using Repositories.Generics;
using Repositories.Interfaces;
using Services.Implementations;

namespace Services.Interfaces;

public class TestObjectService(ITestObjectRepository testObjectRepository) : ITestObjectService
{
    public async Task<List<TestObject>> GetAllTestObjects()
    {
        return await testObjectRepository.GetAllAsync();
    }
}