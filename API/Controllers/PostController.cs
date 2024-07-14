using Business;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using PostEntity = DataAccess.Data.Post;

namespace API.Controllers.Post
{
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private BaseService<PostEntity> PostService;
        public PostController(BaseService<PostEntity> postService)
        {
            PostService = postService;
        }

        [HttpGet()]
        public IQueryable<PostEntity> GetAll()
        {
            return PostService.GetAll();
        }

        [HttpPost()]
        public PostEntity Create([FromBodyAttribute]  PostEntity entity)
        {
            return PostService.CreatePost(entity);
        }

        [HttpPut()]
        public PostEntity Update([FromBodyAttribute] PostEntity entity)
        {
            return PostService.CreatePost(entity);
        }

        [HttpDelete()]
        public PostEntity Delete([FromBodyAttribute] PostEntity entity)
        {
            return PostService.CreatePost(entity);
        }

        [HttpPost("create-multiple")]
        public IActionResult CreateMultiplePosts([FromBodyAttribute] List<PostEntity> posts)
        {
            try
            {
                // Llamar al servicio para crear los posts
                var createdPosts = PostService.CreateMultiplesPosts(posts);

                // Retornar una respuesta exitosa con los posts creados
                return Ok(createdPosts);
            }
            catch (Exception ex)
            {
                // Manejar errores y retornar un BadRequest con el mensaje de error
                return BadRequest($"Error al crear los posts: {ex.Message}");
            }
        }
    }
}

