using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        string GenerateJwt(LoginDto dto);
    }
    public class AccountService : IAccountService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly RestaurantDBContext _context;

        private readonly AuthenticationSettings _authenticationSettings;
        public AccountService(RestaurantDBContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {

            _passwordHasher = passwordHasher;
            _context = context;
            _authenticationSettings = authenticationSettings;
        }

        public string GenerateJwt(LoginDto dto)
        {
            var user = _context.User.Include(ur => ur.Role).
                 FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var verResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verResult == PasswordVerificationResult.Failed)
                throw new BadRequestException("Invalid username or password");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                  new Claim(ClaimTypes.Name, $"{user.FirstName}{user.LastName}"),
                   new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                    new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
                     
            };

            if(!string.IsNullOrWhiteSpace(user.Nationality))
            {
                claims.Add(
                new Claim("Nationality", user.Nationality));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryDate = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expiryDate,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

        }

        public void RegisterUser(RegisterUserDto dto)
        {

            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId
               

            };
            var hashPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashPassword;
            _context.User.Add(newUser);
            _context.SaveChanges();

        }

    }
}
