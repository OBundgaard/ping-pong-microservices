using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Models.PingService;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Serilog;

namespace PingService.Repositories
{
    public class PermissionRepository : IRepository<Permissions>
    {
        private readonly PingDbContext db;
        private readonly AsyncPolicy policy;

        public PermissionRepository(PingDbContext context)
        {
            // ENTITY FRAMEWORK
            db = context;

            // FAULT ISOLATION
            // Policy #1 : Retry
            AsyncRetryPolicy retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            // Policy #2 : Circuit Breaker
            AsyncCircuitBreakerPolicy circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), onBreak: (exception, timespan) => {
                    // SERILOG
                    MonitorService.Log.Error($"[Permission Repository @ Ping Service] : Circuit broken due to: {exception.Message}. Will reset after {timespan.TotalSeconds} seconds.");
                }, onReset: () => {
                    // SERILOG
                    MonitorService.Log.Information("[Permission Repository @ Ping Service] : Circuit closed, request flow returning to normal.");
                }
            );

            // Combined Policy
            policy = Policy
                .WrapAsync(
                    retryPolicy,
                    circuitBreakerPolicy
                );
        }

        public async Task<Permissions> PostAsync(Permissions entry)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Repository @ Ping Service] : POST"))
            {
                return await policy.ExecuteAsync(async () =>
                {
                    // Post, save, and return
                    await db.Permissions.AddAsync(entry);
                    await db.SaveChangesAsync();
                    return entry;
                });
            }
        }

        public async Task<Permissions?> GetAsync(int id)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Repository @ Ping Service] : GET"))
            {
                return await policy.ExecuteAsync(async () =>
                {
                    // Get and return
                    var permissions = await db.Permissions.FindAsync(id);
                    return permissions;
                });
            }
        }

        public async Task<Permissions> PutAsync(Permissions entry)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Repository @ Ping Service] : PUT"))
            {
                return await policy.ExecuteAsync(async () =>
                {
                    // Put, save, and return
                    var permissions = await db.Permissions.FindAsync(entry);

                    permissions!.CanCreate = entry.CanCreate;
                    permissions!.CanRead = entry.CanRead;
                    permissions!.CanUpdate = entry.CanUpdate;
                    permissions!.CanDelete = entry.CanDelete;

                    await db.SaveChangesAsync();
                    return permissions;
                });
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Permission Repository @ Ping Service] : DELETE"))
            {
                await policy.ExecuteAsync(async () =>
                {
                    // Get and verify existence
                    var permissions = await db.Permissions.FindAsync(id);
                    if (permissions == null)
                        return;

                    // Delete and save
                    db.Permissions.Remove(permissions);
                    await db.SaveChangesAsync();
                });
            }
        }
    }
}
