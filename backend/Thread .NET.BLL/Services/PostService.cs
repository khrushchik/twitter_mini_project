using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thread_.NET.BLL.Exceptions;
using Thread_.NET.BLL.Hubs;
using Thread_.NET.BLL.Services.Abstract;
using Thread_.NET.Common.DTO.Post;
using Thread_.NET.DAL.Context;
using Thread_.NET.DAL.Entities;

namespace Thread_.NET.BLL.Services
{
    public sealed class PostService : BaseService
    {
        private readonly IHubContext<PostHub> _postHub;
        private readonly IConfiguration _config;

        public PostService(ThreadContext context, IMapper mapper, IHubContext<PostHub> postHub, IConfiguration config) : base(context, mapper)
        {
            _postHub = postHub;
            _config = config;
        }

        public async Task<ICollection<PostDTO>> GetAllPosts()
        {
            var posts = await _context.Posts
                .Include(post => post.Author)
                    .ThenInclude(author => author.Avatar)
                .Include(post => post.Preview)
                .Include(post => post.Reactions)
                    .ThenInclude(reaction => reaction.User)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.Reactions)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.Author)
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            return _mapper.Map<ICollection<PostDTO>>(posts);
        }

        public async Task<ICollection<PostDTO>> GetAllPosts(int userId)
        {
            var posts = await _context.Posts
                .Include(post => post.Author)
                    .ThenInclude(author => author.Avatar)
                .Include(post => post.Preview)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.Author)
                .Where(p => p.AuthorId == userId) // Filter here
                .ToListAsync();

            return _mapper.Map<ICollection<PostDTO>>(posts);
        }

        public async Task<PostDTO> CreatePost(PostCreateDTO postDto)
        {
            var postEntity = _mapper.Map<Post>(postDto);

            _context.Posts.Add(postEntity);
            await _context.SaveChangesAsync();

            var createdPost = await _context.Posts
                .Include(post => post.Author)
					.ThenInclude(author => author.Avatar)
                .FirstAsync(post => post.Id == postEntity.Id);

            var createdPostDTO = _mapper.Map<PostDTO>(createdPost);
            await _postHub.Clients.All.SendAsync("NewPost", createdPostDTO);

            return createdPostDTO;
        }

        public async Task<PostDTO> DeletePost(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            IQueryable<Comment> comments = from o in _context.Comments where o.PostId == postId select o;
            IQueryable<PostReaction> postReactions = from o in _context.PostReactions where o.PostId == postId select o;
            foreach (Comment comment in comments)
            {
                var commentId = comment.Id;
                IQueryable<CommentReaction> commentReactions = from o in _context.CommentReactions
                                                               where o.CommentId == commentId select o;
                foreach (CommentReaction commentReaction in commentReactions)
                {
                    _context.CommentReactions.Remove(commentReaction);
                }
                _context.Comments.Remove(comment);
            }
            foreach(PostReaction postReaction in postReactions)
            {
                _context.PostReactions.Remove(postReaction);
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return _mapper.Map<PostDTO>(post);
        }

        public async Task UpdatePost(PostDTO postDto)
        {
            var postEntity = await _context.Posts
                .Include(post => post.Author)
                    .ThenInclude(author => author.Avatar)
                .Include(post => post.Preview)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.Author)
                .FirstOrDefaultAsync(p => p.Id == postDto.Id);
            if (postEntity == null)
            {
                throw new NotFoundException(nameof(Post), postDto.Id);
            }
            postEntity.Body = postDto.Body;
            if (!string.IsNullOrEmpty(postDto.PreviewImage))
            {
                if (postEntity.Preview == null)
                {
                    postEntity.Preview = new Image
                    {
                        URL = postDto.PreviewImage
                    };
                }
                else
                {
                    postEntity.Preview.URL = postDto.PreviewImage;
                }
            }
            else
            {
                if (postEntity.Preview != null)
                {
                    _context.Images.Remove(postEntity.Preview);
                }
            }
            _context.Posts.Update(postEntity);
            await _context.SaveChangesAsync();
        }

        public async Task SharePost(PostDTO postDto, string email)
        {
            var postEntity = await _context.Posts
                .Include(post => post.Author)
                    .ThenInclude(author => author.Avatar)
                .Include(post => post.Preview)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.Author)
                .FirstOrDefaultAsync(p => p.Id == postDto.Id);

            EmailService emailService = new EmailService(_config);
            var images = _context.Images.Where(i => i.Id == postEntity.PreviewId).FirstOrDefault();
            await emailService.SendPostByEmailAsync(email, "someome shared post with you", postEntity.Body, images.URL);
        }
    }
}
