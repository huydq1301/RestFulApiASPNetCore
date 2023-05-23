using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
	public class ProductsController : ControllerBase
	{
		private readonly IProductRepository _productRepo;
		private readonly ICategoryRepository _categoryRepo;

		public ProductsController(IProductRepository repo, ICategoryRepository categoryRepo) {
			_productRepo = repo;
			_categoryRepo = categoryRepo;
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

		[HttpGet]
		public async Task<IActionResult> GetAllProductsAsync(int pageSize, int pageNumber = 1)
		{
			try
			{
				var result = await _productRepo.GetAllProductsAsync(pageSize, pageNumber);
				if(result.Products == null) {
					return NotFound("Không có sản phẩm nào ");
				}
				if (pageNumber > result.TotalPages)
				{
					return NotFound($"Không có sản phẩm nào trang số  '{pageNumber}'");
				}
				var response = new ProductResponse
				{
					Products = result.Products,
					TotalPages = result.TotalPages,
					TotalProducts = result.TotalProducts
				};
				return Ok(response);
			}
			catch
			{
				return BadRequest("Dữ liệu nhập không hợp lệ( trang và kích thước trang phải >0 )");
			}
		}

		[HttpGet("SearchAllProductsAsync{pageSize}")]
		public async Task<IActionResult> SearchAllProductsAsync( string? name,  decimal? from, decimal? to,string? sortBy, int pageSize, int pageNumber = 1)
		{
			try
			{
				var result = await _productRepo.SearchAllProductsAsync(name, from, to, sortBy, pageSize, pageNumber);
				if (result.Products == null)
				{
					return NotFound("Không tìm thấy sản phẩm nào ");
				}
				if (pageNumber > result.TotalPages)
				{
					return NotFound($"Không có sản phẩm nào trang số  '{pageNumber}'");
				}
				var response = new ProductResponse
				{
					Products = result.Products,
					TotalPages = result.TotalPages,
					TotalProducts = result.TotalProducts
				};
				return Ok(response);

			}
			catch
			{ 
				return BadRequest(); 
			}
		}

		[HttpGet("GetAllProductsByCategoryAsync{name_category}/{pageSize}")]
		public async Task<IActionResult> GetAllProductsByCategoryAsync(string name_category, int pageSize, int pageNumber = 1)
		{
			try
			{
				var category = await _categoryRepo.GetCategoryByNameAsync(name_category);
				if( category == null)
				{
					return NotFound($"Danh mục '{name_category}' không tồn tại");
				}	
				var result = await _productRepo.GetAllProductsByCategoryAsync(name_category,  pageSize, pageNumber);
				if(result.Products == null || result.TotalPages == 0)
				{
					return NotFound($"Không có sản phẩm nào thuộc danh mục '{name_category}'"); 
				}
				if(pageNumber > result.TotalPages)
				{
					return NotFound($"Không có sản phẩm nào trang số  '{pageNumber}'");
				}
				var response = new ProductResponse
				{
					Products = result.Products,
					TotalPages = result.TotalPages,
					TotalProducts = result.TotalProducts
				};
				return Ok(response);
			}	
			catch
			{
				return BadRequest("Dữ liệu nhập không hợp lệ( trang và kích thước trang phải >0 )");
			}
		}

		[HttpGet("GetAllProductsByProductRelatedAsync{id_product}/{pageSize}")]
		public async Task<IActionResult> GetAllProductsByProductRelatedAsync(int id_product, int pageSize, int pageNumber = 1)
		{
			try
			{
				var product = await _productRepo.GetProductByIdAsync(id_product);
				if (product == null)
					return NotFound($"Không tìm thấy sản phẩm có id = ''{id_product}''");
	
					var result = await _productRepo.GetAllProductsByProductRelatedAsync(id_product, pageSize, pageNumber);
				if (result.Products == null || result.TotalPages == 0)
				{
					return NotFound($"Không có sản phẩm nào cùng danh mục với sản phẩm '{id_product}'");
				}
				if (pageNumber > result.TotalPages)
				{
					return NotFound($"Không có sản phẩm nào trang số  '{pageNumber}'");
				}
				var response = new ProductResponse
				{
					Products = result.Products,
					TotalPages = result.TotalPages,
					TotalProducts = result.TotalProducts
				};
				return Ok(response);
			}
			catch
			{
				return BadRequest("Dữ liệu nhập không hợp lệ( trang và kích thước trang phải >0 )");
			}
		}

		[HttpGet("GetProductByIdAsync{id_product}")]
		public async Task<IActionResult> GetProductByIdAsync(int id_product)
		{
			var product = await _productRepo.GetProductByIdAsync(id_product);
			return product == null ? NotFound($"Không tìm thấy sản phẩm có id = ''{id_product}''") : Ok(product);
		}

		[HttpGet("GetProductInventoryBySizeAsync{id_product}")]
		public async Task<IActionResult> GetProductInventoryBySizeAsync(int id_product)
		{
			var product = await _productRepo.GetProductInventoryBySizeAsync(id_product);
			if (product == null || product.Count == 0)
			{
				return BadRequest($"Sản phẩm '{id_product}' không tồn tại hoặc đã hết hàng");
			}
			else
			{
				return Ok(product);
			} 
			
		}
		[Authorize]

		[HttpPost]
		public async Task<IActionResult> AddProductAsync([FromBody] ProductModel product)
		{
			try
			{
				
				var currentUser = GetCurrentUser();
				if (currentUser.RoleId != "1")
				{
					return Unauthorized("Chỉ Admin mới được thực hiện hành động này");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var newProducId = await _productRepo.AddProductAsync(product);
				var _product = await _productRepo.GetProductByIdAsync(newProducId);
				return _product == null ? NotFound("Id này đã tồn tại") : Ok(_product);
			}
			catch
			{
				return BadRequest();
			}
		}
		[Authorize]

		[HttpPut("UpdateProductAsync{id_product}")]
		public async Task<IActionResult> UpdateProductAsync(int id_product, [FromBody] ProductViewModel product)
		{
			try
			{

				var currentUser = GetCurrentUser();
				if (currentUser.RoleId != "1")
				{
					return Unauthorized("Chỉ Admin mới được thực hiện hành động này");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var aProduct = await _productRepo.GetProductByIdAsync(id_product);

				if (aProduct == null)
				{
					return NotFound($"Sản phẩm '{id_product}' không tồn tại để sửa");
				}
				else
				{
					await _productRepo.UpdateProductAsync(id_product, product);
					return Ok($"Sửa sản phẩm '{id_product}' thành công");
				}
			}
			catch
			{
				return BadRequest();
			}
			
			
		}
		[Authorize]

		[HttpDelete("DeleteProductAsync{id_product}")]
		public async Task<IActionResult> DeleteProductAsync( int id_product)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != "1")
			{
				return Unauthorized("Chỉ Admin mới được thực hiện hành động này");
			}
			var product = await _productRepo.GetProductByIdAsync(id_product);
			if( product == null)
			{
				return NotFound($"Sản phẩm '{id_product}' không tồn tại để xóa");
			}
			else
			{
				await _productRepo.DeleteProductAsync(id_product);
				return Ok($"Xóa sản phẩm '{id_product}' thành công");
			}
			
		}
	}
}
