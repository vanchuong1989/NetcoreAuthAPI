using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.JwtToken.Modal
{
    public class CustomerModal
    {
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string Code { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? Email { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Phone { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Creditlimit { get; set; }

        public bool? IsActive { get; set; }

        public int? Taxcode { get; set; }

        //add this new property to test AutoMapper
        public string? StatusName { get; set; }
    }
}
