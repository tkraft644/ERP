using System.Linq.Expressions;
using ErpSystem.Application.Common;
using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Common;
using ErpSystem.Domain.Modules.Finance;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Dictionaries;
using ErpSystem.Domain.Modules.System.Documents;
using ErpSystem.Domain.Modules.System.Identity;
using ErpSystem.Domain.Modules.Transport;
using ErpSystem.Domain.Modules.Warehouse;
using ErpSystem.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpSystem.Infrastructure.Persistence;

public sealed class ErpSystemDbContext : DbContext
{
    private readonly ICurrentUserAccessor currentUserAccessor;

    public ErpSystemDbContext(DbContextOptions<ErpSystemDbContext> options, ICurrentUserAccessor currentUserAccessor)
        : base(options)
    {
        this.currentUserAccessor = currentUserAccessor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<DocumentHistoryEntry> DocumentHistoryEntries => Set<DocumentHistoryEntry>();
    public DbSet<DocumentStatus> DocumentStatuses => Set<DocumentStatus>();
    public DbSet<DocumentNumberSequence> DocumentNumberSequences => Set<DocumentNumberSequence>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<ContractorAddress> ContractorAddresses => Set<ContractorAddress>();
    public DbSet<ContractorContact> ContractorContacts => Set<ContractorContact>();
    public DbSet<ContractorBankAccount> ContractorBankAccounts => Set<ContractorBankAccount>();
    public DbSet<ContractorNote> ContractorNotes => Set<ContractorNote>();
    public DbSet<CostDocument> CostDocuments => Set<CostDocument>();
    public DbSet<CostPosition> CostPositions => Set<CostPosition>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoicePosition> InvoicePositions => Set<InvoicePosition>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CurrencyRate> CurrencyRates => Set<CurrencyRate>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmploymentContract> EmploymentContracts => Set<EmploymentContract>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();
    public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
    public DbSet<DriverProfile> DriverProfiles => Set<DriverProfile>();
    public DbSet<Carrier> Carriers => Set<Carrier>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Trailer> Trailers => Set<Trailer>();
    public DbSet<TransportOrder> TransportOrders => Set<TransportOrder>();
    public DbSet<TransportOrderRoute> TransportOrderRoutes => Set<TransportOrderRoute>();
    public DbSet<TransportStop> TransportStops => Set<TransportStop>();
    public DbSet<TransportDocument> TransportDocuments => Set<TransportDocument>();
    public DbSet<TransportCost> TransportCosts => Set<TransportCost>();
    public DbSet<TransportStatusHistory> TransportStatusHistoryEntries => Set<TransportStatusHistory>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<WarehouseDocument> WarehouseDocuments => Set<WarehouseDocument>();
    public DbSet<WarehouseDocumentPosition> WarehouseDocumentPositions => Set<WarehouseDocumentPosition>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder.Entity<User>());
        ConfigureRole(modelBuilder.Entity<Role>());
        ConfigurePermission(modelBuilder.Entity<Permission>());
        ConfigureUserRole(modelBuilder.Entity<UserRole>());
        ConfigureRolePermission(modelBuilder.Entity<RolePermission>());
        ConfigureAuditLog(modelBuilder.Entity<AuditLog>());
        ConfigureDictionaryItem(modelBuilder.Entity<DictionaryItem>());
        ConfigureAttachment(modelBuilder.Entity<Attachment>());
        ConfigureDocumentHistory(modelBuilder.Entity<DocumentHistoryEntry>());
        ConfigureDocumentStatus(modelBuilder.Entity<DocumentStatus>());
        ConfigureDocumentNumberSequence(modelBuilder.Entity<DocumentNumberSequence>());
        ConfigureContractor(modelBuilder.Entity<Contractor>());
        ConfigureContractorAddress(modelBuilder.Entity<ContractorAddress>());
        ConfigureContractorContact(modelBuilder.Entity<ContractorContact>());
        ConfigureContractorBankAccount(modelBuilder.Entity<ContractorBankAccount>());
        ConfigureContractorNote(modelBuilder.Entity<ContractorNote>());
        ConfigureCostDocument(modelBuilder.Entity<CostDocument>());
        ConfigureCostPosition(modelBuilder.Entity<CostPosition>());
        ConfigureInvoice(modelBuilder.Entity<Invoice>());
        ConfigureInvoicePosition(modelBuilder.Entity<InvoicePosition>());
        ConfigurePayment(modelBuilder.Entity<Payment>());
        ConfigureCurrencyRate(modelBuilder.Entity<CurrencyRate>());
        ConfigureSettlement(modelBuilder.Entity<Settlement>());
        ConfigureDepartment(modelBuilder.Entity<Department>());
        ConfigurePosition(modelBuilder.Entity<Position>());
        ConfigureEmployee(modelBuilder.Entity<Employee>());
        ConfigureEmploymentContract(modelBuilder.Entity<EmploymentContract>());
        ConfigureLeaveType(modelBuilder.Entity<LeaveType>());
        ConfigureLeaveRequest(modelBuilder.Entity<LeaveRequest>());
        ConfigureWorkSchedule(modelBuilder.Entity<WorkSchedule>());
        ConfigureEmployeeDocument(modelBuilder.Entity<EmployeeDocument>());
        ConfigureDriverProfile(modelBuilder.Entity<DriverProfile>());
        ConfigureCarrier(modelBuilder.Entity<Carrier>());
        ConfigureVehicle(modelBuilder.Entity<Vehicle>());
        ConfigureTrailer(modelBuilder.Entity<Trailer>());
        ConfigureTransportOrder(modelBuilder.Entity<TransportOrder>());
        ConfigureTransportOrderRoute(modelBuilder.Entity<TransportOrderRoute>());
        ConfigureTransportStop(modelBuilder.Entity<TransportStop>());
        ConfigureTransportDocument(modelBuilder.Entity<TransportDocument>());
        ConfigureTransportCost(modelBuilder.Entity<TransportCost>());
        ConfigureTransportStatusHistory(modelBuilder.Entity<TransportStatusHistory>());
        ConfigureWarehouse(modelBuilder.Entity<Warehouse>());
        ConfigureWarehouseLocation(modelBuilder.Entity<WarehouseLocation>());
        ConfigureProductCategory(modelBuilder.Entity<ProductCategory>());
        ConfigureUnitOfMeasure(modelBuilder.Entity<UnitOfMeasure>());
        ConfigureProduct(modelBuilder.Entity<Product>());
        ConfigureStockItem(modelBuilder.Entity<StockItem>());
        ConfigureWarehouseDocument(modelBuilder.Entity<WarehouseDocument>());
        ConfigureWarehouseDocumentPosition(modelBuilder.Entity<WarehouseDocumentPosition>());
        ConfigureStockMovement(modelBuilder.Entity<StockMovement>());

