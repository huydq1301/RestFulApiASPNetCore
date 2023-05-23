using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories.Interfaces;

namespace RestApi.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ProductStoreContext _context;
		private readonly IMapper _mapper;

		public ProductRepository(ProductStoreContext context, IMapper mapper) {
			_context = context;
			_mapper = mapper;
		}
		public async Task<int> AddProductAsync(ProductModel product)
		{
			var newProduct = _mapper.Map<Product>(product);
			_context.Products.Add(newProduct);
			await _context.SaveChangesAsync();
			return newProduct.ProductId;
		}

		public async Task DeleteProductAsync(int id_product)
		{
			var deleteProduct = _context.Products!.SingleOrDefault(p => p.ProductId == id_product);
			if(deleteProduct != null && deleteProduct.IsDeleted==false)
			{
				deleteProduct.IsDeleted = true;
				_context.Products!.Update(deleteProduct);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> GetAllProductsAsync(int pageSize, int pageNumber = 1)
		{
			var TotalProducts = await _context.Products
				.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value)
				.CountAsync();

			if (TotalProducts == 0)
			{
				return (null, 0, 0);
			}
			var totalPages = (int)Math.Ceiling((double)TotalProducts / pageSize);
			var products = await _context.Products
				.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value )
				.Skip(pageSize * (pageNumber - 1))
				.Take(pageSize)
				.ToListAsync();

			var _products = _mapper.Map<List<ProductModel>>(products);

			return (_products, totalPages, TotalProducts);
		}


		public async Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> GetAllProductsByCategoryAsync(string name_category, int pageSize, int pageNumber = 1)
		{
			int? categoryId = _context.Categories
				.Where(c => c.CategoryName == name_category)
				.Select(c => (int?)c.CategoryId)
				.FirstOrDefault();

			var TotalProducts = await _context.Products
				.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value && p.CategoryId == categoryId)
				.CountAsync();

			if (TotalProducts == 0)
			{
				return (null,0,0);
			}
			var totalPages = (int)Math.Ceiling((double)TotalProducts / pageSize);

			var products = await _context.Products
				.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value && p.CategoryId == categoryId).
				Skip(pageSize * (pageNumber - 1))
				.Take(pageSize)
				.ToListAsync();

			var _products = _mapper.Map<List<ProductModel>>(products);

			return (_products, totalPages, TotalProducts);
		}





		public async Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> GetAllProductsByProductRelatedAsync(int id_product, int pageSize, int pageNumber = 1)
		{
			int? categoryId = _context.Products
				.Where(c => c.ProductId == id_product)
				.Select(c => (int?)c.CategoryId)
				.FirstOrDefault();

			if (!categoryId.HasValue)
			{
				return (null, 0, 0);
			}

			var products = await _context.Products
				.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value && p.CategoryId == categoryId)
				.ToListAsync();

			if (products == null || !products.Any())
			{
				return (null, 0, 0);
			}

			var _products = _mapper.Map<List<ProductModel>>(products);

			int totalProducts = _products.Count();
			int totalPages = (int)Math.Ceiling((decimal)totalProducts / pageSize);

			var pagedProducts = _products
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			return (pagedProducts, totalPages, totalProducts);
		}

		public async Task<ProductModel> GetProductByIdAsync(int id_product)
		{
			var product = await _context.Products!.FindAsync(id_product);

			if (product != null && product.IsDeleted == false)
			{
				var productModel = _mapper.Map<ProductModel>(product);
				return productModel;
			}
			return null;
		}

		public async Task<List<ProductVM>> GetProductInventoryBySizeAsync(int id_product)
		{
			var product = _context.Products.FirstOrDefault(p => p.ProductId == id_product && p.IsDeleted == false);
			if (product == null )
			{
				return null;
			}
			else
			{
				var productSizes = await _context.ProductSizes
							.Where(ps => ps.ProductId == id_product && ps.IsDeleted == false)
							.Select(ps => new ProductSizeModel
							{
								Size = ps.Size,
								Quantity = ps.Quantity
							})
							.ToListAsync();


				var _products = _mapper.Map<List<ProductVM>>(productSizes);
				return _products;
			}
			
		}

		public async Task<(List<ProductModel> Products, int TotalPages, int TotalProducts)> SearchAllProductsAsync(string? name, decimal? from, decimal? to, string? sortBy, int pageSize, int pageNumber = 1)
		{
			var allProducts = _context.Products.AsQueryable();
			if (!string.IsNullOrEmpty(name))
			{
				allProducts = allProducts.Where(p => p.ProductName.Contains(name) && p.IsDeleted.HasValue && !p.IsDeleted.Value);
			}
			if (from.HasValue)
			{
				allProducts = allProducts.Where(p => p.Price >= from && p.IsDeleted.HasValue && !p.IsDeleted.Value);
			}
			if (to.HasValue)
			{
				allProducts = allProducts.Where(p => p.Price <= to && p.IsDeleted.HasValue && !p.IsDeleted.Value);
			}
			allProducts = allProducts.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value);
			allProducts = allProducts.OrderBy(p => p.ProductName);
			if (!string.IsNullOrEmpty(sortBy))
			{
				switch (sortBy)
				{
					case "name_desc":
						allProducts = allProducts.OrderByDescending(p => p.ProductName);
						break;
					case "price_asc":
						allProducts = allProducts.OrderBy(p => p.Price);
						break;
					case "price_desc":
						allProducts = allProducts.OrderByDescending(p => p.Price);
						break;
					case "":
						allProducts = allProducts.OrderBy(p => p.ProductId);
						break;
					default:
						allProducts = allProducts.OrderBy(p => p.ProductId);
						break;
				}
			}
			var TotalProducts = await allProducts.CountAsync();
			if (TotalProducts == 0)
			{
				return (null, 0, 0);
			}
			var totalPages = (int)Math.Ceiling((double)TotalProducts / pageSize);
			allProducts = allProducts.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
			var result = allProducts.Select(p => new ProductModel
			{
				ProductId = p.ProductId,
				ProductName = p.ProductName,
				CategoryId = p.CategoryId,
				Price = p.Price,
				Description = p.Description,
				ImageUrl = p.ImageUrl,

			});
			var _products = _mapper.Map<List<ProductModel>>(result);
			return (_products, totalPages, TotalProducts);
		}

		public async Task UpdateProductAsync(int id_product, ProductViewModel product)
		{
			var existingProduct = await _context.Products.FindAsync(id_product);
			if (existingProduct != null)
			{
				// Update the existing category object with new property values
				existingProduct.ProductName = product.ProductName;
				existingProduct.Price = product.Price;
				existingProduct.Description = product.Description;
				existingProduct.CategoryId = product.CategoryId;
				existingProduct.ImageUrl = product.ImageUrl;
				await _context.SaveChangesAsync();
			}
		}
	}
}
