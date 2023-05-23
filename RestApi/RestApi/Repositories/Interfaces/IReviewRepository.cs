using RestApi.Models;

namespace RestApi.Repositories.Interfaces
{
	public interface IReviewRepository
	{
		public Task<List<ReviewModel>> GetAllReviewsAsync();
		public Task<List<ReviewModel>> GetAllReviewsByUserAsync(int id_User);
		public Task<List<ProductViewModel>> GetProductsTopRateAsync(int limit);
		public Task<List<ProductViewModel>> GetProductsRecentReviewAsync(int limit);
		public Task<ReviewModel> GetReviewProductByUserAsync(int id_User, int id_Product);
		public Task<ReviewModel> GetReviewByIdAsync(int id_Review);
		public Task<int> AddReviewAsync(ReviewModel review);
		public Task UpdateReviewAsync(int id_Review, int rating, string comment);
		public Task DeleteReviewAsync(int id_Review);

	}
}
