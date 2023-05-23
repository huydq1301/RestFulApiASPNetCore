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
	public class WishlistController : ControllerBase
	{
		private readonly IWishlistRepository _WishlistRepo;
		private readonly IUserRepository _UserRepo;
		private readonly IProductRepository _ProductRepo;

		public WishlistController(IWishlistRepository repo, IUserRepository userRepo, IProductRepository productRepo)
		{
			_WishlistRepo = repo;
			_UserRepo = userRepo;
			_ProductRepo = productRepo;
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
		[HttpGet("{id_User}")]
		public async Task<IActionResult> GetProductInWishlistAsync(int id_User)
		{
			try
			{
				var user = await _UserRepo.GetUserByIdAsync(id_User);
				if (user == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser.UserId != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền truy cập vào danh sách yêu thích của người khác");
				}
				var product = await _WishlistRepo.GetProductInWishlistAsync(id_User);
				return product.Count == 0 ? NotFound($"Trong sản phẩm yêu thích của UserId = '{id_User}' không có sản phẩm nào") : Ok(product);
			}
			catch
			{
				return BadRequest();
			}
		}
		
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddProductToWishlistAsync(int id_User, WishlistModel Wishlist)
		{
			try
			{
				var currentUser = GetCurrentUser();
				if (currentUser.UserId != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền hành động với danh sách yêu thích của người khác");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var userId = await _UserRepo.GetUserByIdAsync(id_User);
				if (userId == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}

				if (id_User != Wishlist.UserId)
				{
					return BadRequest($"id_User nhập không trùng khớp'");
				}
				var productId = await _ProductRepo.GetProductByIdAsync(Wishlist.ProductId);
				if (productId == null)
				{
					return NotFound($"Không tồn tại sản phẩm nào có Id = '{Wishlist.ProductId}'");
				}
				var newWishlistId = await _WishlistRepo.AddProductToWishlistAsync(id_User, Wishlist);
				if (newWishlistId == -1)
				{
					return BadRequest($"Sản phẩm {Wishlist.ProductId} đã tồn tại trong sản phẩm yêu thích của UserID = '{id_User}'");
				}
				if (newWishlistId == -2)
				{
					return Ok($"Sản phẩm {Wishlist.ProductId} đã được khôi phục trong sản phẩm yêu thích của UserID = '{id_User}'");
				}
				var newWishlist = await _WishlistRepo.GetWishlistByIdAsync(newWishlistId);
				return newWishlist == null ? NotFound() : Ok(newWishlist);
			}
			catch
			{
				return BadRequest("IdWishlist  đã tồn tại ");
			}
		}

		[Authorize]
		[HttpDelete("DeleteAProductInWishlistAsync{id_User}/{id_Product}")]
		public async Task<IActionResult> DeleteAProductInWishlistAsync(int id_User, int id_Product)
		{
			try
			{
				var user = await _UserRepo.GetUserByIdAsync(id_User);
				if (user == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser.UserId != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền hành động với danh sách yêu thích của người khác");
				}
				bool check = await _WishlistRepo.DeleteAProductInWishlistAsync(id_User, id_Product);
				return check == false ? NotFound($"Sản phẩm '{id_Product}' không tồn tại trong sản phẩm yêu thích của '{id_User}' để xóa")
					: Ok($"Sản phẩm '{id_Product}'  trong sản phẩm yêu thích của '{id_User}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}


		}
		
		[Authorize]		
		[HttpDelete("DeleteAllProductsInWishlistAsync{id_User}")]
		public async Task<IActionResult> DeleteAllProductsInWishlistAsync(int id_User)
		{
			try
			{
				var user = await _UserRepo.GetUserByIdAsync(id_User);
				if (user == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser.UserId != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền hành động với danh sách yêu thích của người khác");
				}
				bool check = await _WishlistRepo.DeleteAllProductsInWishlistAsync(id_User);
				return check == false ? NotFound($"Không tồn tại sản phẩm trong sản phẩm yêu thích của '{id_User}' để xóa")
					: Ok($" Tất cả sản phẩm  trong sản phẩm yêu thích của '{id_User}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}


		}
	}
}
