using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Waho.WahoModels
{
    public partial class WahoContext : DbContext
    {
        public WahoContext()
        {
        }

        public WahoContext(DbContextOptions<WahoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillDetail> BillDetails { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Oder> Oders { get; set; }
        public virtual DbSet<OderDetail> OderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<WahoInformation> WahoInformations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Waho"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.BillId)
                    .ValueGeneratedNever()
                    .HasColumnName("billID");

                entity.Property(e => e.CustomerId).HasColumnName("customerID");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("userName");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bill_Customers");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bill_Employees");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.BillId).HasColumnName("billID");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.ProductId).HasColumnName("productID");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Bill)
                    .WithMany()
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetails_Bill");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetails_Products");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("categoryID");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("categoryName");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .HasColumnName("description")
                    .IsFixedLength();

                entity.Property(e => e.HaveDate).HasColumnName("haveDate");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId)
                    .ValueGeneratedNever()
                    .HasColumnName("customerID");

                entity.Property(e => e.Adress)
                    .HasMaxLength(50)
                    .HasColumnName("adress");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(50)
                    .HasColumnName("customerName");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.Dob).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.Property(e => e.TaxCode)
                    .HasMaxLength(50)
                    .HasColumnName("taxCode");

                entity.Property(e => e.TypeOfCustomer).HasColumnName("typeOfCustomer");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.UserName);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("userName");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .HasColumnName("address");

                entity.Property(e => e.Dob).HasColumnType("date");

                entity.Property(e => e.EmployeeName)
                    .HasMaxLength(50)
                    .HasColumnName("employeeName");

                entity.Property(e => e.HireDate)
                    .HasColumnType("date")
                    .HasColumnName("hireDate");

                entity.Property(e => e.Note)
                    .HasMaxLength(50)
                    .HasColumnName("note");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .HasColumnName("region");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.WahoId).HasColumnName("wahoID");

                entity.HasOne(d => d.Waho)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.WahoId)
                    .HasConstraintName("FK_WahoInformation_Employees");
            });

            modelBuilder.Entity<Oder>(entity =>
            {
                entity.Property(e => e.OderId)
                    .ValueGeneratedNever()
                    .HasColumnName("oderID");

                entity.Property(e => e.Cod)
                    .HasMaxLength(10)
                    .HasColumnName("cod")
                    .IsFixedLength();

                entity.Property(e => e.CustomerId).HasColumnName("customerID");

                entity.Property(e => e.OderState)
                    .HasMaxLength(50)
                    .HasColumnName("oderState");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .HasColumnName("region");

                entity.Property(e => e.ShipperId).HasColumnName("shipperID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("userName");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Oders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oders_Customers");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Oders)
                    .HasForeignKey(d => d.ShipperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oders_Shippers");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Oders)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oders_Employees");
            });

            modelBuilder.Entity<OderDetail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.OderId).HasColumnName("oderID");

                entity.Property(e => e.ProductId).HasColumnName("productID");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Oder)
                    .WithMany()
                    .HasForeignKey(d => d.OderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OderDetails_Oders");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OderDetails_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .ValueGeneratedNever()
                    .HasColumnName("productID");

                entity.Property(e => e.DateOfManufacture)
                    .HasColumnType("date")
                    .HasColumnName("dateOfManufacture");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .HasColumnName("description");

                entity.Property(e => e.Expiry)
                    .HasColumnType("date")
                    .HasColumnName("expiry");

                entity.Property(e => e.HaveDate).HasColumnName("haveDate");

                entity.Property(e => e.ImportPrice).HasColumnName("importPrice");

                entity.Property(e => e.InventoryLevelMax).HasColumnName("inventoryLevelMax");

                entity.Property(e => e.InventoryLevelMin).HasColumnName("inventoryLevelMin");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .HasColumnName("location");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("productName");

                entity.Property(e => e.SubCategoryId).HasColumnName("subCategoryID");

                entity.Property(e => e.SupplierId).HasColumnName("supplierID");

                entity.Property(e => e.Trademark)
                    .HasMaxLength(50)
                    .HasColumnName("trademark");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .HasColumnName("unit");

                entity.Property(e => e.UnitInStock).HasColumnName("unitInStock");

                entity.Property(e => e.UnitPrice).HasColumnName("unitPrice");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.SubCategory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SubCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_SubCategories");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_Suppliers");
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.Property(e => e.ShipperId)
                    .ValueGeneratedNever()
                    .HasColumnName("shipperID");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.Property(e => e.ShipperName)
                    .HasMaxLength(50)
                    .HasColumnName("shipperName");
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.Property(e => e.SubCategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("subCategoryID");

                entity.Property(e => e.CategoryId).HasColumnName("categoryID");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.SubCategoryName)
                    .HasMaxLength(150)
                    .HasColumnName("subCategoryName");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.SubCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubCategories_Categories");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.SupplierId)
                    .ValueGeneratedNever()
                    .HasColumnName("supplierID");

                entity.Property(e => e.Address)
                    .HasMaxLength(150)
                    .HasColumnName("address");

                entity.Property(e => e.Branch)
                    .HasMaxLength(50)
                    .HasColumnName("branch");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(150)
                    .HasColumnName("companyName");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.Phone)
                    .HasMaxLength(24)
                    .HasColumnName("phone");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .HasColumnName("region");

                entity.Property(e => e.TaxCode)
                    .HasMaxLength(50)
                    .HasColumnName("taxCode");
            });

            modelBuilder.Entity<WahoInformation>(entity =>
            {
                entity.HasKey(e => e.WahoId);

                entity.ToTable("WahoInformation");

                entity.Property(e => e.WahoId)
                    .ValueGeneratedNever()
                    .HasColumnName("wahoID");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .HasColumnName("address");

                entity.Property(e => e.CategoryId).HasColumnName("categoryID");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.Property(e => e.WahoName)
                    .HasMaxLength(50)
                    .HasColumnName("wahoName");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.WahoInformations)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WahoInformation_Categories");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
