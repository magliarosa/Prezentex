﻿using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;

namespace Prezentex.Api
{
    public static class Extensions
    {
        public static GiftDto AsDto(this Gift gift)
        {
            return new GiftDto(
                gift.Id, 
                gift.CreatedDate, 
                gift.Name, 
                gift.Description, 
                gift.Price, 
                gift.ProductUrl,
                gift.Recipients.Select(recipient => recipient.AsDto()));
        }
        public static RecipientDto AsDto(this Recipient recipient)
        {
            return new RecipientDto(
                recipient.Id,
                recipient.CreatedDate,
                recipient.Name,
                recipient.Note,
                recipient.BirthDay,
                recipient.NameDay);
        }
        public static UserDto AsDto(this User user)
        {
            return new UserDto(
                user.Id,
                user.CreatedDate,
                user.Username,
                user.Gifts,
                user.Recipients,
                user.Email);
        }
    }
}
