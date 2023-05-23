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
	public class ReviewsController : ControllerBase
	{
		private readonly IReviewRepository _reviewRepo;
		private readonly IUserRepository _UserRepo;
		private readonly IProductRepository _ProductRepo;

		public ReviewsController(IReviewRepository repo, IUserRepository userRepo, IProductRepository productRepo) {
			_reviewRepo = repo;
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
		[HttpGet]
		public async Task<IActionResult> GetAllReviewsAsync()
		{
			try
			{
				return Ok(await _reviewRepo.GetAllReviewsAsync());
			}
			catch
			{
				return BadRequest();
			}
		}

		[Authorize]
		[HttpGet("GetAllReviewsByUserAsync{id_User}")]
		public async Task<IActionResult> GetAllReviewsByUserAsync(int id_User)
		{
			try
			{
				var user = await _UserRepo.GetUserByIdAsync(id_User);
				if (user == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{id_User}'");
				}
				var reviews = await _reviewRepo.GetAllReviewsByUserAsync(id_User);
				return reviews.Count==0 ? NotFound("Người dùng này chưa đánh giá bài viết nào") : Ok(reviews);
			}
			catch
			{
				return BadRequest();
			}
		}

		[Authorize]
		[HttpGet("GetProductsTopRateAsync{limit}")]
		public async Task<IActionResult> GetProductsTopRateAsync(int limit)
		{
			try
			{
				var products = await _reviewRepo.GetProductsTopRateAsync(limit);
				return products == null ? NotFound() : Ok(products);
			}
			catch
			{
				return BadRequest();
			}
		}

		[Authorize]
		[HttpGet("GetProductsRecentReviewAsync{limit}")]
		public async Task<IActionResult> GetProductsRecentReviewAsync(int limit)
		{
			try
			{
				var products = await _reviewRepo.GetProductsRecentReviewAsync(limit);
				return products == null ? NotFound() : Ok(products);
			}
			catch
			{
				return BadRequest();
			}
		}

		[Authorize]
		[HttpGet("GetReviewProductByUserAsync{id_User}/{id_Product}")]
		public async Task<IActionResult> GetReviewProductByUserAsync(int id_User, int id_Product)
		{
			try
			{
				var aReview = await _reviewRepo.GetReviewProductByUserAsync(id_User, id_Product);
				return aReview == null ? NotFound("Không tìm thấy bài đánh giá phù hợp") : Ok(aReview);
			}
			catch
			{
				return BadRequest();
			}

		}

		[Authorize]
		[HttpGet("GetReviewByIdAsync{id_Review}")]
		public async Task<IActionResult> GetReviewByIdAsync(int id_Review)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != "1")
			{
				return Forbid("Chỉ Admin mới được thực hiện hành động này");
			}
			var review = await _reviewRepo.GetReviewByIdAsync(id_Review);
			return review == null ? NotFound($"Không tìm thấy đánh giá có id = ''{id_Review}''") : Ok(review);

		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddReviewAsync(ReviewModel review)
		{
			try
			{
				var userId = await _UserRepo.GetUserByIdAsync(review.UserId);
				if (userId == null)
				{
					return NotFound($"Không tồn tại người dùng nào có Id = '{review.UserId}'");
				}
				var currentUser = GetCurrentUser();
				if (currentUser.UserId != review.UserId.ToString())
				{
					return Forbid("Người dùng chỉ được đăng đánh giá của bản thân");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var productId = await _ProductRepo.GetProductByIdAsync(review.ProductId);
				if (productId == null)
				{
					return NotFound($"Không tồn tại sản phẩm nào có Id = '{review.ProductId}'");
				}
				var newReviewId = await _reviewRepo.AddReviewAsync(review);
				if(newReviewId==-1)
				{
					return BadRequest("Một người chỉ có 1 bài đánh giá cho 1 sản phẩm");
				}
				var _review = await _reviewRepo.GetReviewByIdAsync(newReviewId);
				return _review == null ? NotFound() : Ok(_review);
			}
			catch
			{
				return BadRequest("Đã xảy ra 1 trong các lỗi sau: 1.Id đánh giá  đã tồn tại 2. Id_User hoặc Id_Products bạn nhập không có trong cơ sở dữ liệu");
			}
		}
		
		[Authorize]
		[HttpPut("UpdateReviewAsync{id_Review}/{comment}")]
		public async Task<IActionResult> UpdateReviewAsync(int id_Review,[FromQuery] int rating, string comment)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != "1")
			{
				return Forbid("Chỉ Admin mới được thực hiện hành động này");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var aReview = await _reviewRepo.GetReviewByIdAsync(id_Review);
			
			if (aReview == null)
			{
				return NotFound($"Đánh giá '{id_Review}' không tồn tại để sửa");
			}
			else
			{
				await _reviewRepo.UpdateReviewAsync(id_Review, rating, comment);
				return Ok($"Sửa đánh giá '{id_Review}' thành công");
			}
		}

		[Authorize]
		[HttpDelete("DeleteReviewAsync{id_Review}")]
		public async Task<IActionResult> DeleteReviewAsync( int id_Review)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != "1")
			{
				return Forbid("Chỉ Admin mới được thực hiện hành động này");
			}
			var review = await _reviewRepo.GetReviewByIdAsync(id_Review);
			if(review == null)
			{
				return NotFound($"Đánh giá '{id_Review}' không tồn tại để xóa");
			}
			else
			{
				await _reviewRepo.DeleteReviewAsync(id_Review);
				return Ok($"Xóa Đánh giá '{id_Review}' thành công");
			}
			
		}
	}
}
