using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories.Interfaces;
using System.Security.Claims;

namespace RestApi.Apis
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]

	public class OrdersController : ControllerBase
	{
		private readonly IOrderRepository _orderRepo;
		private readonly IUserRepository _UserRepo;
		private readonly IProductRepository _ProductRepo;

		public OrdersController(IOrderRepository repo, IUserRepository userRepo, IProductRepository productRepo)
		{
			_orderRepo = repo;
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
		[HttpPost]
		public async Task<IActionResult> AddOrderAsync(int id_User, OrderVM order)
		{
			try
			{
				var userId = await _UserRepo.GetUserByIdAsync(id_User);
				if (userId == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser.UserId != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (id_User != order.UserId)
				{
					return BadRequest($"id_User nhập không trùng khớp'");
				}
				var newOrder = await _orderRepo.AddOrderAsync(id_User, order);
				return newOrder == null ? NotFound() : Ok($"Tạo thành công Order có Id ={newOrder}");
			}
			catch
			{
				return BadRequest("IdOrder  đã tồn tại ");
			}
		}
		[Authorize]
		[HttpPost("AddOrderDetailAsync{id_User}/{id_Order}")]
		public async Task<IActionResult> AddOrderDetailAsync(int id_User, int id_Order, OrderDetailVM orderDetail)
		{
			try
			{
				var userId = await _UserRepo.GetUserByIdAsync(id_User);
				if (userId == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}

				var currentUser = GetCurrentUser();
				if (currentUser.UserId != id_User.ToString())
				{
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (id_Order != orderDetail.OrderId)
				{
					return BadRequest($"id_Order nhập không trùng khớp'");
				}
			
				var newOrderDetail = await _orderRepo.AddOrderDetailAsync(id_User, id_Order, orderDetail);
			
				if (newOrderDetail == -1)
				{
					return NotFound($"UserId = '{id_User}' không có OrderId = '{id_Order}' nào");
				}
				if (newOrderDetail == -2)
				{
					return NotFound($"Không tồn tại sản phẩm nào có ProductSize như bạn nhập");
				}
				if (newOrderDetail == -3)
				{
					return NotFound($"Số lượng nhập không hợp lệ");
				}
				return Ok($"Tạo thành công OrderDetail có Id ={newOrderDetail}");
			}
			catch
			{
				return BadRequest("IdOrderDetail  đã tồn tại ");
			}
		}
		[Authorize]
		[HttpGet("GetOrdersByUserAsync{id_User}")]
		public async Task<IActionResult> GetOrdersByUserAsync(int id_User)
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
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				var order = await _orderRepo.GetOrdersByUserAsync(id_User);
				return order.Count == 0 ? NotFound($"Không tồn tại Order của UserId = '{id_User}' ") : Ok(order);
			}
			catch
			{
				return BadRequest();
			}
		}
		[Authorize]
		[HttpGet("GetOrderDetailsAsync{id_User}/{id_Order}")]
		public async Task<IActionResult> GetOrderDetailsAsync(int id_User, int id_Order)
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
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				var orderDetails = await _orderRepo.GetOrderDetailsAsync(id_User, id_Order);
				if(orderDetails ==null)
				{
					return BadRequest($"UserId = '{id_User}' không có OrderId = '{id_Order}' nào");
				}
				return orderDetails.Count == 0 ? NotFound($"Không tồn tại OrderDetail của OrderID = '{id_Order}' ") : Ok(orderDetails);
			}
			catch
			{
				return BadRequest();
			}
		}
		[Authorize]
		[HttpDelete("DeleteAOrderByUserAsync{id_User}/{id_Order}")]
		public async Task<IActionResult> DeleteAOrderByUserAsync(int id_User, int id_Order)
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
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				bool check = await _orderRepo.DeleteAOrderByUserAsync(id_User, id_Order);
				if (check == false)
				{
					return	NotFound($"Order '{id_Order}' không tồn tại  trong UserID= '{id_User}' để xóa");
				}
				var deleteOrderDetails =  await _orderRepo.DeleteAllOrdersDetailByOrderIdAsync(id_User, id_Order);
				if (deleteOrderDetails == 0)
				{
					return NotFound($"Không tồn tại orderId = '{id_Order}' để xóa");

				}
				return Ok($"Order '{id_Order}'  của UserId = '{id_User}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}


		}
		[Authorize]
		[HttpDelete("DeleteAllOrdersByUserAsync{id_User}")]
		public async Task<IActionResult> DeleteAllOrdersByUserAsync(int id_User)
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
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				bool check = await _orderRepo.DeleteAllOrdersByUserAsync(id_User);
				return check == false ? NotFound($"Không tồn tại order của '{id_User}' để xóa")
					: Ok($"Tất cả order của  UserId =  '{id_User}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}
		}
		[Authorize]
		[HttpDelete("DeleteAOrderDetailByOrderIdAsync{id_User}/{id_Order}/{id_OrderDetail}")]
		public async Task<IActionResult> DeleteAOrderDetailByOrderIdAsync(int id_User, int id_Order, int id_OrderDetail)
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
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				int check = await _orderRepo.DeleteAOrderDetailByOrderIdAsync(id_User, id_Order, id_OrderDetail);
				if( check == -1) {
					return NotFound($"UserId = '{id_User}' không có OrderId = '{id_Order}' nào");

				}
				if( check == 0 )
				{
					return NotFound($"Không tồn tại OrderDetailId = '{id_OrderDetail}' trong OrderID = '{id_Order}'");

				}
				return  Ok($"OrderDetailId = '{id_OrderDetail}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}


		}
		[Authorize]
		[HttpDelete("DeleteAllOrdersDetailByOrderIdAsync{id_User}/{id_Order}")]
		public async Task<IActionResult> DeleteAllOrdersDetailByOrderIdAsync(int id_User, int id_Order)
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
					return Unauthorized("Bạn không có quyền truy cập vào order của người khác");
				}
				int check = await _orderRepo.DeleteAllOrdersDetailByOrderIdAsync(id_User, id_Order);
				if (check == -1)
				{
					return NotFound($"UserId = '{id_User}' không có OrderId = '{id_Order}' nào");

				}
				if (check == 0)
				{
					return NotFound($"Không tồn tại orderId = '{id_Order}' để xóa");

				}
				return Ok($"Tất cả orderDetail của  orderId =  '{id_Order}' đã được xóa");
			}
			catch
			{
				return BadRequest();
			}


		}
	}
}
