using MediatR;
using Prezentex.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Api.Commands.Users
{
    public class CreateUserCommand : IRequest<User>
    {
        public CreateUserCommand(string username, string email)
        {
            Username = username;
            Email = email;
        }

        public string Username { get; set; }
        public string Email { get; set; }

    }
}
