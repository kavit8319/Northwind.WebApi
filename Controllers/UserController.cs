using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Northwind.Interface.Shared;
using Northwind.WebApi.AutoMaper;
using Northwind.WebApi.Config;
using Northwind.WebApi.Model.Data;
using Northwind.WebApi.Model.Models;
using Northwind.WebApi.ModelsExt.Errors;
using Northwind.WebApi.ModelsExt.ModelAuthentic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : CustomControllerBase
    {
        private readonly JWTSettings _jwtsettings;
        private readonly IOptions<MailConfiguration> configMail;
        private readonly IOptions<ComfirmEmail> configAddress;
        public UsersController(NorthwindContext context, IOptions<JWTSettings> jwtsettings, IOptions<MailConfiguration> _conf, IOptions<ComfirmEmail> _confirmAddres, IMapper _mapper) : base(context, _mapper)
        {
            _jwtsettings = jwtsettings.Value;
            configMail = _conf;
            configAddress = _confirmAddres;
        }

        // GET: api/Users
        [HttpGet("GetUsersPaging")]

        public async Task<Result<IEnumerable<UserReturn>>> GetUsersPaging(string? firstName, string? lastName, int? roleId, string? email, string? collSortname, string? ascDesc, int pageNum, int pageSize)
        {
            try
            {
                var result = (await context.Procedures.SelectUserPagingAsync(firstName, lastName, email, roleId, collSortname, ascDesc, pageNum, pageSize));
                return Result<IEnumerable<UserReturn>>.Success(mapper.Map<List<SelectUserPagingResult>, List<UserReturn>>(result));
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpGet()]
        [Route("GetUsersRoles")]
        public async Task<Result<List<Role>>> GetUserRoles()
        {
            try
            {
                var result = (await context.Roles.ToListAsync());
                return Result<List<Role>>.Success(result);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<Result<UserReturn>> GetUser(int id)
        {
            try
            {
                var user = (await context.Procedures.UserByIdAsync(id)).FirstOrDefault();
                if (user == null)
                    return Result<UserReturn>.Fail("User not found");
                return Result<UserReturn>.Success(mapper.Map<UserReturn>(user));
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
        // GET: api/Users/5
        [HttpGet("GetUserByEmail")]

        public async Task<Result<UserReturn>> GetUser(string? email)
        {
            try
            {
                var user = await context.AdmUsers.Where(user => user.EmailAddress == email).FirstOrDefaultAsync();

                if (user == null)
                    return Result<UserReturn>.Fail("User not found");
                return Result<UserReturn>.Success(mapper.Map<UserReturn>(user));
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }

        }
        [AllowAnonymous]
        [HttpGet("EmailExist")]
        public async Task<Result<bool>> EmailExist(string email)
        {
            try
            {
                var outParameter = new OutputParameter<int>();
                var res = await context.Procedures.UserEmailExistAsync(email, outParameter);
                if (outParameter.Value == 1)
                    return Result<bool>.Fail("EmailExist");
                return Result<bool>.Success("EmailNotExist");
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<Result<UserReturn>> Login(string email, string password)
        {
            try
            {
                var user = (await context.Procedures.LogoneAsync(email)).FirstOrDefault();
                if (user == null)
                    return Result<UserReturn>.Fail("UserNotFound");
                if (!VerifyPassword(password, user.Salt, user.Password))
                    return Result<UserReturn>.Fail("PasswordIncorect");
                if (!user.EmailVerified)
                    return Result<UserReturn>.Fail("EmailNotConfirm");

                UserWithToken userWithToken = null!;
                RefreshToken refreshToken = GenerateRefreshToken();
                await context.Procedures.InsRefreshTokenAsync(user.UserID, refreshToken.Token, refreshToken.ExpiryDate);

                userWithToken = new UserWithToken(user);
                userWithToken.RefreshToken = refreshToken.Token;
                //sign your token here here..
                userWithToken.AccessToken = GenerateAccessToken(user.UserID);
                return Result<UserReturn>.Success(mapper.Map<LogoneResult, UserWithToken, UserReturn>(user, userWithToken));
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }

        }

        [HttpPost("RefreshToken")]
        public async Task<Result<UserReturn>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            try
            {
                if (refreshRequest.AccessToken != null)
                {
                    var user = await GetUserFromAccessToken(refreshRequest.AccessToken);
                    if (user == null)
                        return Result<UserReturn>.Fail("UserNotFound");
                    if (ValidateRefreshToken(user, refreshRequest.RefreshToken).Result)
                        return Result<UserReturn>.Fail("RefreshToketNotCorect");
                    var userWithToken = new UserWithToken(mapper.Map<LogoneResult>(user));
                    userWithToken.AccessToken = GenerateAccessToken(user.UserID);
                    return Result<UserReturn>.Success(mapper.Map<UserReturn>(userWithToken));
                }
                return Result<UserReturn>.Fail("AccessTokenIsNull");
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }
        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        [HttpPut("EditAdminUser")]
      
        public async Task<Result<UserReturn>> EditUser(UserReturn user)
        {
            try
            {
                var userForUpd = mapper.Map<AdmUser>(user);
                context.Entry(userForUpd).State = EntityState.Modified;
                context.Entry(userForUpd).Property(el => el.Password).IsModified = false;
                context.Entry(userForUpd).Property(el => el.Salt).IsModified = false;
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!UserExists(userForUpd.UserId))
                    {
                        return Result<UserReturn>.Fail("UserNotFound");
                    }
                    return Result<UserReturn>.Fail(ex.Message);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
            return Result<UserReturn>.Success(user);
        }

       
        [HttpPost]
        [Route("ChangePersonalInfo")]
        public async Task<Result<bool>> ChangePersonalUserInfo(UserReturn user)
        {
            try
            {
                if (user != null)
                {
                    var userById = await context.Procedures.UserByIdAsync(user.UserID);
                    if (userById == null) { return Result<bool>.Fail("UserNotFound"); }
                    var hashPassword = EncryptPassword(user.Password);
                    user.Password = hashPassword.Hash;
                    user.Salt = hashPassword.Salt;
                    await context.Procedures.UserChengePersonalInfoAsync(user.UserID, user.FirstName, user.LastName, user.EmailAddress, user.Password, user.Salt);
                    
                }
                else
                return Result<bool>.Fail("UserIsNull");
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
            return Result<bool>.Success("UserUpdated");
        }
       

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [AllowAnonymous]
        [HttpPost("AddAdminUser")]
        public async Task<Result<UserReturn>> AddUser(UserReturn user)
        {
            try
            {
                if (user.Password != null)
                {
                    var hashPassword = EncryptPassword(user.Password);
                    user.Password = hashPassword.Hash;
                    user.Salt = hashPassword.Salt;
                }
                var userAdd = mapper.Map<AdmUser>(user);

                context.AdmUsers.Add(userAdd);
                await context.SaveChangesAsync();
                user.UserID = userAdd.UserId;
                if (!user.EmailVerified)
                {
                    //var code = SymmetricEncryptionDecryptionManager.Encrypt(Guid.NewGuid().ToString("N"), configAddress.Value.CryptoKey);
                    var code = SymmetricEncryptionDecryptionManager.Encrypt(userAdd.Activationcode.ToString("N"), configAddress.Value.CryptoKey);
                    var body = string.Format("Activate your account by <a href='{0}/login?code={1}'>clicking here</a>.", configAddress.Value.AddressComfirm, code);
                    string message = $"To: {user.EmailAddress}\r\nSubject: Comfirm email on site Northwind1551983\r\nContent-Type: text/html;charset=utf-8\r\n\r\n {body}";
                    await SendEmailApi(user.EmailAddress, message);

                }
                return Result<UserReturn>.Success(mapper.Map<UserReturn>(user));
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
            // CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        private async Task<bool> SendEmailApi(string email, string message)
        {
            string[] Scopes = { GmailService.Scope.GmailSend };
            UserCredential credential;
            using (var stream = new FileStream("client_secret_googleGmailApi.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token_Send";
                var host = Request.Host;
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                                             GoogleClientSecrets.FromStream(stream).Secrets,
                                              Scopes,
                                              "user",
                                              CancellationToken.None,
                                              new FileDataStore(credPath, true));
            }
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Northwind1551983"
            });
            var newMsg = new Message();
            newMsg.Raw = Base64UrlEncode(message.ToString());
            Message response = service.Users.Messages.Send(newMsg, "me").Execute();
            if (response != null) return true;

            return false;
        }

        [AllowAnonymous]
        [HttpPost("RecoveryPassword")]
        public async Task<Result<string>> RecoveryPassword(string email)
        {
            try
            {
                var outParameter = new OutputParameter<int>();
                var res = await context.Procedures.UserEmailExistAsync(email, outParameter);
                if (outParameter.Value == 1)
                {
                    var userByEmail = await context.Procedures.UserByEmailAsync(email, outParameter);
                    var user = userByEmail.FirstOrDefault();
                    if (user.EmailVerified)
                    {
                        var password = CreatePassword(7);
                        var hashPassword = EncryptPassword(password);
                        user.Password = hashPassword.Hash;
                        user.Salt = hashPassword.Salt;
                        var userForUpdate = mapper.Map<AdmUser>(user);
                        context.Entry(userForUpdate).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                        var body = string.Format("New password for site Northwind1551983: {0}", password);
                        string message = $"To: {email}\r\nSubject: Password recovery on site Northwind1551983\r\nContent-Type: text/html;charset=utf-8\r\n\r\n {body}";

                        var resultSend = await SendEmailApi(email, message);
                        if (resultSend == null) return Result<string>.Fail("SendEmailError");
                    }
                    else
                        return Result<string>.Fail("UserNotActivated");
                    return Result<string>.Success("PasswordChangeed");
                }
                else
                    return Result<string>.Fail("EmailNotExist");
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }

        private async Task SendEmail(string emailTo, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(configMail.Value.From);
                mail.To.Add(emailTo);
                mail.Subject = configMail.Value.UserName;
                mail.Body = body;
                mail.IsBodyHtml = true;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(configMail.Value.Host, configMail.Value.Port))
                {
                    smtp.Credentials = new NetworkCredential(configMail.Value.From, configMail.Value.Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ActivateUserByCode")]
        public async Task<Result<bool>> ActivateUserEmailByCode(string code)
        {
            try
            {
                //var res = Decompress(code);
                //var result=Convert.ToBase64String(res);
                var guid = SymmetricEncryptionDecryptionManager.Decrypt(code, configAddress.Value.CryptoKey);
                if (Guid.TryParse(guid, out Guid t))
                {
                    var user = context.AdmUsers.FirstOrDefault(el => el.Activationcode == t);
                    if (user == null)
                        return Result<bool>.Fail($"User not found={guid} {code}");
                    if (user.EmailVerified)
                        return Result<bool>.Success(true, $"Email has been activated: {user.DateActivation.Value:MM/dd/yyyy HH:mm}");
                    context.Entry(user).State = EntityState.Modified;
                    context.Entry(user).Property(el => el.EmailVerified).CurrentValue = true;
                    context.Entry(user).Property(el => el.DateActivation).CurrentValue = DateTime.Now;
                    await context.SaveChangesAsync();
                    return Result<bool>.Success(true, "Ok");
                }
                else
                    return Result<bool>.Fail("Bad code");

            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }

        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<Result<UserReturn>> DeleteUser(int id)
        {
            AdmUser user;
            try
            {
                user = (await context.AdmUsers.FindAsync(id))!;
                if (user == null)
                    return Result<UserReturn>.Fail("UserNotFound");

                context.AdmUsers.Remove(user);
                await context.SaveChangesAsync();
                return Result<UserReturn>.Success(mapper.Map<UserReturn>(user));
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        // POST: api/Users
        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public async Task<Result<UserReturn>> RegisterUser([FromBody] UserReturn user)
        {
            try
            {
                if (user == null)
                    return Result<UserReturn>.Fail("UserIsNull");
                if (user.Password != null)
                {
                    var hashPassword = EncryptPassword(user.Password);
                    user.Password = hashPassword.Hash;
                    user.Salt = hashPassword.Salt;
                }
                else
                    return Result<UserReturn>.Fail("PasswordIsNull");

                var fromClientUser = mapper.Map<AdmUser>(user);
                context.AdmUsers.Add(fromClientUser);
                await context.SaveChangesAsync();

                ////load role for registered user
                //var userNew = (await context.Procedures.UserByIdAsync(fromClientUser.UserId)).FirstOrDefault();
                //UserWithToken userWithToken = null!;

                //if (user != null)
                //{
                //    RefreshToken refreshToken = GenerateRefreshToken();
                //    await context.Procedures.InsRefreshTokenAsync(userNew!.UserID, refreshToken.Token, refreshToken.ExpiryDate);
                //    userWithToken = new UserWithToken(mapper.Map<LogoneResult>(user));
                //    userWithToken.RefreshToken = refreshToken.Token;
                //}
                ////sign your token here here..
                //userWithToken.AccessToken = GenerateAccessToken(userNew!.UserID);
                fromClientUser.Password = null;
                fromClientUser.Salt = null;
                return Result<UserReturn>.Success(mapper.Map<UserReturn>(fromClientUser));
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }
        private bool UserExists(int id)
        {
            return context.AdmUsers.Any(e => e.UserId == id);
        }

        [HttpPost("GetUserByAccessToken")]
        public async Task<Result<UserReturn>> GetUserByAccessToken([FromBody] string accessToken)
        {
            try
            {
                var user = await GetUserFromAccessToken(accessToken);
                if (user == null)
                    return Result<UserReturn>.Fail("UserIsNull");
                return Result<UserReturn>.Success(mapper.Map<UserReturn>(user));
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }
        private async Task<UserReturn> GetUserFromAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey!);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = (securityToken as JwtSecurityToken)!;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = principle.FindFirst(ClaimTypes.Name)?.Value;
                    var result = (await context.Procedures.UserByIdAsync(Convert.ToInt32(userId))).First();
                    return mapper.Map<UserReturn>(result);
                }
            }
            catch (Exception)
            {
                return new UserReturn();
            }

            return new UserReturn();
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

        private string GenerateAccessToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (_jwtsettings.SecretKey != null)
            {
                var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, Convert.ToString(userId))
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            return string.Empty;
        }
        private async Task<bool> ValidateRefreshToken(UserReturn user, string? refreshToken)
        {
            var refreshTokenUser = (await context.Procedures.SelectRefreshTokenByTokenAsync(refreshToken)).FirstOrDefault();
            if (refreshTokenUser != null && refreshTokenUser.UserID == user.UserID
                && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
                return true;
            return false;
        }

        private HashSalt EncryptPassword(string password)
        {
            byte[] salt = new byte[128 / 8]; // Generate a 128-bit salt using a secure PRNG
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return new HashSalt { Hash = encryptedPassw, Salt = salt };
        }

        private bool VerifyPassword(string enteredPassword, byte[] salt, string storedPassword)
        {
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return encryptedPassw == storedPassword;
        }
    }

    public class HashSalt
    {
        public string? Hash { get; set; }
        public byte[]? Salt { get; set; }
    }

}
