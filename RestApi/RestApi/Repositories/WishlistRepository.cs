using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories.Interfaces;

namespace RestApi.Repositories
{
	public class WishlistRepository : IWishlistRepository
	{
		private readonly ProductStoreContext _context;
		private readonly IMapper _mapper;

		public WishlistRepository(ProductStoreContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<int> AddProductToWishlistAsync(int id_User, WishlistModel Wishlist)
		{
			var existingWishlist = await _context.Wishlists.FirstOrDefaultAsync(r =>
			r.ProductId == Wishlist.ProductId && r.UserId == Wishlist.UserId);
			if (existingWishlist != null && existingWishlist.IsDeleted.HasValue && !existingWishlist.IsDeleted.Value)
			{
				return -1;
			}
			if(existingWishlist != null && existingWishlist.IsDeleted == true)
			{
				existingWishlist.IsDeleted = false;
				await _context.SaveChangesAsync();
				return -2;
			}	
			var newWishlist = _mapper.Map<Wishlist>(Wishlist);
			_context.Wishlists.Add(newWishlist);
			await _context.SaveChangesAsync();
			return newWishlist.WishlistId;
		}

		public async Task<bool> DeleteAllProductsInWishlistAsync(int id_User)
		{
			var deleteWishlists = _context.Wishlists!.Where(p => p.UserId == id_User
			&& p.IsDeleted.HasValue && !p.IsDeleted.Value).ToList();
			if (deleteWishlists != null && deleteWishlists.Any())
			{
				foreach (var Wishlist in deleteWishlists)
				{
					Wishlist.IsDeleted = true;
					_context.Wishlists!.Update(Wishlist);
				}
				await _context.SaveChangesAsync();
				return true;
			}
			return false;

		}


		public async Task<bool> DeleteAProductInWishlistAsync(int id_User, int id_Product)
		{
			var deleteWishlist = await _context.Wishlists
				!.SingleOrDefaultAsync(p => p.UserId == id_User
					&& p.ProductId == id_Product
					&& p.IsDeleted.HasValue && !p.IsDeleted.Value);

			if (deleteWishlist != null)
			{
				deleteWishlist.IsDeleted = true;
				_context.Wishlists.Update(deleteWishlist);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<WishlistModel> GetWishlistByIdAsync(int id_Wishlist)
		{
			var Wishlist = await _context.Wishlists!.FindAsync(id_Wishlist);

			if (Wishlist != null && Wishlist.IsDeleted == false)
			{
				var WishlistModel = _mapper.Map<WishlistModel>(Wishlist);
				return WishlistModel;
			}
			return null;
		}

		public async Task<List<ProductViewModel>> GetProductInWishlistAsync(int id_User)
		{
			var productIds = await _context.Wishlists
				!.Where(c => c.UserId == id_User && c.IsDeleted.HasValue && !c.IsDeleted.Value)
				.Select(c => c.ProductId)
				.ToListAsync();
			var products = await _context.Products
			   !.Where(p => productIds.Contains(p.ProductId))
			   .ToListAsync();

			var _products = _mapper.Map<List<ProductViewModel>>(products);
			return _products;
		}
	}
}
