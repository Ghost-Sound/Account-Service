using AccountService.Infrastructure.DB.Models;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.DB.Repositories
{
    public class ThirdPartyInitiatedRepository
    {
        private readonly ConfigurationDbContext _context;

        public ThirdPartyInitiatedRepository(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThirdPartyInitiatedLoginLink>> GetClientsWithLoginUris(string filter = null)
        {
            var query = _context.Clients
                .Where(c => c.InitiateLoginUri != null);

            if (!String.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(x => x.ClientId.Contains(filter) || x.ClientName.Contains(filter));
            }

            var result = query.Select(c => new ThirdPartyInitiatedLoginLink
            {
                LinkText = string.IsNullOrWhiteSpace(c.ClientName) ? c.ClientId : c.ClientName,
                InitiateLoginUri = c.InitiateLoginUri
            });

            return await result.ToArrayAsync();
        }
    }
}
