using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Models.PingService;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace PingService.Repositories
{
    public class CredentialRepository : IRepositoryAll<Credentials>
    {
        private readonly PingDbContext db;
        private readonly AsyncPolicy policy;

        public CredentialRepository(PingDbContext context) 
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
                    MonitorService.Log.Error($"[Credential Repository @ Ping Service] : Circuit broken due to: {exception.Message}. Will reset after {timespan.TotalSeconds} seconds.");
                }, onReset: () => {
                    // SERILOG
                    MonitorService.Log.Information("[Credential Repository @ Ping Service] : Circuit closed, request flow returning to normal.");
                }
            );

            // Combined Policy
            policy = Policy
                .WrapAsync(
                    retryPolicy,
                    circuitBreakerPolicy
                );
        }

        public async Task<Credentials> PostAsync(Credentials entry)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Repository @ Ping Service] : POST"))
            {
                return await policy.ExecuteAsync(async () =>
                {
                    // Post, save, and return
                    await db.Credentials.AddAsync(entry);
                    await db.SaveChangesAsync();
                    return entry;
                });
            }
        }

        public async Task<Credentials?> GetAsync(int id)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Repository @ Ping Service] : GET"))
            {
                return await policy.ExecuteAsync(async () =>
                {
                    // Get and return
                    var credentials = await db.Credentials.FindAsync(id);
                    return credentials;
                });
            }
        }

        public async Task<IEnumerable<Credentials>> GetAllAsync()
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Repository @ Ping Service] : GET ALL"))
            {
                return await policy.ExecuteAsync(async () =>
                {
                    // Get all and return
                    var credentials = await db.Credentials.ToListAsync();
                    return credentials;
                });
            }
        }

        public async Task<Credentials> PutAsync(Credentials entry)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Repository @ Ping Service] : PUT"))
            {
                return await policy.ExecuteAsync(async () =>
                {
                    // Put, save, and return
                    var credentials = await db.Credentials.FindAsync(entry);

                    credentials!.Email = entry.Email;
                    credentials!.Password = entry.Password;

                    await db.SaveChangesAsync();
                    return credentials;
                });
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var activity = MonitorService.ActivitySource.StartActivity("[Credential Repository @ Ping Service] : DELETE"))
            {
                await policy.ExecuteAsync(async () =>
                {
                    // Get and verify existence
                    var credentials = await db.Credentials.FindAsync(id);
                    if (credentials == null)
                        return;

                    // Delete and save
                    db.Credentials.Remove(credentials);
                    await db.SaveChangesAsync();
                });
            }
        }
    }
}
