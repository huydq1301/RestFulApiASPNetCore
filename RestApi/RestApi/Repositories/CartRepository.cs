using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories.Interfaces;

namespace RestApi.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly ProductStoreContext _context;
		private readonly IMapper _mapper;

		public CartRepository(ProductStoreContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<int> AddProductToCartAsync(int id_User, CartModel cart)
		{
			var existingCart= await _context.Carts.FirstOrDefaultAsync(r =>
			r.ProductId == cart.ProductId && r.UserId == cart.UserId);
			if (existingCart!= null)
			{
				return -1;
			}
			else
			{
				var newCart= _mapper.Map<Cart>(cart);
				_context.Carts.Add(newCart);
				await _context.SaveChangesAsync();
				return newCart.CartId;
			}
		}

		public async Task<bool> DeleteAllProductsInCartAsync(int id_User)
		{
			var deleteCarts = _context.Carts!.Where(p => p.UserId == id_User 
			&& p.IsDeleted.HasValue && !p.IsDeleted.Value).ToList();
			if (deleteCarts != null && deleteCarts.Any())
			{
				foreach (var cart in deleteCarts)
				{
					cart.IsDeleted = true;
					_context.Carts!.Update(cart);
				}
				await _context.SaveChangesAsync();
				return true;
			}
			return false;

		}


		public async Task<bool> DeleteAProductInCartAsync(int id_User, int id_Product)
		{
			var deleteCart = await _context.Carts
				!.SingleOrDefaultAsync(p => p.UserId == id_User
					&& p.ProductId == id_Product
					&& p.IsDeleted.HasValue && !p.IsDeleted.Value);

			if (deleteCart != null)
			{
				deleteCart.IsDeleted = true;
				_context.Carts.Update(deleteCart);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<CartModel> GetCartByIdAsync(int id_Cart)
		{
			var cart = await _context.Carts!.FindAsync(id_Cart);

			if (cart != null && cart.IsDeleted == false)
			{
				var cartModel = _mapper.Map<CartModel>(cart);
				return cartModel;
			}
			return null;
		}

		public async Task<List<ProductViewModel>> GetProductInCartAsync(int id_User)
		{
			var productIds = await _context.Carts
				!.Where(c => c.UserId == id_User && c.IsDeleted.HasValue && !c.IsDeleted.Value)
				.Select(c => c.ProductId)
				.ToListAsync();
			var products = await _context.Products
			   !.Where(p => productIds.Contains(p.ProductId))
			   .ToListAsync();

			var _products = _mapper.Map<List<ProductViewModel>>(products);
			return _products;
		}
		public async Task<bool> UpdateCartAsync(int id_User, int id_Product, int quantity)
		{
			var existingCart = await _context.Carts
				!.SingleOrDefaultAsync(p => p.UserId == id_User
					&& p.ProductId == id_Product
					&& p.IsDeleted.HasValue && !p.IsDeleted.Value);

			if (existingCart != null)
			{
				existingCart.Quantity = quantity;
				_context.Carts.Update(existingCart);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

	}
}
