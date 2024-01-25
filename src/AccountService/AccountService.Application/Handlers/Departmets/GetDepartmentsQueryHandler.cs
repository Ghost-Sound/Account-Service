﻿using AccountService.Application.Models.Departments;
using AccountService.Application.Models.Users;
using AccountService.Application.Queries.Departments;
using AccountService.Domain.Entity;
using AutoMapper;
using CustomHelper.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Handlers.Departmets
{
    public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, IList<GetDepartmentDTO>>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetDepartmentsQueryHandler(
            DbContext dbContext,
            IMapper mapper,
            ILogger logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IList<GetDepartmentDTO>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Set<Department>().AsNoTracking();

                if (request.GetDepartments.SortParameters.Any())
                {
                    var convertedSortParameters = request.GetDepartments.SortParameters
                                                .Select(sp => (sp.PropertyName, sp.IsDescending))
                                                .ToList();

                    query = query.OrderByDynamic(convertedSortParameters);
                }

                if (request.GetDepartments.Page > 0 && request.GetDepartments.PageSize > 0)
                {
                    query = query
                        .Skip((request.GetDepartments.Page - 1) * request.GetDepartments.PageSize)
                        .Take(request.GetDepartments.PageSize);
                }

                var departments = await query.ToListAsync(cancellationToken);

                var dtos = _mapper.ProjectTo<GetDepartmentDTO>((IQueryable)departments).ToList();

                return dtos;
            }
            catch
            {
                throw;
            }
        }
    }
}
