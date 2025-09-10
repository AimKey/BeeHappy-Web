using BusinessObjects;

namespace Services.Implementations;

public interface ITestObjectService
{
   Task<List<TestObject>> GetAllTestObjects(); 
}