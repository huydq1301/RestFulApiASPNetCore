using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories.Interfaces;

namespace RestApi.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ProductStoreContext _context;
		private readonly IMapper _mapper;

		public OrderRepository(ProductStoreContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<int> AddOrderAsync(int id_User, OrderVM order)
		{
			var newOrder = _mapper.Map<Order>(order);
			newOrder.UserId = id_User;
			_context.Orders.Add(newOrder);
			await _context.SaveChangesAsync();
			return newOrder.OrderId;
		}

		public async Task<bool> DeleteAllOrdersByUserAsync(int id_User)
		{
			var deleteOrders = _context.Orders!.Where(p => p.UserId == id_User
			&& p.IsDeleted.HasValue && !p.IsDeleted.Value).ToList();
			if (deleteOrders != null && deleteOrders.Any())
			{
				foreach (var Order in deleteOrders)
				{
					Order.IsDeleted = true;
					_context.Orders!.Update(Order);
					var deleteOrderDetails = _context.OrderDetails!.Where(p => p.OrderId == Order.OrderId
			&& p.IsDeleted.HasValue && !p.IsDeleted.Value).ToList();
					if (deleteOrderDetails != null && deleteOrderDetails.Any())
					{
						foreach (var OrderDetail in deleteOrderDetails)
						{
							OrderDetail.IsDeleted = true;
							_context.OrderDetails!.Update(OrderDetail);
						}
						await _context.SaveChangesAsync();
					}
				}
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<bool> DeleteAOrderByUserAsync(int id_User, int id_Order)
		{
			var deleteOrder = await _context.Orders
				!.SingleOrDefaultAsync(p => p.UserId == id_User
					&& p.OrderId == id_Order
					&& p.IsDeleted.HasValue && !p.IsDeleted.Value);

			if (deleteOrder != null)
			{
				deleteOrder.IsDeleted = true;
				_context.Orders.Update(deleteOrder);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<List<OrderModel>> GetOrdersByUserAsync(int id_User)
		{
			var orders = await _context.Orders
			   !.Where(p => p.UserId == id_User && p.IsDeleted.HasValue && !p.IsDeleted.Value)
			   .ToListAsync();

			var _orders = _mapper.Map<List<OrderModel>>(orders);
			return _orders;
		}
		public async Task<List<OrderDetailModel>> GetOrderDetailsAsync(int id_User, int id_Order)
		{
			var existingOrder = await _context.Orders
				!.FirstOrDefaultAsync(o => o.OrderId == id_Order && o.UserId == id_User);
			if(existingOrder!= null)
			{
				var orderDetails = await _context.OrderDetails!.Where(x => x.OrderId==id_Order).ToListAsync();
				var _orders = _mapper.Map<List<OrderDetailModel>>(orderDetails);
				return _orders;
			}
			return null;
		}
		public async Task<int> AddOrderDetailAsync(int id_User, int id_Order, OrderDetailVM orderDetail)
		{
			var existingOrder = await _context.Orders
				!.FirstOrDefaultAsync(o => o.OrderId == id_Order && o.UserId == id_User && o.IsDeleted.HasValue && !o.IsDeleted.Value);
			if (existingOrder != null)
			{
				var newOrder = _mapper.Map<OrderDetail>(orderDetail);
				var checkProductSizeId = await _context.ProductSizes.FirstOrDefaultAsync(
					p => p.ProductSizeId == orderDetail.ProductSizeId && p.IsDeleted.HasValue
					&& !p.IsDeleted.Value);
				if (checkProductSizeId == null)
				{
					return -2;

				}
				if (orderDetail.Quantity < 1 || orderDetail.Quantity > checkProductSizeId.Quantity)
				{
					return -3;
				}

				newOrder.OrderId = id_Order;
				_context.OrderDetails.Add(newOrder);
				await _context.SaveChangesAsync();
				return newOrder.OrderDetailId;
			}
			return -1;

		}
		public async Task<int> DeleteAOrderDetailByOrderIdAsync(int id_User, int id_Order, int id_OrderDetail)
		{
			var existingOrder = await _context.Orders
				!.FirstOrDefaultAsync(o => o.OrderId == id_Order && o.UserId == id_User);
			if (existingOrder != null)
			{
				var deleteOrderDetail = await _context.OrderDetails
				!.SingleOrDefaultAsync(p => p.OrderDetailId == id_OrderDetail && p.OrderId == id_Order
					&& p.IsDeleted.HasValue && !p.IsDeleted.Value);

				if (deleteOrderDetail != null)
				{
					deleteOrderDetail.IsDeleted = true;
					_context.OrderDetails.Update(deleteOrderDetail);
					await _context.SaveChangesAsync();
					return 1;
				}
				return 0;
			}
			return -1;
			
		}
		public async Task<int> DeleteAllOrdersDetailByOrderIdAsync(int id_User, int id_Order)
		{
			var existingOrder = await _context.Orders
				!.FirstOrDefaultAsync(o => o.OrderId == id_Order && o.UserId == id_User);
			if (existingOrder != null)
			{
				var deleteOrderDetails = _context.OrderDetails!.Where(p => p.OrderId == id_Order
			&& p.IsDeleted.HasValue && !p.IsDeleted.Value).ToList();
				if (deleteOrderDetails != null && deleteOrderDetails.Any())
				{
					foreach (var OrderDetail in deleteOrderDetails)
					{
						OrderDetail.IsDeleted = true;
						_context.OrderDetails!.Update(OrderDetail);
					}
					await _context.SaveChangesAsync();
					return 1;
				}
				return 0;
			}
			return -1;


		}
	}
}
