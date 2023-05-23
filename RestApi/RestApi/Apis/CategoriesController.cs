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
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryRepository _categoryRepo;

		public CategoriesController(ICategoryRepository repo) {
			_categoryRepo = repo;

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
		public async Task<IActionResult> GetAllCategoriesAsync()
		{
			try
			{
				return Ok(await _categoryRepo.GetAllCategoriesAsync());
			}
			catch
			{
				return BadRequest();
			}
		}
		[HttpGet("GetCategoryByIdAsync{id_Category}")]
		public async Task<IActionResult> GetCategoryByIdAsync(int id_Category)
		{
			var category = await _categoryRepo.GetCategoryByIdAsync(id_Category);
			return category == null ? NotFound($"Không tìm thấy danh mục có id = ''{id_Category}''") : Ok(category);
		}
		[HttpGet("GetAllProductsByCategoryAsync{id_Category}")]

		public async Task<IActionResult> GetAllProductsByCategoryAsync(int id_Category)
		{
			try
			{
				var aCategory = await _categoryRepo.GetCategoryByIdAsync(id_Category);
				if (aCategory == null)
				{
					return NotFound($"Danh mục '{id_Category}' không tồn tại");
				}
				var product = await _categoryRepo.GetAllProductsByCategoryAsync(id_Category);
				return product.Count == 0 ? NotFound($"Không có sản phẩm nào thuộc danh mục '{id_Category}'") : Ok(product);
			}
			catch
			{
				return BadRequest();
			}
		}
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddCategoryAsync(CategoryModel category)
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
				var newCategoryId = await _categoryRepo.AddCategoryAsync(category);
				var _category = await _categoryRepo.GetCategoryByIdAsync(newCategoryId);
				return _category == null ? NotFound() : Ok(_category);
			}
			catch
			{
				return BadRequest("Id này đã tồn tại");
			}
		}
		[Authorize]
		[HttpPut("UpdateCategoryAsync{id_Category}")]
		public async Task<IActionResult> UpdateCategoryAsync(int id_Category, [FromBody] CategoryVM category)
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
			var aProduct = await _categoryRepo.GetCategoryByIdAsync(id_Category);
			if (aProduct == null)
			{
				return NotFound($"Danh mục '{id_Category}' không tồn tại để sửa");
			}
			else
			{
				await _categoryRepo.UpdateCategoryAsync(id_Category, category);
				return Ok($"Sửa danh mục '{id_Category}' thành công");
			}

		}
		[Authorize]
		[HttpDelete("DeleteCategoryAsync{id_Category}")]
		public async Task<IActionResult> DeleteCategoryAsync( int id_Category)
		{
			var currentUser = GetCurrentUser();
			if (currentUser.RoleId != "1")
			{
				return Unauthorized("Chỉ Admin mới được thực hiện hành động này");
			}
			var category = await _categoryRepo.GetCategoryByIdAsync(id_Category);
			if (category == null)
			{
				return NotFound($"Danh mục  '{id_Category}' không tồn tại để xóa");
			}
			else
			{
				await _categoryRepo.DeleteCategoryAsync(id_Category);
				return Ok($"Xóa danh mục '{id_Category}' thành công");
			}

		}
	}
}
