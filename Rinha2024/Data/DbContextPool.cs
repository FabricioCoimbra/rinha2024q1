using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Rinha2024.Data;

internal class DbContextPool
{
    private readonly ConcurrentQueue<AppDBContext> Pool;
    private readonly int maxPoolSize = 50;
    private int currentPoolSize = 0;
    private readonly object _lock = new object();

    public DbContextPool()
    {
        Pool = new ConcurrentQueue<AppDBContext>();
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            Pool.Enqueue(CreateNewDbContext());
            currentPoolSize++;
        }
    }

    public AppDBContext GetDbContext()
    {
        lock (_lock)
        {
            if (Pool.TryDequeue(out AppDBContext? context))
            {
                return context;
            }
            else
            {
                if (currentPoolSize < maxPoolSize)
                {
                    currentPoolSize++;
                    return CreateNewDbContext();
                }
                else
                {
                    Monitor.Wait(_lock);
                    return GetDbContext();
                }
            }
        }
    }

    public void ReleaseDbContext(AppDBContext context)
    {
        lock (_lock)
        {
            Pool.Enqueue(context);
            Monitor.Pulse(_lock);
        }
    }

    private AppDBContext CreateNewDbContext()
    {
        // Implemente aqui a lógica para criar e retornar um novo DbContext
        return new AppDBContext();
    }
}
