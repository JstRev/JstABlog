using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Models.Entities;

namespace Application.Mappers
{
    public static class PostCreateMapper
    {
        public static PostCreateDto ToCreateDto(this Post post)
        {
            if(post != null)
            {
                return new PostCreateDto(
                    post.Title,
                    post.Content
                    );
            }
            else
            {
                throw new ArgumentNullException(nameof(post), "Post cannot be null.");
            }
        }

        public static Post ToCreateEntity(this PostCreateDto postDto)
        {
            if(postDto != null)
            {
                return new Post(
                    postDto.Title,
                    postDto.Content
                    );
            }
            else
            {
                throw new ArgumentNullException(nameof(postDto), "PostDto cannot be null.");
            }
        }
    }
}
