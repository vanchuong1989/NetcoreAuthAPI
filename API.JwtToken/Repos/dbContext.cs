using System;
using System.Collections.Generic;
using API.JwtToken.Repos.Models;
using Microsoft.EntityFrameworkCore;

namespace API.JwtToken.Repos;

public partial class dbContext : DbContext
{
    public dbContext()
    {
    }

    public dbContext(DbContextOptions<dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    public virtual DbSet<TblProductImage> TblProductImages { get; set; }

    public virtual DbSet<TblRefeshtoken> TblRefeshtokens { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TlbProduct> TlbProducts { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.\\;Database=test_db;User Id=sa;Password=chuong@1989;TrustServerCertificate=true;");

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<TblProductImage>(entity =>
    //    {
    //        entity.Property(e => e.Id).ValueGeneratedNever();
    //    });

    //    OnModelCreatingPartial(modelBuilder);
    //}

    //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
