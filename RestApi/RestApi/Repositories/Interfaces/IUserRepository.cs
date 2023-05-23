using RestApi.Models;
using RestApi.Models.ViewModels;

namespace RestApi.Repositories.Interfaces
{
	public interface IUserRepository
	{
		public Task<List<UserModel>> GetAllUsersAsync();
		public Task<UserModel> GetUserByIdAsync(int id_User);
		public Task<int> AddUserAsync(UserVM User);
		public Task UpdateUserAsync(int id_User, UserModel User);
		public Task DeleteUserAsync(int id_User);
	}
}
