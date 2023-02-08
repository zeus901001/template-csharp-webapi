using System.ComponentModel.DataAnnotations;

namespace csharp_webapi.Models.Auth
{
    public class RefreshTokenModel
    {
        [DataType(DataType.Text)]
        public string? RefreshToken { get; set; }
    }
}
