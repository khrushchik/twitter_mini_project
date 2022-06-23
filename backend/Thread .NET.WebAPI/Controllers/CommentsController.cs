using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thread_.NET.BLL.Services;
using Thread_.NET.Common.DTO.Comment;
using Thread_.NET.Common.DTO.Like;
using Thread_.NET.Extensions;

namespace Thread_.NET.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;
        private readonly LikeService _likeService;

        public CommentsController(CommentService commentService, LikeService likeService)
        {
            _commentService = commentService;
            _likeService = likeService;
        }

        [HttpPost]
        public async Task<ActionResult<CommentDTO>> CreatePost([FromBody] NewCommentDTO comment)
        {
            comment.AuthorId = this.GetUserIdFromToken();
            return Ok(await _commentService.CreateComment(comment));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id, [FromBody] NewCommentDTO comment)
        {
            comment.AuthorId = this.GetUserIdFromToken();
            return Ok(await _commentService.DeleteComment(id));
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikeComment(NewReactionDTO reaction)
        {
            reaction.UserId = this.GetUserIdFromToken();

            await _likeService.LikeComment(reaction);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult<CommentDTO>> EditComment([FromBody] CommentDTO dto)
        {
            await _commentService.UpdateComment(dto);
            return NoContent();
        }
    }
}