using API.JwtToken.Repos;
using API.JwtToken.Service;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace API.JwtToken.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly dbContext dbContext;

        public RefreshHandler(dbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomNumber = new byte[32];
            using (var randomNumberGeneratetor = RandomNumberGenerator.Create())
            {
                randomNumberGeneratetor.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);
                var existToken = this.dbContext.TblRefeshtokens.FirstOrDefaultAsync(item => item.Userid == username).Result;
                if (existToken != null)
                {
                    existToken.Refreshtoken = refreshToken;
                }
                else
                {
                    await this.dbContext.TblRefeshtokens.AddAsync(new Repos.Models.TblRefeshtoken
                    {
                        Userid = username,
                        Tokenid = new Random().Next().ToString(),
                        Refreshtoken = refreshToken
                    });
                }
                await this.dbContext.SaveChangesAsync();
                return refreshToken;
            }
        }
    }
}
