using AccountService.API.Grpc.Service;
using AccountService.Application.Commands.Departments;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using AccountService.API.Extensions;
using IMediator = MediatR.IMediator;
using System.Threading;

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
	}
}
