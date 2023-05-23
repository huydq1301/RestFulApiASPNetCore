using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Repositories.Interfaces;
using RestApi.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RestApi.Apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _UserRepo;

		public UsersController(IUserRepository repo)
		{
			_UserRepo = repo;

		}
		private UserRoleVM GetCurrentUser()
		{
			var identity = HttpContext.User.Identity as ClaimsIdentity;

			if (identity != null)
			{
				var userClaims = identity.Claims;

				return new UserRoleVM
				{
					UserId = userClaims.FirstOrDefault(o => o.Type == "ID")?.Value,
					RoleId = userClaims.FirstOrDefault(o => o.Type == "RoleId")?.Value,
				};
			}
			return null;

		}
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAllUsersAsync()
		{
			try
			{
				var currentUser = GetCurrentUser();
				if (currentUser.RoleId != "1")
				{
					return Unauthorized("Chỉ Admin mới được thực hiện hành động này");
				}
				return Ok(await _UserRepo.GetAllUsersAsync());
			}
			catch
			{
				return BadRequest();
			}
		}
		
		[Authorize]
		[HttpGet("GetUserByIdAsync{id_User}")]
		public async Task<IActionResult> GetUserByIdAsync(int id_User)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != "1")
			{
				return Unauthorized("Chỉ Admin mới được thực hiện hành động này");
			}
			var User = await _UserRepo.GetUserByIdAsync(id_User);
			return User == null ? NotFound($"Không tìm thấy người dùng có id = ''{id_User}''") : Ok(User);
		}

		[HttpPost]
		public async Task<IActionResult> AddUserAsync(UserVM User)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var newUserId = await _UserRepo.AddUserAsync(User);
				var _User = await _UserRepo.GetUserByIdAsync(newUserId);
				return _User == null ? NotFound() : Ok(_User);
			}
			catch
			{
				return BadRequest("Id này đã tồn tại");
			}
		}

		[Authorize]
		[HttpPut("UpdateUserAsync{id_User}")]
		public async Task<IActionResult> UpdateUserAsync(int id_User, [FromBody] UserModel User)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != id_User.ToString())
			{
				return Unauthorized("Người dùng chỉ có thể sửa thông tin của mình");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (id_User != User.UserId)
			{
				return NotFound($"Id nhập không trùng khớp");
			}
			var aUser = await _UserRepo.GetUserByIdAsync(id_User);
			if (aUser == null)
			{
				return NotFound($"người dùng '{id_User}' không tồn tại để sửa");
			}
			else
			{
				await _UserRepo.UpdateUserAsync(id_User, User);
				return Ok($"Sửa người dùng '{id_User}' thành công");
			}

		}
		
		[Authorize]
		[HttpDelete("DeleteUserAsync{id_User}")]
		public async Task<IActionResult> DeleteUserAsync(int id_User)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != "1" && currentUser.UserId != id_User.ToString())
			{
				return Unauthorized("Người dùng chỉ có thể xóa thông tin của bản thân. Chỉ Admin mới có thể xóa thông tin của mọi người");
			}
			var User = await _UserRepo.GetUserByIdAsync(id_User);

			if (User == null)
			{
				return NotFound($"Người dùng '{id_User}' không tồn tại để xóa");
			}
			else
			{
				await _UserRepo.DeleteUserAsync(id_User);
				return Ok($"Xóa người dùng '{id_User}' thành công");
			}

			}
		}
}
