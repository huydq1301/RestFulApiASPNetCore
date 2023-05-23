using AutoMapper;
using Microsoft.AspNetCore.HttpLogging;
using System.Diagnostics.Eventing.Reader;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;

namespace RestApi.Helpers
{
	public class ApplicationMapper: Profile
	{
		public ApplicationMapper() 
		{
			CreateMap<Cart, CartModel>().ReverseMap();
			CreateMap<Category, CategoryModel>().ReverseMap();
			CreateMap<Contact, ContactModel>().ReverseMap();
			CreateMap<OrderDetail, OrderDetailModel>().ReverseMap();
			CreateMap<Order, OrderModel>().ReverseMap();
			CreateMap<Product, ProductViewModel>().ReverseMap();
			CreateMap<ProductSize, ProductSizeModel>().ReverseMap();
			CreateMap<Review, ReviewModel>().ReverseMap();
			CreateMap<Role, RoleModel>().ReverseMap();
			CreateMap<User, UserModel>().ReverseMap();
			CreateMap<Wishlist, WishlistModel>().ReverseMap();
			CreateMap<ProductSizeModel, ProductVM>().ReverseMap();
			CreateMap<Order, OrderVM>().ReverseMap();
			CreateMap<OrderDetail, OrderDetailVM>().ReverseMap();
			CreateMap<User, UserWithToken>().ReverseMap();
			CreateMap<Product, ProductModel>().ReverseMap();
			CreateMap<Category, CategoryVM>().ReverseMap();
			CreateMap<User, UserVM>().ReverseMap();

		}

	}
}
