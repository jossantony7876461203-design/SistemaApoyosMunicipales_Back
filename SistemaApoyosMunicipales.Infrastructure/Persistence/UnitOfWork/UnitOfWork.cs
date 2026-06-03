using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.UnitOfWork
{
    public sealed class UnitOfWork
         : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(
            AppDbContext context
        )
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await _context
                .SaveChangesAsync(cancellationToken);
        }
    }
}
