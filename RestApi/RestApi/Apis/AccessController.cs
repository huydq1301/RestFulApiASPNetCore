using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using RestApi.Entities;
using RestApi.Models.ViewModels;
using RestApi.Models;
using AutoMapper;

namespace RestApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccessController : ControllerBase
	{
		private readonly ProductStoreContext _context;
		private readonly JWTSettings _jwtsettings;
		private readonly IMapper _mapper;

		public AccessController(ProductStoreContext context, IOptions<JWTSettings> jwtsettings, IMapper mapper)
		{
			_context = context;
			_jwtsettings = jwtsettings.Value;
			_mapper = mapper;
		}

		[HttpPost("Login")]
		public async Task<ActionResult<UserWithToken>> Login([FromForm] LoginVM login)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username
						&& u.Password == login.Password && u.IsDeleted.HasValue && !u.IsDeleted.Value );

			var userModel = _mapper.Map<UserModel>(user);

			UserWithToken userWithToken = null;

			if (userModel != null)
			{
				RefreshToken refreshToken = GenerateRefreshToken();
				user.RefreshTokens.Add(refreshToken);
				await _context.SaveChangesAsync();

				userWithToken = new UserWithToken(userModel);
				userWithToken.RefreshToken = refreshToken.Token;
			}

			if (userWithToken == null)
			{
				return NotFound("Thông tin đăng nhập chưa chính xác");
			}

			//sign your token here here..
			userWithToken.AccessToken = GenerateAccessToken(userModel.UserId, userModel.RoleId);
			return userWithToken;

		}
		
		[HttpPost("RefreshToken")]
		public async Task<ActionResult<UserWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest)
		{
			var checkToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshRequest.RefreshToken);
			if (checkToken == null)
				return NotFound("TokenRefresh Không tồn tại");
			User user = await GetUserFromAccessToken(refreshRequest.AccessToken);
			var userModel = _mapper.Map<UserModel>(user);

			if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
			{
				UserWithToken userWithToken = new UserWithToken(userModel);
				userWithToken.AccessToken = GenerateAccessToken(user.UserId, user.RoleId);
				RefreshToken refreshToken = GenerateRefreshToken();
				user.RefreshTokens.Add(refreshToken);
				userWithToken.RefreshToken = refreshToken.Token;
				await _context.SaveChangesAsync();
				return userWithToken;
			}	
			return NotFound("TokenAccess Không tồn tại");
			
		}

		[HttpPost("GetUserByAccessToken")]
		public async Task<ActionResult<UserModel>> GetUserByAccessToken( string accessToken)
		{
			User user = await GetUserFromAccessToken(accessToken);
			var userModel = _mapper.Map<UserModel>(user);
			if (user.Username != null)
			{
				return userModel;
			}

			return NotFound("Token không tồn tại");
		}

		private bool ValidateRefreshToken(User user, string refreshToken)
		{

			RefreshToken refreshTokenUser = _context.RefreshTokens.Where(rt => rt.Token == refreshToken && rt.UserId == user.UserId)
												.OrderByDescending(rt => rt.ExpiryDate)
												.FirstOrDefault();

			if (refreshTokenUser != null && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
			{
				return true;
			}

			return false;
		}

		private async Task<User> GetUserFromAccessToken(string accessToken)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

				var tokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false
				};

				SecurityToken securityToken;
				var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

				JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

				if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				{
					var userId = principle.FindFirst("ID")?.Value;

					return await _context.Users.Include(u => u.Role)
										.Where(u => u.UserId == Convert.ToInt32(userId)).FirstOrDefaultAsync();
				}
			}
			catch (Exception)
			{
				return new User();
			}

			return new User();
		}

		private RefreshToken GenerateRefreshToken()
		{
			RefreshToken refreshToken = new RefreshToken();

			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				refreshToken.Token = Convert.ToBase64String(randomNumber);
			}
			refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

			return refreshToken;
		}

		private string GenerateAccessToken(int userId, int roleId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim("ID", Convert.ToString(userId)),
					new Claim("RoleId", Convert.ToString(roleId))

				}),
				Expires = DateTime.UtcNow.AddMinutes(10),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

	
	}
}
