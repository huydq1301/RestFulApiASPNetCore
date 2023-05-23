namespace RestApi.Models.ViewModels
{
	public class ProductResponse
	{
		public int TotalProducts { get; set; }
		public int TotalPages { get; set; }

		public List<ProductModel> Products { get; set; }

	}
}
