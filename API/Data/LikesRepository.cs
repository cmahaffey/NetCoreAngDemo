using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int likingUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(likingUserId, likedUserId);
        }
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(user => user.LikedUsers)
                .FirstOrDefaultAsync(user => user.Id == userId);
        }
        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(user => user.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likesParams.predicate == "liked")
            {
                likes = likes.Where(like => like.LikingUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if (likesParams.predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.LikingUser);
            }

            var likedUsers = users.Select(user => new LikeDTO
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }
    }
}