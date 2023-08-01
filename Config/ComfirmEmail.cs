namespace Northwind.WebApi.Config
{
    public class ComfirmEmail
    {
        public string AddressComfirm { get; set; }

        //var res= TripleDESCryptoServiceProvider.Create();
        //res.GenerateIV();
        //res.GenerateKey();
        //var strKey = Convert.ToBase64String(res.Key);
        public string CryptoKey { get; set; }
    }
}
