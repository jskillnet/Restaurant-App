using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.IdentityServer;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.API.Data
{

    public class ConfigurationDbContextSeed
    {
        public async Task SeedAsync(
            ILogger<ConfigurationDbContextSeed> logger,
            ConfigurationDbContext context,
            IConfiguration configuration)
        {

            var policy = RetryPolicyCreator.CreatePolicy(logger, nameof(RestaurantDbContextSeed));
            await policy.ExecuteAsync(async () =>
            {
                var clientUrls = new Dictionary<string, string>();
                clientUrls.Add("MenuApiUrl", configuration["MENU_API_URL"]);
                clientUrls.Add("BasketApiUrl", configuration["BASKET_API_URL"]);

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients(clientUrls))
                    {
                        if (!context.Clients.Any(c => c.ClientId == client.ClientId))
                        {
                            logger.LogInformation($"Client: {client.ClientId} not found, and creating..");
                            await context.Clients.AddAsync(client.ToEntity());
                        }
                    }
                    var _ = context.ChangeTracker.HasChanges() ? await context.SaveChangesAsync() : 0;
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var identityResource in Config.GetIdentityResources())
                    {
                        if (!context.IdentityResources.Any(r => r.Name == identityResource.Name))
                        {
                            logger.LogInformation($"Resource: {identityResource.Name} not found, and creating it..");
                            await context.IdentityResources.AddAsync(identityResource.ToEntity());
                        }
                    }

                    var _ = context.ChangeTracker.HasChanges() ? await context.SaveChangesAsync() : 0;
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var apiResource in Config.GetApiResources())
                    {
                        if (!context.ApiResources.Any(ar => ar.Name == apiResource.Name))
                        {
                            logger.LogInformation($"Resource Api: {apiResource.Name} not found, and creating it..");
                            await context.ApiResources.AddAsync(apiResource.ToEntity());
                        }
                    }
                    var _ = context.ChangeTracker.HasChanges() ? await context.SaveChangesAsync() : 0;
                }
            });
        }
    }
}