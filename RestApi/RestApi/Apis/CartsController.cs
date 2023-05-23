using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace RestApi.Apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartsController : ControllerBase
	{
		private readonly ICartRepository _cartRepo;
		private readonly IUserRepository _UserRepo;
		private readonly IProductRepository _ProductRepo;

		public CartsController(ICartRepository repo, IUserRepository userRepo, IProductRepository productRepo)
		{
			_cartRepo = repo;
			_UserRepo = userRepo;
			_ProductRepo = productRepo;
		}
		private string GetCurrentUser()
		{
			var identity = HttpContext.User.Identity as ClaimsIdentity;
			string idUser = identity?.FindFirst("ID")?.Value;
			return idUser;
		}
		[Authorize]
		[HttpGet("{id_User}")]
		public async Task<IActionResult> GetProductInCartAsync(int id_User)
		{
			try
			{
				var user = await _UserRepo.GetUserByIdAsync(id_User);
				if (user == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser == id_User.ToString())
				{
					var product = await _cartRepo.GetProductInCartAsync(id_User);
					return product.Count == 0 ? NotFound($"Trong giỏ hàng của UserId = '{id_User}' không có sản phẩm nào") : Ok(product);
				}
				return Unauthorized("Bạn không có quyền truy cập giỏ hàng của người khác");
			}
			catch
			{
				return BadRequest();
			}
		}
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddProductToCartAsync(int id_User, CartModel cart)
		{
			try
			{
				var userId = await _UserRepo.GetUserByIdAsync(id_User);
				if (userId == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền truy cập giỏ hàng của người khác");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (id_User != cart.UserId)
				{
					return BadRequest($"id_User nhập không trùng khớp'");
				}
				var productId = await _ProductRepo.GetProductByIdAsync(cart.ProductId);
				if (productId == null)
				{
					return NotFound($"Không tồn tại sản phẩm nào có Id = '{cart.ProductId}'");
				}
				var newCartId = await _cartRepo.AddProductToCartAsync(id_User,cart);
				if (newCartId == -1)
				{
					return BadRequest($"Sản phẩm {cart.ProductId} đã tồn tại trong giỏ hàng của UserID = '{id_User}'");
				}
				var newCart = await _cartRepo.GetCartByIdAsync(newCartId);
				return newCart == null ? NotFound(): Ok(newCart);
			}
			catch
			{
				return BadRequest("IdCart  đã tồn tại ");
			}
		}
		[Authorize]
		[HttpPut("UpdateCartAsync{id_User}/{id_Product}")]
		public async Task<IActionResult> UpdateCartAsync(int id_User, int id_Product,[FromQuery] int quantity=1)
		{
			var userId = await _UserRepo.GetUserByIdAsync(id_User);
			if (userId == null)
			{
				return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
			}
			var currentUser = GetCurrentUser();
			if (currentUser != id_User.ToString())
			{
				return Unauthorized("Bạn không có quyền truy cập giỏ hàng của người khác");

			}
			if (quantity<0)
			{
				return BadRequest("quanity phải >0");
			}
			var productId = await _ProductRepo.GetProductByIdAsync(id_Product);
			if (productId == null)
			{
				return NotFound($"Không tồn tại sản phẩm nào có Id = '{id_Product}'");
			}
			var updateCart = await _cartRepo.UpdateCartAsync(id_User, id_Product, quantity);
			return updateCart == false 
				?NotFound($"Không tồn tại sản phẩm'{id_Product}' trong giỏ hàng của '{id_User}' ")
				: Ok($"Sửa thành công! Số lượng sản phẩm hiện tại là '{quantity}'");
		}
		[Authorize]
		[HttpDelete("DeleteAProductInCartAsync{id_User}/{id_Product}")]
		public async Task<IActionResult> DeleteAProductInCartAsync(int id_User, int id_Product)
		{
			try
			{
				var user = await _UserRepo.GetUserByIdAsync(id_User);
				if (user == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var productId = await _ProductRepo.GetProductByIdAsync(id_Product);
				if (productId == null)
				{
					return NotFound($"Không tồn tại sản phẩm nào có Id = '{id_Product}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền truy cập giỏ hàng của người khác");

				}
				bool check = await _cartRepo.DeleteAProductInCartAsync(id_User,id_Product);
				return check == false ? NotFound($"Sản phẩm '{id_Product}' không tồn tại trong giỏ hàng của '{id_User}' để xóa") 
					: Ok($"Sản phẩm '{id_Product}'  trong giỏ hàng của '{id_User}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}
			

		}
		[Authorize]

		[HttpDelete("DeleteAllProductsInCartAsync{id_User}")]
		public async Task<IActionResult> DeleteAllProductsInCartAsync(int id_User)
		{
			try
			{
				var user = await _UserRepo.GetUserByIdAsync(id_User);
				if (user == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền truy cập giỏ hàng của người khác");

				}
				bool check = await _cartRepo.DeleteAllProductsInCartAsync(id_User);
				return check == false ? NotFound($"Không tồn tại sản phẩm trong giỏ hàng của '{id_User}' để xóa")
					: Ok($"Tất cả sản phẩm  trong giỏ hàng của '{id_User}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}


		}
	}
}
