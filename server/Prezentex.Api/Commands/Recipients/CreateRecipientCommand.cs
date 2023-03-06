using MediatR;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Commands.Recipients
{
    public class CreateRecipientCommand : IRequest<Recipient>
    {
        public string Name { get; set; }
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }
        public string Note { get; set; }
        public Guid UserId { get; set; }

        public CreateRecipientCommand(string name, DateTimeOffset birthDay, DateTimeOffset nameDay, string note, Guid userId)
        {
            Name = name;
            BirthDay = birthDay;
            NameDay = nameDay;
            Note = note;
            UserId = userId;
        }
    }
}
