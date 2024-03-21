using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Data
{
    public class Authrepository : IAuthrepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public Authrepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var responce = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
            if (user is null)
            {
                responce.Success = false;
                responce.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
            {
                responce.Success = false;
                responce.Message = "Wrong password";
            }
            else
            {
                responce.Data = CreateToken(user);//user.Id.ToString();
            }
            return responce;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var responce = new ServiceResponse<int>();
            if (await UserExist(user.Username))
            {
                responce.Success = false;
                responce.Message = "User already exist.";
                return responce;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            responce.Data = user.Id;
            return responce;
        }

        public async Task<bool> UserExist(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512() )
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password,byte[]  passwordHash,byte[] passwodSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwodSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var appSettingsToken = _configuration.GetSection("Appsettings:Token").Value;
            if (appSettingsToken is null)
            {
                throw new Exception("Appsettings Token is null!");
            }

            SymmetricSecurityKey key =  new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));
            
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}