using DataAccessObjects;

namespace Repositories.Generics;

public class UnitOfWork : IUnitOfWork
{
    private readonly BeeHappyContext _context;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(BeeHappyContext context)
    {
        _repositories = new Dictionary<Type, object>();
        this._context = context;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new GenericRepository<T>(_context);
            _repositories[type] = repositoryInstance;
        }

        return (IGenericRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}