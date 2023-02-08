using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace csharp_webapi.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [JsonIgnore]
        public string? Password { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? LastName { get; set; }

        public byte Role { get; set; }

        public byte Permission { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
    }
}
