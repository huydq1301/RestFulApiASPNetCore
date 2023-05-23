using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;

namespace RestApi.Repositories.Interfaces
{
	public interface IProductRepository
	{
		public Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> GetAllProductsAsync( int pageSize, int pageNumber = 1);
		public Task<ProductModel> GetProductByIdAsync(int id_product);
		public Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> GetAllProductsByCategoryAsync(string name_category, int pageSize, int pageNumber = 1);
		public Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> GetAllProductsByProductRelatedAsync(int id_product, int pageSize, int pageNumber = 1);
		public Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> SearchAllProductsAsync(string? name, decimal? from, decimal? to, string? sortBy, int pageSize, int pageNumber = 1);
		public Task<List<ProductVM>> GetProductInventoryBySizeAsync(int id_product);

		public Task<int> AddProductAsync(ProductModel product);
		public Task UpdateProductAsync(int id_product, ProductViewModel product);
		public Task DeleteProductAsync(int id_product);
	}
}
