using Prezentex.Api.Entities;
using MediatR;
using System.Collections.Generic;
using Prezentex.Api.Dtos;

namespace Prezentex.Api.Queries
{
    public class GetAllGiftsQuery : IRequest<IEnumerable<Gift>>
    {

    }
}
