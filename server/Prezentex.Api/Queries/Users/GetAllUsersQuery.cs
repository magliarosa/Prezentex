using MediatR;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Queries.Users
{
    public class GetAllUsersQuery : IRequest<IEnumerable<User>>
    {
       public GetAllUsersQuery() { }
    }
}
