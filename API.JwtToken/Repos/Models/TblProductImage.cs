using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.JwtToken.Repos.Models;

[Table("tbl_product_image")]
public partial class TblProductImage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ProductCode { get; set; }

    [Column("product_img", TypeName = "image")]
    public byte[]? ProductImg { get; set; }
}
