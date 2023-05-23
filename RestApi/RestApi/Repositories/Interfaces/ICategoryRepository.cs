using RestApi.Models;
using RestApi.Models.ViewModels;

namespace RestApi.Repositories.Interfaces
{
	public interface ICategoryRepository
	{
		public Task<List<CategoryModel>> GetAllCategoriesAsync();
		public Task<CategoryModel> GetCategoryByIdAsync(int id_Category);
		public Task<CategoryModel> GetCategoryByNameAsync(string name_Category);
		public Task<List<ProductViewModel>> GetAllProductsByCategoryAsync(int id_Category);
		public Task<int> AddCategoryAsync(CategoryModel category);
		public Task UpdateCategoryAsync(int id_Category, CategoryVM category);
		public Task DeleteCategoryAsync(int id_Category);

	}
}
