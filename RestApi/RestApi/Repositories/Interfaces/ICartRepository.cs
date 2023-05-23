using RestApi.Models;
using RestApi.Models.ViewModels;

namespace RestApi.Repositories.Interfaces
{
	public interface ICartRepository
	{
		public Task<List<ProductViewModel>> GetProductInCartAsync(int id_User);
		public Task<CartModel> GetCartByIdAsync(int id_Cart);

		public Task<int> AddProductToCartAsync(int id_User, CartModel cart);
		public Task<bool> UpdateCartAsync(int id_User, int id_Product, int quantity);
		public Task<bool> DeleteAProductInCartAsync(int id_User, int id_Product);
		public Task<bool> DeleteAllProductsInCartAsync(int id_User);
	}
}
