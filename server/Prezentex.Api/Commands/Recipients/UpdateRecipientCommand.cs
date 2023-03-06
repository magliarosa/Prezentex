using MediatR;
using Prezentex.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Api.Commands.Recipients
{
    public class UpdateRecipientCommand : IRequest<Recipient>
    {

        public Guid RecipientId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }
        public Guid UserId { get; set; }

        public UpdateRecipientCommand(Guid recipientId, string name, string note, DateTimeOffset bithDay, DateTimeOffset nameDay, Guid userId)
        {
            Name = name;
            Note = note;
            BirthDay = bithDay;
            NameDay = nameDay;
            UserId = userId;
            RecipientId = recipientId;
        }
    }
}
