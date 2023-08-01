using Northwind.WebApi.Model.Models;


namespace Northwind.WebApi.ModelsExt.ModelAuthentic
{

    public class UserWithToken : AdmUser
    {

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public UserWithToken(LogoneResult user)
        {
            this.UserId = user.UserID;
            this.EmailAddress = user.EmailAddress;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Role =new Role{Name = user.RoleName, RoleId = user.RoleID!.Value};
        }
    }
}
