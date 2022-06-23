using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Thread_.NET.BLL.Services.Abstract;
using Thread_.NET.Common.DTO.Like;
using Thread_.NET.DAL.Context;

namespace Thread_.NET.BLL.Services
{
    public sealed class LikeService : BaseService
    {
        private readonly IConfiguration _config;
        public LikeService(ThreadContext context, IMapper mapper, IConfiguration config) : base(context, mapper) { 
            _config = config;
        }
       
        public async Task LikePost(NewReactionDTO reaction)
        {
            var likes = _context.PostReactions.Where(x => x.UserId == reaction.UserId && x.PostId == reaction.EntityId);

            if (likes.Any())
            {
                _context.PostReactions.RemoveRange(likes);
                await _context.SaveChangesAsync();

                return;
            }

            _context.PostReactions.Add(new DAL.Entities.PostReaction
            {
                PostId = reaction.EntityId,
                IsLike = reaction.IsLike,
                UserId = reaction.UserId
            });
            
            await _context.SaveChangesAsync();

            var posts = _context.Posts.Where(p => p.Id == reaction.EntityId).FirstOrDefault();
            var users = _context.Users.Where(u => u.Id == posts.AuthorId).FirstOrDefault();
            var whoLiked = _context.Users.Where(u => u.Id == reaction.UserId).FirstOrDefault();
            EmailService emailService = new EmailService(_config);
            await emailService.SendEmailAsync(users.Email, "someone liked your post", $"{whoLiked.UserName} liked your post.");
        }

        public async Task LikeComment(NewReactionDTO reaction)
        {
            var likes = _context.CommentReactions.Where(x => x.UserId == reaction.UserId && x.CommentId == reaction.EntityId);

            if (likes.Any())
            {
                _context.CommentReactions.RemoveRange(likes);
                await _context.SaveChangesAsync();

                return;
            }

            _context.CommentReactions.Add(new DAL.Entities.CommentReaction
            {
                CommentId = reaction.EntityId,
                IsLike = reaction.IsLike,
                UserId = reaction.UserId
            });

            await _context.SaveChangesAsync();
        }
    }
}
