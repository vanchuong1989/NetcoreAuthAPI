using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.JwtToken.Repos.Models;

[Table("tlb_product")]
public partial class TlbProduct
{
    [Key]
    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("price", TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }
}
