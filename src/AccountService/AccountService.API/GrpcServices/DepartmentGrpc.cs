using AccountService.API.Grpc.Service;
using AccountService.Application.Commands.Departments;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using AccountService.API.Extensions;
using IMediator = MediatR.IMediator;
using AccountService.Application.Queries.Departments;
using CustomHelper.Exception;

namespace AccountService.API.GrpcServices
{
	public class DepartmentGrpc(IMediator mediator) : DepartmentService.DepartmentServiceBase
	{
		public override async Task<GetDepartmentModel> CreateDepartment(CreateDepartmentModel request, ServerCallContext context)
		{
			try
			{
				
				var department = await mediator.Send(new CreateDepartmentCommand(new Application.Models.Departments.CreateDepartmentDTO
				{
					Description = request.Desctiption,
					Email = request.Email,
					Name = request.Name,
					PhoneNumber = request.PhoneNumber,
				    Users = request.Users.Select(x => Ulid.Parse(x.Value)).ToList(),
				}), context.CancellationToken);

				var idDepartment = GrpcMapping.ConvertToUlidGrpc(department.Id);

				var result = new GetDepartmentModel
				{
					CreationDate = Timestamp.FromDateTime(department.CreationDate.Value),
					Descrtiption = department.Description,
					IdDepartment = idDepartment,
					LastModifiedDate = Timestamp.FromDateTime(department.LastModifiedDate.Value),
					Email = department.Email,
					Name = department.Name,
					PhoneNumber = department.PhoneNumber,
				};

				Parallel.ForEach(request.Users, userId =>
				{
					  result.Users.Add(userId);
				});

				return await Task.FromResult(result);
			}
			catch
			{
				throw;
			}
		}

		public override async Task<Empty> DeleteDepartmentById(UlidDepartment request, ServerCallContext context)
		{
			try
			{
				var department = await mediator.Send(new DeleteDepartmentCommand(Ulid.Parse(request.Id.Value)), context.CancellationToken);
				return new Empty();
			}
			catch
			{
				throw;
			}
		}

		public override async Task<GetDepartmentModel> GetDepartmentById(UlidDepartment request, ServerCallContext context)
		{
			try
			{
				var department = await mediator.Send(new GetDepartmentQuery(Ulid.Parse(request.Id.Value)), context.CancellationToken);

				var idDepartment = GrpcMapping.ConvertToUlidGrpc(department.Id);

				var result = new GetDepartmentModel
				{
					CreationDate = Timestamp.FromDateTime(department.CreationDate.Value),
					Descrtiption = department.Description,
					IdDepartment = idDepartment,
					LastModifiedDate = Timestamp.FromDateTime(department.LastModifiedDate.Value),
					Email = department.Email,
					Name = department.Name,
					PhoneNumber = department.PhoneNumber,
				};

				Parallel.ForEach(department.Users, userId =>
				{
					result.Users.Add(GrpcMapping.ConvertToUlidGrpc(userId));
				});

				return await Task.FromResult(result);
			}
			catch
			{
				throw;
			}
		}

		public override async Task<GetDepartmentsGetModel> GetDepartments(GetDepartmentsModel request, ServerCallContext context)
		{
			try
			{
				var departments = await mediator.Send(new GetDepartmentsQuery(new Application.Models.Departments.GetDepartmentsDTO
				{
					Page = request.Page,
					PageSize = request.PageSize,
					SortParameters = request.SortParameters.Select(x => new Application.Models.SortParameter
					{
						IsDescending = x.IsDescending,
						PropertyName = x.PropertyName,
					}).ToList(),
				}), context.CancellationToken);

				var result = new GetDepartmentsGetModel();

				var getDepartmentModels = departments.AsParallel().Select(department =>
				{
					var idDepartment = GrpcMapping.ConvertToUlidGrpc(department.Id);

					var users = department.Users.Select(x => GrpcMapping.ConvertToUlidGrpc(x)).ToList();

					return new GetDepartmentModel
					{
						CreationDate = Timestamp.FromDateTime(department.CreationDate.Value),
						Descrtiption = department.Description,
						IdDepartment = idDepartment,
						LastModifiedDate = Timestamp.FromDateTime(department.LastModifiedDate.Value),
						Email = department.Email,
						Name = department.Name,
						PhoneNumber = department.PhoneNumber,
						Users = { users } // Add users to the Users list
					};
				}).ToList();

				result.Departments.AddRange(getDepartmentModels);

				return await Task.FromResult(result);
			}
			catch
			{
				throw;
			}
		}

		public override async Task<GetDepartmentModel> UpdateDepartmentById(UpdateDepartmentModel request, ServerCallContext context)
		{
			try
			{
				var department = await mediator.Send(new UpdateDepartmentCommand(new Application.Models.Departments.UpdateDepartmentDTO
				{
					Description = request.Desctiption,
					Email = request.Email,
					Id = Ulid.Parse(request.Id.Value),
					PhoneNumber = request.PhoneNumber,
					Name = request.Name,
					Users = request.Users.Select(x => Ulid.Parse(x.Value)).ToList(),
				}), context.CancellationToken);

				if (department == null)
				{
					throw new CustomException("Department is null");
				}

				var idDepartment = GrpcMapping.ConvertToUlidGrpc(department.Id);

				var result = new GetDepartmentModel
				{
					CreationDate = Timestamp.FromDateTime(department.CreationDate.Value),
					Descrtiption = department.Description,
					IdDepartment = idDepartment,
					LastModifiedDate = Timestamp.FromDateTime(department.LastModifiedDate.Value),
					Email = department.Email,
					Name = department.Name,
					PhoneNumber = department.PhoneNumber,
				};

				Parallel.ForEach(request.Users, userId =>
				{
					result.Users.Add(userId);
				});

				return await Task.FromResult(result);
			}
			catch
			{
				throw;
			}
		}
	}
}
