using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestApi.Entities;
using RestApi.Models;
using RestApi.Models.ViewModels;
using RestApi.Repositories;
using RestApi.Repositories.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	// ...

	// Thêm cấu hình để sử dụng "Authorize" trên Swagger
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] { }
		}
	});
});
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
//builder.Services.AddIdentity<UserWithToken, IdentityRole>()
//	.AddEntityFrameworkStores<ProductStoreContext>().AddDefaultTokenProviders();
var jwtSection = builder.Configuration.GetSection("JWTSettings");
builder.Services.Configure<JWTSettings>(jwtSection);

//to validate the token which has been sent by clients
var appSettings = jwtSection.Get<JWTSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
	x.RequireHttpsMetadata = true;
	x.SaveToken = true;
	x.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = false,
		ValidateAudience = false,
		ClockSkew = TimeSpan.Zero
	};
});
var connectionString = builder.Configuration.GetConnectionString("ProductStoreContext");
builder.Services.AddDbContext<ProductStoreContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
