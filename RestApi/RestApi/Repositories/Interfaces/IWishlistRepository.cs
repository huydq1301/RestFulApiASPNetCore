using RestApi.Models;

namespace RestApi.Repositories.Interfaces
{
	public interface IWishlistRepository
	{
		public Task<List<ProductViewModel>> GetProductInWishlistAsync(int id_User);
		public Task<WishlistModel> GetWishlistByIdAsync(int id_Wishlist);
		public Task<int> AddProductToWishlistAsync(int id_User, WishlistModel wishlist);
		public Task<bool> DeleteAProductInWishlistAsync(int id_User, int id_Product);
		public Task<bool> DeleteAllProductsInWishlistAsync(int id_User);
	}
}
