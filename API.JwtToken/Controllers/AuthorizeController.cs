using API.JwtToken.Modal;
using API.JwtToken.Repos;
using API.JwtToken.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.JwtToken.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    //implement Authorize  
    public class AuthorizeController : ControllerBase
    {
        private readonly dbContext dbContext;
        private readonly IRefreshHandler refreshHandler;
        private readonly JwtSettings jwtSettings; 

        public AuthorizeController(dbContext dbContext, IOptions<JwtSettings> options, IRefreshHandler refreshHandler)
        {
            this.dbContext = dbContext;
            this.refreshHandler = refreshHandler;
            this.jwtSettings = options.Value;
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCredential userCredential)
        {
            var user = await this.dbContext.TblUsers.FirstOrDefaultAsync(
                                    x => x.Username == userCredential.userName && x.Password == userCredential.password
                                );
            if (user != null)
            {
               //Generate token here
                var tokenHandler=new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };

                //after config tokenDescription then we will generate our token out
                var token = tokenHandler.CreateToken(tokenDescription);
                var finalToken=tokenHandler.WriteToken(token);

                return Ok(new TokenResponse() { Token=finalToken, RefreshToken= await this.refreshHandler.GenerateToken(userCredential.userName) });

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenResponse tokenResponse)
        {
            var _refreshToken = await this.dbContext.TblRefeshtokens.FirstOrDefaultAsync(
                                    t=>t.Refreshtoken == tokenResponse.RefreshToken
                                );
            if (_refreshToken != null)
            {
                //Generate token here
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(tokenResponse.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);

                var _token = securityToken as JwtSecurityToken;

                if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)) 
                {
                    string username = principal.Identity?.Name;//get userName from principal
                    var _existData = await this.dbContext.TblRefeshtokens.FirstOrDefaultAsync(item => item.Userid == username
                                                                        && item.Refreshtoken == tokenResponse.RefreshToken);

                    if (_existData != null)
                    {
                        var _newtoken = new JwtSecurityToken(
                                claims:principal.Claims.ToArray(),
                                expires:DateTime.Now.AddSeconds(30),
                                signingCredentials:new SigningCredentials(
                                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.securityKey)), SecurityAlgorithms.HmacSha256
                                    )
                            );
                        var _finalToken = tokenHandler.WriteToken(_newtoken);
                        return Ok(new TokenResponse() { Token = _finalToken, RefreshToken = await this.refreshHandler.GenerateToken(username) });

                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }



                //var tokenDescription = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(new Claim[]
                //    {
                //            new Claim(ClaimTypes.Name, user.Username),
                //            new Claim(ClaimTypes.Role, user.Role)
                //    }),
                //    Expires = DateTime.UtcNow.AddSeconds(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                //};

                ////after config tokenDescription then we will generate our token out
                //var token = tokenHandler.CreateToken(tokenDescription);
                //var finalToken = tokenHandler.WriteToken(token);

                //return Ok(new TokenResponse() { Token = finalToken, RefreshToken = await this.refreshHandler.GenerateToken(userCredential.userName) });

            }
            else
            {
                return Unauthorized();
            }

        }
    }
}
