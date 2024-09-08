using AccountService.API.Extensions;
using AccountService.API.Grpc.Service;
using AccountService.Application.Commands.Departments;
using AccountService.Application.Commands.Users;
using AccountService.Application.Models.Users;
using AccountService.Application.Queries.Departments;
using AccountService.Application.Queries.User;
using AccountService.Domain.Entity;
using CustomHelper.Exception;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IMediator = MediatR.IMediator;

namespace AccountService.API.GrpcServices
{
    public class UserGrpc(IMediator mediator) : UserService.UserServiceBase
    {
        public override async Task<GetUserModel> GetUserById(UlidUser request, ServerCallContext context)
        {
            try
            {
                var user = await mediator.Send(new GetUserQuery(Ulid.Parse(request.Id.Value)), context.CancellationToken);

                var idUser = GrpcMapping.ConvertToUlidGrpc(user.Id);

                var result = new GetUserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LastSuccessfulEmailVerification = Timestamp.FromDateTime(user.LastSuccessfulEmailVerification.Value),
                    LastSuccessfulLogin = Timestamp.FromDateTime(user.LastSuccessfulLogin.Value),
                    IdUser = idUser,
                    MiddleName = user.FirstName,
                };

                Parallel.ForEach(user.Groups, groupId =>
                {
                    result.Departments.Add(GrpcMapping.ConvertToUlidGrpc(groupId));
                });

                return await Task.FromResult(result);
            }
            catch
            {
                throw;
            }
        }

        public override async Task<Empty> DeleteUserById(UlidUser request, ServerCallContext context)
        {
            try
            {
                var department = await mediator.Send(new DeleteUserCommand(Ulid.Parse(request.Id.Value)), context.CancellationToken);
                return new Empty();
            }
            catch
            {
                throw;
            }
        }

        public override async Task<GetUsersGetModel> GetUsers(GetUsersModel request, ServerCallContext context)
        {
            try
            {
                var users = await mediator.Send(new GetUsersQuery(new Application.Models.Users.UsersGetDTO
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    SortParameters = request.SortParameters.Select(x => new Application.Models.SortParameter
                    {
                        IsDescending = x.IsDescending,
                        PropertyName = x.PropertyName,
                    }).ToList(),
                }), context.CancellationToken);

                var result = new GetUsersGetModel();

                var getUserModels = users.AsParallel().Select(user =>
                {
                    var idUser = GrpcMapping.ConvertToUlidGrpc(user.Id);

                    var groups = user.Groups.Select(x => GrpcMapping.ConvertToUlidGrpc(x)).ToList();

                    return new GetUserModel
                    {
                        LastSuccessfulEmailVerification = Timestamp.FromDateTime(user.LastSuccessfulEmailVerification.Value),
                        FirstName = user.FirstName,
                        IdUser = idUser,
                        LastSuccessfulLogin = Timestamp.FromDateTime(user.LastSuccessfulLogin.Value),
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        Departments = { groups } // Add users to the Users list
                    };
                }).ToList();

                result.Users.AddRange(getUserModels);

                return await Task.FromResult(result);
            }
            catch
            {
                throw;
            }
        }

        public override async Task<GetUserModel> UpdateUserById(UpdateUserModel request, ServerCallContext context)
        {
            try
            {
                var user = await mediator.Send(new UpdateUserCommand(new Application.Models.Users.UserUpdateDTO
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Id = Ulid.Parse(request.IdUser.Value),

                }), context.CancellationToken);

                var idUser = GrpcMapping.ConvertToUlidGrpc(user.Id);

                var result = new GetUserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LastSuccessfulEmailVerification = Timestamp.FromDateTime(user.LastSuccessfullEmailVerification),
                    LastSuccessfulLogin = Timestamp.FromDateTime(user.LastSuccessfullLogin),
                    IdUser = idUser,
                    MiddleName = user.FirstName,
                };

                Parallel.ForEach(user.Departments, group =>
                {
                    result.Departments.Add(GrpcMapping.ConvertToUlidGrpc(group.Id));
                });

                return await Task.FromResult(result);
            }
            catch
            {
                throw;
            }
        }

        public override async Task<GetUserByIdsResponse> GetUsersByGroup(GetUserByIds request, ServerCallContext context)
        {
            try
            {
                var users = await mediator.Send(new GetUsersByIdsQuery(request.Ids.Select(x => x.ToString()).ToList()), context.CancellationToken);

                var response = new GetUserByIdsResponse
                {
                    Users = new GetUsersGetModel()
                };

                users.ForEach(user =>
                {
                    var idUser = GrpcMapping.ConvertToUlidGrpc(user.Id);

                    var groups = user.Groups is not null ?
                                user.Groups.Select(x => GrpcMapping.ConvertToUlidGrpc(x)).ToList() : [];

                    response.Users.Users.Add(new GetUserModel
                    {
                        LastSuccessfulEmailVerification = Timestamp.FromDateTime(
                            user.LastSuccessfulEmailVerification.HasValue
                                ? user.LastSuccessfulEmailVerification.Value.ToUniversalTime()
                                : DateTime.UtcNow),
                        FirstName = user.FirstName,
                        IdUser = idUser,
                        LastSuccessfulLogin = Timestamp.FromDateTime(
                            user.LastSuccessfulLogin.HasValue
                                ? user.LastSuccessfulLogin.Value.ToUniversalTime()
                                : DateTime.UtcNow),
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        Departments = { groups } // Add users to the Users list
                    });
                });

                return await Task.FromResult(response);
            }
            catch
            {
                throw;
            }
        }
    }
}
