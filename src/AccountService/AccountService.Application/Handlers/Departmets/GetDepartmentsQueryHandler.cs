using AccountService.Application.Models.Departments;
using AccountService.Application.Queries.Departments;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using CustomHelper.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Handlers.Departmets
{
    public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, IList<GetDepartmentDTO>>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDepartmentsQueryHandler> _logger;

        public GetDepartmentsQueryHandler(
            UserDbContext dbContext,
            IMapper mapper,
            ILogger<GetDepartmentsQueryHandler> logger)
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
