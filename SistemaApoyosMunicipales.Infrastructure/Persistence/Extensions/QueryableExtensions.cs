using SistemaApoyosMunicipales.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> PaginateAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            var totalRecords = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(
                    totalRecords / (double)pageSize),

                HasPreviousPage = pageNumber > 1,

                HasNextPage =
                    pageNumber <
                    (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }
    }
}
