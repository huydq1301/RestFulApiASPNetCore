using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories.Interfaces;

namespace RestApi.Repositories
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly ProductStoreContext _context;
		private readonly IMapper _mapper;

		public CategoryRepository(ProductStoreContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<int> AddCategoryAsync(CategoryModel category)
		{
			var newCategory = _mapper.Map<Category>(category);
			_context.Categories.Add(newCategory);
			await _context.SaveChangesAsync();
			return newCategory.CategoryId;
		}

		public async Task DeleteCategoryAsync(int id_Category)
		{
			var deleteCategory = _context.Categories!.SingleOrDefault(p => p.CategoryId == id_Category);
			if (deleteCategory != null && deleteCategory.IsDeleted == false)
			{
				deleteCategory.IsDeleted = true;
				_context.Categories!.Update(deleteCategory);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<List<CategoryModel>> GetAllCategoriesAsync()
		{
			var categories = await _context.Categories
				!.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value)
				.ToListAsync();
			var _categories = _mapper.Map<List<CategoryModel>>(categories);
			return _categories;
		}

		public async Task<List<ProductViewModel>> GetAllProductsByCategoryAsync(int id_Category)
		{
			var products = await _context.Products
				.Where(p => p.IsDeleted.HasValue && !p.IsDeleted.Value && p.CategoryId == id_Category)
				.ToListAsync();
			var _products = _mapper.Map<List<ProductViewModel>>(products);
			return _products;
		}

		public async Task<CategoryModel> GetCategoryByIdAsync(int id_Category)
		{
			var category = await _context.Categories!.FindAsync(id_Category);

			if (category != null && category.IsDeleted == false)
			{
				var categoryModel = _mapper.Map<CategoryModel>(category);
				return categoryModel;
			}
			return null;
		}
		public async Task<CategoryModel> GetCategoryByNameAsync(string name_Category)
		{
			int? categoryId = _context.Categories
				.Where(c => c.CategoryName == name_Category)
				.Select(c => (int?)c.CategoryId)
				.FirstOrDefault();

			if (!categoryId.HasValue)
			{
				return null;
			}
			var category = await _context.Categories!.FindAsync(categoryId);

			if (category != null && category.IsDeleted == false)
			{
				var categoryModel = _mapper.Map<CategoryModel>(category);
				return categoryModel;
			}
			return null;
		}

		public async Task UpdateCategoryAsync(int id_Category, CategoryVM category)
		{
			var existingCategory = await _context.Categories.FindAsync(id_Category);
			if (existingCategory != null)
			{
				// Update the existing category object with new property values
				existingCategory.CategoryName = category.CategoryName;
				existingCategory.Description = category.Description;
				await _context.SaveChangesAsync();
			}

		}
	}
}
