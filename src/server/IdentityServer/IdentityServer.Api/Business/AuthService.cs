using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Data.Repository;
using IdentityServer.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityServer.Api.Business
{
    public class AuthService(IUserRepository userRepository, IConfiguration configuration)
        : BaseService<IUserRepository, AppUser>(userRepository), IAuthService
    {
        public async Task<ResponseDto<LoginResponseModel>> LoginAsync(LoginRequestModel loginRequest)
        {
            var user = await Repository.GetByUsername(loginRequest.Username);
            if (user is null)
                return ReturnFail<LoginResponseModel>("Invalid username or password!", HttpStatusCode.NotFound);

            byte[] salt = user.PasswordSalt.ToByteArray();
            var hashedPassword = HashPassword(loginRequest.Password, salt);
            if (hashedPassword != user.HashedPassword)
                return ReturnFail<LoginResponseModel>("Invalid username or password!", HttpStatusCode.NotFound);

            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new("fullname", user.Fullname)
            };

            var encodedJwt = GenerateJwtToken(claims);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(2);
            await Repository.SaveChangesAsync();

            var response = new LoginResponseModel
            {
                UserId = user.Id,
                AccessToken = encodedJwt,
                RefreshToken = refreshToken,
                Username = user.Username
            };

            return ReturnSuccess(response);
        }

        public async Task<ResponseDto<bool>> RegisterAsync(RegisterRequestModel registerModel)
        {
            var userExists = await Repository.GetByUsername(registerModel.UserName);
            if (userExists is not null)
                return ReturnFail<bool>("User already exists!", HttpStatusCode.BadRequest);

            //önce validasyon olmalı, sonra eklenecek
            var passwordSalt = Guid.NewGuid();
            byte[] salt = passwordSalt.ToByteArray();
            var newAppUser = new AppUser
            {
                Fullname = registerModel.Fullname,
                Username = registerModel.UserName,
                Email = registerModel.Email,
                HashedPassword = HashPassword(registerModel.Password, salt),
                PasswordSalt = passwordSalt,
                ProfileImage = Path.Combine(Directory.GetCurrentDirectory(), "images/users/profile/default.jpg")
            };
            await Repository.AddAsync(newAppUser);
            return await SaveChangesAsync();
        }

        public async Task<ResponseDto<LoginResponseModel>> RefreshTokenAsync(RefreshTokenRequestModel request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return ReturnFail<LoginResponseModel>("User not found!", HttpStatusCode.BadRequest);

            var user = await Repository.GetFirstAsync(x => x.Id == int.Parse(userId) && x.RefreshToken == request.RefreshToken);

            if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return ReturnFail<LoginResponseModel>("Expired Token!", HttpStatusCode.BadRequest);
            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new("fullname", user.Fullname)
            };
            var newAccessToken = GenerateJwtToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await Repository.SaveChangesAsync();

            var response = new LoginResponseModel
            {
                UserId = user.Id,
                Username = user.Username,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return ReturnSuccess(response);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:SecretKey"])),
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            return securityToken is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
                ? principal
                : null;
        }

        private string GenerateJwtToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:SecretKey"]));
            var expiry = DateTime.Now.AddMinutes(configuration.GetValue<int>("JwtConfig:Expiry"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(claims: claims, expires: expiry, signingCredentials: creds, notBefore: DateTime.Now);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64]; // 512 bits
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public string HashPassword(string password, byte[] salt, int iterations = 10000, int hashByteSize = 32)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(hashByteSize);
            return Convert.ToBase64String(hash);
        }
    }
}
