namespace Northwind.WebApi.ModelsExt.ModelAuthentic
{
    public class UserReturn
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int UserID { get; set; }
        public string? EmailAddress { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public int? TotalRows { get; set; }
        public byte[]? Salt { get; set; }
        public Guid Activationcode { get; set; }

        public bool EmailVerified { get; set; }

        public bool EditPassword { get; set; }


    }
}
