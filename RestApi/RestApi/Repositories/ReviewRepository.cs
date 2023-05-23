using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Repositories.Interfaces;

namespace RestApi.Repositories
{
	public class ReviewRepository : IReviewRepository
	{
		private readonly ProductStoreContext _context;
		private readonly IMapper _mapper;

		public ReviewRepository(ProductStoreContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<int> AddReviewAsync(ReviewModel review)
		{
			var existingReview = await _context.Reviews.FirstOrDefaultAsync(r =>
			r.ProductId == review.ProductId && r.UserId == review.UserId);
			if (existingReview != null)
			{
				return -1;
			}
			else
			{
				var newReview = _mapper.Map<Review>(review);
				_context.Reviews.Add(newReview);
				await _context.SaveChangesAsync();
				return newReview.ReviewId;
			}	
		}

		public async Task DeleteReviewAsync(int id_Review)
		{
			var deleteReview = _context.Reviews!.SingleOrDefault(p => p.ReviewId == id_Review);
			if (deleteReview != null && deleteReview.IsDeleted == false)
			{
				deleteReview.IsDeleted = true;
				_context.Reviews!.Update(deleteReview);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<List<ReviewModel>> GetAllReviewsAsync()
		{
			var reviews = await _context.Reviews
				!.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value)
				.ToListAsync();
			var _reviews = _mapper.Map<List<ReviewModel>>(reviews);
			return _reviews;
		}

		public async Task<List<ReviewModel>> GetAllReviewsByUserAsync(int id_User)
		{
			var reviews = await _context.Reviews
				!.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value && p.UserId == id_User)
				.ToListAsync();
			var _reviews = _mapper.Map<List<ReviewModel>>(reviews);
			return _reviews;
		}

		public async Task<List<ProductViewModel>> GetProductsRecentReviewAsync(int limit)
		{
			var recentProducts = await _context.Products
				.Include(p => p.Reviews)
				.Where(p => p.Reviews.Any())
				.OrderByDescending(p => p.Reviews.Max(r => r.ReviewDate))
				.Take(limit)
				.ToListAsync();

			var _recentProducts = _mapper.Map<List<ProductViewModel>>(recentProducts);
			return _recentProducts;
		}


		public async Task<List<ProductViewModel>> GetProductsTopRateAsync(int limit)
		{
			var topRatedProducts = await _context.Products
			.Include(p => p.Reviews)
			.Where(p => p.Reviews.Any())
			.OrderByDescending(p => p.Reviews.Max(r => r.Rating))
			.Take(limit)
			.ToListAsync();

			var _topRatedProducts = _mapper.Map<List<ProductViewModel>>(topRatedProducts);
			return _topRatedProducts;
		}

		public async Task<ReviewModel> GetReviewByIdAsync(int id_Review)
		{
			var review = await _context.Reviews!.FindAsync(id_Review);

			if (review != null && review.IsDeleted == false)
			{
				var reviewModel = _mapper.Map<ReviewModel>(review);
				return reviewModel;
			}
			return null;
		}

		public async Task<ReviewModel> GetReviewProductByUserAsync(int id_User, int id_Product)
		{
			var review = await _context.Reviews
				.FirstOrDefaultAsync(p => p.IsDeleted.HasValue && !p.IsDeleted.Value && p.UserId == id_User && p.ProductId == id_Product);
			if(review ==null)
			{
				return null;
			}	
			var _review = _mapper.Map<ReviewModel>(review);
			return _review;
		}


		public async Task UpdateReviewAsync(int id_Review, int rating , string comment)
		{
			var existingReview = await _context.Reviews.FindAsync(id_Review);
			if (existingReview != null)
			{
				// Update the existing category object with new property values
				existingReview.Rating = rating;
				existingReview.Comment = comment;
				await _context.SaveChangesAsync();
			}
		}
	}
}