        Seed(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var utcNow = DateTime.UtcNow;
        var userId = currentUserAccessor.UserId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedByUserId = userId;
                    entry.Entity.ModifiedAt = null;
                    entry.Entity.ModifiedByUserId = null;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = utcNow;
                    entry.Entity.ModifiedByUserId = userId;
                    entry.Property(item => item.CreatedAt).IsModified = false;
                    entry.Property(item => item.CreatedByUserId).IsModified = false;
                    entry.Property(item => item.IsDeleted).IsModified = false;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.ModifiedAt = utcNow;
                    entry.Entity.ModifiedByUserId = userId;
                    break;
            }
        }
    }

    private static void ConfigureUser(EntityTypeBuilder<User> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.UserName).HasMaxLength(100).IsRequired();
        entity.Property(item => item.Email).HasMaxLength(256).IsRequired();
        entity.Property(item => item.DisplayName).HasMaxLength(200).IsRequired();
        entity.Property(item => item.PasswordHash).HasMaxLength(512).IsRequired();
        entity.Property(item => item.PasswordSalt).HasMaxLength(256).IsRequired();
        entity.Property(item => item.MustChangePassword).IsRequired();
        entity.HasIndex(item => item.UserName).IsUnique();
        entity.HasIndex(item => item.Email).IsUnique();
    }

    private static void ConfigureRole(EntityTypeBuilder<Role> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(500).IsRequired();
        entity.HasIndex(item => item.Code).IsUnique();
    }

    private static void ConfigurePermission(EntityTypeBuilder<Permission> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Name).HasMaxLength(200).IsRequired();
        entity.Property(item => item.ModuleKey).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Resource).HasMaxLength(100).IsRequired();
        entity.Property(item => item.Action).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(500).IsRequired();
        entity.HasIndex(item => item.Name).IsUnique();
    }

    private static void ConfigureUserRole(EntityTypeBuilder<UserRole> entity)
    {
        ConfigureAuditable(entity);
        entity.HasIndex(item => new { item.UserId, item.RoleId }).IsUnique();
        entity.HasOne<User>().WithMany().HasForeignKey(item => item.UserId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<Role>().WithMany().HasForeignKey(item => item.RoleId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureRolePermission(EntityTypeBuilder<RolePermission> entity)
    {
        ConfigureAuditable(entity);
        entity.HasIndex(item => new { item.RoleId, item.PermissionId }).IsUnique();
        entity.HasOne<Role>().WithMany().HasForeignKey(item => item.RoleId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<Permission>().WithMany().HasForeignKey(item => item.PermissionId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureAuditLog(EntityTypeBuilder<AuditLog> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.EntityName).HasMaxLength(120).IsRequired();
        entity.Property(item => item.ActionName).HasMaxLength(120).IsRequired();
        entity.Property(item => item.OldValues).HasColumnType("nvarchar(max)").IsRequired();
        entity.Property(item => item.NewValues).HasColumnType("nvarchar(max)").IsRequired();
        entity.Property(item => item.Summary).HasMaxLength(1000).IsRequired();
    }

    private static void ConfigureDictionaryItem(EntityTypeBuilder<DictionaryItem> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.DictionaryName).HasMaxLength(100).IsRequired();
        entity.Property(item => item.Code).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.Property(item => item.Value).HasMaxLength(250).IsRequired();
        entity.HasIndex(item => new { item.DictionaryName, item.Code }).IsUnique();
    }

    private static void ConfigureAttachment(EntityTypeBuilder<Attachment> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.OwnerEntityName).HasMaxLength(120).IsRequired();
        entity.Property(item => item.FileName).HasMaxLength(255).IsRequired();
        entity.Property(item => item.ContentType).HasMaxLength(120).IsRequired();
        entity.Property(item => item.StoragePath).HasMaxLength(500).IsRequired();
    }

    private static void ConfigureDocumentHistory(EntityTypeBuilder<DocumentHistoryEntry> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.DocumentType).HasMaxLength(120).IsRequired();
        entity.Property(item => item.EntryType).HasMaxLength(120).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(1000).IsRequired();
        entity.Property(item => item.StatusCode).HasMaxLength(60);
    }

    private static void ConfigureDocumentStatus(EntityTypeBuilder<DocumentStatus> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.ModuleKey).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Code).HasMaxLength(60).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.HasIndex(item => new { item.ModuleKey, item.Code }).IsUnique();
    }

    private static void ConfigureDocumentNumberSequence(EntityTypeBuilder<DocumentNumberSequence> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Key).HasMaxLength(120).IsRequired();
        entity.Property(item => item.Prefix).HasMaxLength(30).IsRequired();
        entity.HasIndex(item => item.Key).IsUnique();
    }

    private static void ConfigureContractor(EntityTypeBuilder<Contractor> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(60).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(200).IsRequired();
        entity.Property(item => item.ShortName).HasMaxLength(120);
        entity.Property(item => item.TaxId).HasMaxLength(40);
        entity.Property(item => item.Types).IsRequired();
        entity.HasIndex(item => item.Code).IsUnique();
        entity.HasMany(item => item.Addresses)
            .WithOne(item => item.Contractor)
            .HasForeignKey(item => item.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Contacts)
            .WithOne(item => item.Contractor)
            .HasForeignKey(item => item.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.BankAccounts)
            .WithOne(item => item.Contractor)
            .HasForeignKey(item => item.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Notes)
            .WithOne(item => item.Contractor)
            .HasForeignKey(item => item.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureContractorAddress(EntityTypeBuilder<ContractorAddress> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Label).HasMaxLength(150).IsRequired();
        entity.Property(item => item.CountryCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.PostalCode).HasMaxLength(20).IsRequired();
        entity.Property(item => item.City).HasMaxLength(120).IsRequired();
        entity.Property(item => item.Street).HasMaxLength(150).IsRequired();
        entity.Property(item => item.BuildingNumber).HasMaxLength(20).IsRequired();
        entity.Property(item => item.ApartmentNumber).HasMaxLength(20);
    }

    private static void ConfigureContractorContact(EntityTypeBuilder<ContractorContact> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.FullName).HasMaxLength(150).IsRequired();
        entity.Property(item => item.Position).HasMaxLength(120);
        entity.Property(item => item.Email).HasMaxLength(200);
        entity.Property(item => item.PhoneNumber).HasMaxLength(50);
    }

    private static void ConfigureContractorBankAccount(EntityTypeBuilder<ContractorBankAccount> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.BankName).HasMaxLength(150).IsRequired();
        entity.Property(item => item.AccountNumber).HasMaxLength(80).IsRequired();
        entity.Property(item => item.CurrencyCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.Swift).HasMaxLength(20);
    }

    private static void ConfigureContractorNote(EntityTypeBuilder<ContractorNote> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Title).HasMaxLength(200).IsRequired();
        entity.Property(item => item.Content).HasMaxLength(4000).IsRequired();
    }

    private static void ConfigureCostDocument(EntityTypeBuilder<CostDocument> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Number).HasMaxLength(50).IsRequired();
        entity.Property(item => item.CurrencyCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.ExchangeRate).HasColumnType("decimal(18,6)");
        entity.Property(item => item.ExternalNumber).HasMaxLength(120);
        entity.Property(item => item.Description).HasMaxLength(1000);
        entity.HasIndex(item => item.Number).IsUnique();
        entity.HasOne(item => item.Contractor).WithMany().HasForeignKey(item => item.ContractorId).OnDelete(DeleteBehavior.Restrict);
        entity.HasMany(item => item.Positions)
            .WithOne(item => item.CostDocument)
            .HasForeignKey(item => item.CostDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Settlements)
            .WithOne(item => item.CostDocument)
            .HasForeignKey(item => item.CostDocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureCostPosition(EntityTypeBuilder<CostPosition> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.CostCategory).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(250).IsRequired();
        entity.Property(item => item.Quantity).HasColumnType("decimal(18,4)");
        entity.Property(item => item.UnitPrice).HasColumnType("decimal(18,4)");
        entity.Property(item => item.TaxRate).HasColumnType("decimal(8,2)");
        entity.Property(item => item.Notes).HasMaxLength(1000);
        entity.HasIndex(item => new { item.CostDocumentId, item.LineNumber }).IsUnique();
        entity.HasOne(item => item.TransportOrder).WithMany().HasForeignKey(item => item.TransportOrderId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Vehicle).WithMany().HasForeignKey(item => item.VehicleId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Employee).WithMany().HasForeignKey(item => item.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Warehouse).WithMany().HasForeignKey(item => item.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Contractor).WithMany().HasForeignKey(item => item.ContractorId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Department).WithMany().HasForeignKey(item => item.DepartmentId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureInvoice(EntityTypeBuilder<Invoice> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Number).HasMaxLength(50).IsRequired();
        entity.Property(item => item.CurrencyCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.ExchangeRate).HasColumnType("decimal(18,6)");
        entity.Property(item => item.ExternalNumber).HasMaxLength(120);
        entity.Property(item => item.Description).HasMaxLength(1000);
        entity.HasIndex(item => item.Number).IsUnique();
        entity.HasOne(item => item.Contractor).WithMany().HasForeignKey(item => item.ContractorId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.TransportOrder).WithMany().HasForeignKey(item => item.TransportOrderId).OnDelete(DeleteBehavior.Restrict);
        entity.HasMany(item => item.Positions)
            .WithOne(item => item.Invoice)
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Settlements)
            .WithOne(item => item.Invoice)
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureInvoicePosition(EntityTypeBuilder<InvoicePosition> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.ItemName).HasMaxLength(160).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(250).IsRequired();
        entity.Property(item => item.Quantity).HasColumnType("decimal(18,4)");
        entity.Property(item => item.UnitPrice).HasColumnType("decimal(18,4)");
        entity.Property(item => item.TaxRate).HasColumnType("decimal(8,2)");
        entity.Property(item => item.Notes).HasMaxLength(1000);
        entity.HasIndex(item => new { item.InvoiceId, item.LineNumber }).IsUnique();
    }

    private static void ConfigurePayment(EntityTypeBuilder<Payment> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.CurrencyCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.ExchangeRate).HasColumnType("decimal(18,6)");
        entity.Property(item => item.Amount).HasColumnType("decimal(18,2)");
        entity.Property(item => item.Method).HasMaxLength(80).IsRequired();
        entity.Property(item => item.ReferenceNumber).HasMaxLength(120);
        entity.Property(item => item.Notes).HasMaxLength(1000);
        entity.HasOne(item => item.Contractor).WithMany().HasForeignKey(item => item.ContractorId).OnDelete(DeleteBehavior.Restrict);
        entity.HasMany(item => item.Settlements)
            .WithOne(item => item.Payment)
            .HasForeignKey(item => item.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureCurrencyRate(EntityTypeBuilder<CurrencyRate> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.BaseCurrencyCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.QuoteCurrencyCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.Rate).HasColumnType("decimal(18,6)");
        entity.Property(item => item.Source).HasMaxLength(80).IsRequired();
        entity.HasIndex(item => new { item.RateDate, item.BaseCurrencyCode, item.QuoteCurrencyCode }).IsUnique();
    }

    private static void ConfigureSettlement(EntityTypeBuilder<Settlement> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Amount).HasColumnType("decimal(18,2)");
        entity.Property(item => item.Notes).HasMaxLength(1000);
        entity.HasOne(item => item.Payment).WithMany(item => item.Settlements).HasForeignKey(item => item.PaymentId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(item => item.Invoice).WithMany(item => item.Settlements).HasForeignKey(item => item.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.CostDocument).WithMany(item => item.Settlements).HasForeignKey(item => item.CostDocumentId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureDepartment(EntityTypeBuilder<Department> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(40).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(500);
        entity.HasIndex(item => item.Code).IsUnique();
    }

    private static void ConfigurePosition(EntityTypeBuilder<Position> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(40).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(500);
        entity.HasIndex(item => item.Code).IsUnique();
    }

    private static void ConfigureEmployee(EntityTypeBuilder<Employee> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.EmployeeNumber).HasMaxLength(40).IsRequired();
        entity.Property(item => item.FirstName).HasMaxLength(100).IsRequired();
        entity.Property(item => item.LastName).HasMaxLength(100).IsRequired();
        entity.Property(item => item.Email).HasMaxLength(200).IsRequired();
        entity.Property(item => item.PhoneNumber).HasMaxLength(50);
        entity.Property(item => item.PersonalId).HasMaxLength(30);
        entity.HasIndex(item => item.EmployeeNumber).IsUnique();
        entity.HasIndex(item => item.Email).IsUnique();
        entity.HasMany(item => item.EmploymentContracts)
            .WithOne(item => item.Employee)
            .HasForeignKey(item => item.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.WorkSchedules)
            .WithOne(item => item.Employee)
            .HasForeignKey(item => item.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Documents)
            .WithOne(item => item.Employee)
            .HasForeignKey(item => item.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.LeaveRequests)
            .WithOne(item => item.Employee)
            .HasForeignKey(item => item.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(item => item.DriverProfile)
            .WithOne(item => item.Employee)
            .HasForeignKey<DriverProfile>(item => item.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureEmploymentContract(EntityTypeBuilder<EmploymentContract> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.ContractNumber).HasMaxLength(50).IsRequired();
        entity.Property(item => item.ContractType).HasMaxLength(50).IsRequired();
        entity.Property(item => item.EmploymentRate).HasColumnType("decimal(5,2)");
        entity.Property(item => item.MonthlySalary).HasColumnType("decimal(18,2)");
        entity.Property(item => item.Notes).HasMaxLength(1000);
        entity.HasIndex(item => item.ContractNumber).IsUnique();
        entity.HasOne(item => item.Department).WithMany().HasForeignKey(item => item.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Position).WithMany().HasForeignKey(item => item.PositionId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureLeaveType(EntityTypeBuilder<LeaveType> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(40).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(120).IsRequired();
        entity.HasIndex(item => item.Code).IsUnique();
    }

    private static void ConfigureLeaveRequest(EntityTypeBuilder<LeaveRequest> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.DayCount).HasColumnType("decimal(6,2)");
        entity.Property(item => item.Reason).HasMaxLength(1000);
        entity.Property(item => item.DecisionNote).HasMaxLength(1000);
        entity.HasOne(item => item.LeaveType).WithMany().HasForeignKey(item => item.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<User>().WithMany().HasForeignKey(item => item.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureWorkSchedule(EntityTypeBuilder<WorkSchedule> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Name).HasMaxLength(120).IsRequired();
        entity.Property(item => item.WeeklyHours).HasColumnType("decimal(5,2)");
        entity.Property(item => item.Notes).HasMaxLength(1000);
    }

    private static void ConfigureEmployeeDocument(EntityTypeBuilder<EmployeeDocument> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.DocumentType).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Title).HasMaxLength(200).IsRequired();
        entity.Property(item => item.DocumentNumber).HasMaxLength(80);
        entity.Property(item => item.Notes).HasMaxLength(1000);
    }

    private static void ConfigureDriverProfile(EntityTypeBuilder<DriverProfile> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.LicenseNumber).HasMaxLength(50).IsRequired();
        entity.Property(item => item.LicenseCategories).HasMaxLength(30);
        entity.Property(item => item.DriverCardNumber).HasMaxLength(50);
        entity.HasIndex(item => item.EmployeeId).IsUnique();
    }

    private static void ConfigureCarrier(EntityTypeBuilder<Carrier> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(40).IsRequired();
        entity.HasIndex(item => item.Code).IsUnique();
        entity.HasIndex(item => item.ContractorId).IsUnique();
        entity.HasOne(item => item.Contractor).WithMany().HasForeignKey(item => item.ContractorId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureVehicle(EntityTypeBuilder<Vehicle> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.RegistrationNumber).HasMaxLength(30).IsRequired();
        entity.Property(item => item.Brand).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Model).HasMaxLength(80).IsRequired();
        entity.Property(item => item.PayloadTons).HasColumnType("decimal(18,3)");
        entity.HasIndex(item => item.RegistrationNumber).IsUnique();
        entity.HasOne(item => item.Carrier).WithMany().HasForeignKey(item => item.CarrierId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureTrailer(EntityTypeBuilder<Trailer> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.RegistrationNumber).HasMaxLength(30).IsRequired();
        entity.Property(item => item.TrailerType).HasMaxLength(80).IsRequired();
        entity.Property(item => item.PayloadTons).HasColumnType("decimal(18,3)");
        entity.HasIndex(item => item.RegistrationNumber).IsUnique();
        entity.HasOne(item => item.Carrier).WithMany().HasForeignKey(item => item.CarrierId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureTransportOrder(EntityTypeBuilder<TransportOrder> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Number).HasMaxLength(50).IsRequired();
        entity.Property(item => item.LoadingCountryCode).HasMaxLength(3);
        entity.Property(item => item.UnloadingCountryCode).HasMaxLength(3);
        entity.Property(item => item.Incoterms).HasMaxLength(20);
        entity.Property(item => item.CurrencyCode).HasMaxLength(3);
        entity.Property(item => item.ExchangeRate).HasColumnType("decimal(18,6)");
        entity.Property(item => item.DomesticRegion).HasMaxLength(100);
        entity.Property(item => item.DomesticTransportKind).HasMaxLength(100);
        entity.Property(item => item.ExternalReference).HasMaxLength(120);
        entity.Property(item => item.Notes).HasMaxLength(2000);
        entity.HasIndex(item => item.Number).IsUnique();
        entity.HasOne(item => item.Carrier).WithMany().HasForeignKey(item => item.CarrierId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Vehicle).WithMany().HasForeignKey(item => item.VehicleId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Trailer).WithMany().HasForeignKey(item => item.TrailerId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Driver).WithMany().HasForeignKey(item => item.DriverId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Route)
            .WithOne(item => item.TransportOrder)
            .HasForeignKey<TransportOrderRoute>(item => item.TransportOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Stops)
            .WithOne(item => item.TransportOrder)
            .HasForeignKey(item => item.TransportOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Documents)
            .WithOne(item => item.TransportOrder)
            .HasForeignKey(item => item.TransportOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.Costs)
            .WithOne(item => item.TransportOrder)
            .HasForeignKey(item => item.TransportOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(item => item.StatusHistory)
            .WithOne(item => item.TransportOrder)
            .HasForeignKey(item => item.TransportOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureTransportOrderRoute(EntityTypeBuilder<TransportOrderRoute> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.PlannedDistanceKm).HasColumnType("decimal(18,2)");
        entity.Property(item => item.PlannedRevenue).HasColumnType("decimal(18,2)");
        entity.Property(item => item.RouteSummary).HasMaxLength(500);
        entity.HasIndex(item => item.TransportOrderId).IsUnique();
    }

    private static void ConfigureTransportStop(EntityTypeBuilder<TransportStop> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Name).HasMaxLength(200).IsRequired();
        entity.Property(item => item.CountryCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.City).HasMaxLength(120).IsRequired();
        entity.Property(item => item.AddressLine).HasMaxLength(250).IsRequired();
        entity.Property(item => item.Notes).HasMaxLength(1000);
        entity.HasIndex(item => new { item.TransportOrderId, item.Sequence }).IsUnique();
        entity.HasOne(item => item.Contractor).WithMany().HasForeignKey(item => item.ContractorId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureTransportDocument(EntityTypeBuilder<TransportDocument> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.DocumentNumber).HasMaxLength(80);
        entity.Property(item => item.FileName).HasMaxLength(255);
    }

    private static void ConfigureTransportCost(EntityTypeBuilder<TransportCost> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.CostType).HasMaxLength(80).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(200).IsRequired();
        entity.Property(item => item.CurrencyCode).HasMaxLength(3).IsRequired();
        entity.Property(item => item.Amount).HasColumnType("decimal(18,2)");
        entity.Property(item => item.ExchangeRate).HasColumnType("decimal(18,6)");
    }

    private static void ConfigureTransportStatusHistory(EntityTypeBuilder<TransportStatusHistory> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Note).HasMaxLength(1000);
        entity.HasOne<User>().WithMany().HasForeignKey(item => item.ChangedByUserId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureWarehouse(EntityTypeBuilder<Warehouse> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(40).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(500);
        entity.HasIndex(item => item.Code).IsUnique();
    }

    private static void ConfigureWarehouseLocation(EntityTypeBuilder<WarehouseLocation> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(40).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.HasIndex(item => new { item.WarehouseId, item.Code }).IsUnique();
        entity.HasOne(item => item.Warehouse).WithMany().HasForeignKey(item => item.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureProductCategory(EntityTypeBuilder<ProductCategory> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(40).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
        entity.Property(item => item.Description).HasMaxLength(500);
        entity.HasIndex(item => item.Code).IsUnique();
    }

    private static void ConfigureUnitOfMeasure(EntityTypeBuilder<UnitOfMeasure> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(20).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(100).IsRequired();
        entity.Property(item => item.Symbol).HasMaxLength(20).IsRequired();
        entity.HasIndex(item => item.Code).IsUnique();
    }

    private static void ConfigureProduct(EntityTypeBuilder<Product> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Code).HasMaxLength(60).IsRequired();
        entity.Property(item => item.Name).HasMaxLength(200).IsRequired();
        entity.Property(item => item.Sku).HasMaxLength(80);
        entity.Property(item => item.MinimumStockLevel).HasColumnType("decimal(18,4)");
        entity.HasIndex(item => item.Code).IsUnique();
        entity.HasOne(item => item.ProductCategory).WithMany().HasForeignKey(item => item.ProductCategoryId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.UnitOfMeasure).WithMany().HasForeignKey(item => item.UnitOfMeasureId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureStockItem(EntityTypeBuilder<StockItem> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.QuantityOnHand).HasColumnType("decimal(18,4)");
        entity.HasIndex(item => new { item.WarehouseId, item.WarehouseLocationId, item.ProductId }).IsUnique();
        entity.HasOne(item => item.Warehouse).WithMany().HasForeignKey(item => item.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.WarehouseLocation).WithMany().HasForeignKey(item => item.WarehouseLocationId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Product).WithMany().HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureWarehouseDocument(EntityTypeBuilder<WarehouseDocument> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Number).HasMaxLength(50).IsRequired();
        entity.Property(item => item.ExternalReference).HasMaxLength(120);
        entity.Property(item => item.Notes).HasMaxLength(2000);
        entity.HasIndex(item => item.Number).IsUnique();
        entity.HasOne(item => item.Contractor).WithMany().HasForeignKey(item => item.ContractorId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.SourceWarehouse).WithMany().HasForeignKey(item => item.SourceWarehouseId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.SourceLocation).WithMany().HasForeignKey(item => item.SourceLocationId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.TargetWarehouse).WithMany().HasForeignKey(item => item.TargetWarehouseId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.TargetLocation).WithMany().HasForeignKey(item => item.TargetLocationId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureWarehouseDocumentPosition(EntityTypeBuilder<WarehouseDocumentPosition> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.Quantity).HasColumnType("decimal(18,4)");
        entity.Property(item => item.UnitPrice).HasColumnType("decimal(18,4)");
        entity.Property(item => item.Notes).HasMaxLength(1000);
        entity.HasOne(item => item.WarehouseDocument).WithMany(item => item.Positions).HasForeignKey(item => item.WarehouseDocumentId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(item => item.Product).WithMany().HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.SourceLocation).WithMany().HasForeignKey(item => item.SourceLocationId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.TargetLocation).WithMany().HasForeignKey(item => item.TargetLocationId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureStockMovement(EntityTypeBuilder<StockMovement> entity)
    {
        ConfigureAuditable(entity);
        entity.Property(item => item.QuantityDelta).HasColumnType("decimal(18,4)");
        entity.Property(item => item.UnitPrice).HasColumnType("decimal(18,4)");
        entity.HasOne(item => item.WarehouseDocument).WithMany(item => item.StockMovements).HasForeignKey(item => item.WarehouseDocumentId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(item => item.WarehouseDocumentPosition).WithMany().HasForeignKey(item => item.WarehouseDocumentPositionId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Product).WithMany().HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.Warehouse).WithMany().HasForeignKey(item => item.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(item => item.WarehouseLocation).WithMany().HasForeignKey(item => item.WarehouseLocationId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureAuditable<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : AuditableEntity
    {
        entity.HasKey(item => item.Id);
        entity.Property(item => item.CreatedAt).IsRequired();
        entity.Property(item => item.RowVersion).IsRowVersion();
        entity.HasQueryFilter(CreateSoftDeleteFilter<TEntity>());
    }

    private static Expression<Func<TEntity, bool>> CreateSoftDeleteFilter<TEntity>() where TEntity : AuditableEntity
        => item => !item.IsDeleted;

    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(FoundationSeedData.Roles);
        modelBuilder.Entity<Permission>().HasData(FoundationSeedData.Permissions);
        modelBuilder.Entity<RolePermission>().HasData(FoundationSeedData.RolePermissions);
        modelBuilder.Entity<DictionaryItem>().HasData(FoundationSeedData.DictionaryItems);
        modelBuilder.Entity<DocumentStatus>().HasData(FoundationSeedData.DocumentStatuses);
        modelBuilder.Entity<DocumentNumberSequence>().HasData(FoundationSeedData.DocumentNumberSequences);
        modelBuilder.Entity<Warehouse>().HasData(WarehouseSeedData.Warehouses);
        modelBuilder.Entity<WarehouseLocation>().HasData(WarehouseSeedData.Locations);
        modelBuilder.Entity<ProductCategory>().HasData(WarehouseSeedData.Categories);
        modelBuilder.Entity<UnitOfMeasure>().HasData(WarehouseSeedData.Units);
    }
}
