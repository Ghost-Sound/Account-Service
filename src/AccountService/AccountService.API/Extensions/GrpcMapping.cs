using UlidGrpc;

namespace AccountService.API.Extensions
{
	public static class GrpcMapping
	{
		public static UlidGrpc.UlidGrpc ConvertToUlidGrpc(Ulid id)
		{
			return new UlidGrpc.UlidGrpc { Value = id.ToString() };
		}
	}
}
