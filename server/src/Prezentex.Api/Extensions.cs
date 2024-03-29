﻿using Prezentex.Api.Dtos;
using Prezentex.Api.DTOs;
using Prezentex.Domain.Entities;
using Prezentex.Domain.Identity;

namespace Prezentex.Api
{
    public static class Extensions
    {
        public static Dtos.GiftDto AsDto(this Gift gift)
        {
            return new Dtos.GiftDto(
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
                user.UserName,
                user.Gifts.Select(x => x.AsDto()).ToList(),
                user.Recipients,
                user.Email);
        }

        public static LoggedUserDto AsLoggedUserDto(this User user)
        {
            return new LoggedUserDto(
                    user.DisplayName,
                    user.Email,
                    null,
                    user.UserName);
        }

        public static Guid GetUserId(this HttpContext httpContext)
        {
            var plainId = httpContext.User.Claims.Single(x => x.Type == "id").Value;
            var id = Guid.Parse(plainId);
            return id;
        }
    }
}
