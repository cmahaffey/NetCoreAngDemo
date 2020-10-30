using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var likingUserId = User.GetUserId();
            var likingUser = await _unitOfWork.LikesRepository.GetUserWithLikes(likingUserId);
            var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            if (likedUser == null) return NotFound();

            if (likingUser.UserName == username) return BadRequest("You can like your own account!");

            var userLike = await _unitOfWork.LikesRepository.GetUserLike(likingUserId, likedUser.Id);

            //TODO: setup toggle for deleting like
            if (userLike != null) return BadRequest("You already liked this user");

            userLike = new UserLike
            {
                LikingUserId = likingUser.Id,
                LikedUserId = likedUser.Id,
            };

            likingUser.LikedUsers.Add(userLike);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}