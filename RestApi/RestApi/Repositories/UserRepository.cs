using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories.Interfaces;

namespace RestApi.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly ProductStoreContext _context;
		private readonly IMapper _mapper;

		public UserRepository(ProductStoreContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<int> AddUserAsync(UserVM User)
		{
			var newUser = _mapper.Map<User>(User);
			newUser.RoleId = 2;
			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();
			return newUser.UserId;
		}

		public async Task DeleteUserAsync(int id_User)
		{
			var deleteUser = _context.Users!.SingleOrDefault(p => p.UserId == id_User);
			if (deleteUser != null && deleteUser.IsDeleted == false)
			{
				deleteUser.IsDeleted = true;
				_context.Users!.Update(deleteUser);
				await _context.SaveChangesAsync();
			}
		}


		public async Task<List<UserModel>> GetAllUsersAsync()
		{
			var Users = await _context.Users
				!.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value)
				.ToListAsync();
			var _Users = _mapper.Map<List<UserModel>>(Users);
			return _Users;
		}

		public async Task<UserModel> GetUserByIdAsync(int id_User)
		{
			var User = await _context.Users!.FirstOrDefaultAsync(u => u.UserId == id_User && u.IsDeleted.HasValue && !u.IsDeleted.Value);

			if (User != null && User.IsDeleted == false)
			{
				var UserModel = _mapper.Map<UserModel>(User);
				return UserModel;
			}
			return null;
		}

		public async Task UpdateUserAsync(int id_User, UserModel User)
		{
			var existingUser = await _context.Users!.FirstOrDefaultAsync(u => u.UserId == id_User && u.IsDeleted.HasValue && !u.IsDeleted.Value);
			if (existingUser != null)
			{
				// Update the existing User object with new property values
				existingUser.Username = User.Username;
				existingUser.Password = User.Password;
				existingUser.Email = User.Email;
				existingUser.Phone = User.Phone;
				existingUser.Address = User.Address;
				existingUser.RoleId = User.RoleId;
				await _context.SaveChangesAsync();
			}
		}
	}
}
