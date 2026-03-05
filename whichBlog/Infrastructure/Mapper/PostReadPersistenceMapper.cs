using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Dto;
using Domain.Models.Entities;

namespace Infrastructure.Mapper
{
    public static class PostReadPersistenceMapper
    {
        public static PostPersistenceDto ToPersistenceDto(this Post post)
        {
            return new PostPersistenceDto(
                post.Id.ToString(),
                post.Title,
                post.Content,
                ((DateTimeOffset)post.CreatedAt).ToUnixTimeSeconds() //converts DateTime to long Unix timestamp
                );
        }

        public static Post ToEntity(this PostPersistenceDto dto)
        {
            return new Post(
                new Guid(dto.Id),
                dto.Title,
                dto.Content,
                DateTimeOffset.FromUnixTimeSeconds(dto.TimeStamp).DateTime // converts long Unix timestamp back to DateTime
                );
        }
    }
}
