using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class BenefitsDbContext : DbContext
    {
        public BenefitsDbContext(DbContextOptions<BenefitsDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Dependent> Dependents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // An employee may have an unlimited number of children
            modelBuilder.Entity<Employee>()
                .HasMany(x => x.Dependents)
                .WithOne(x => x.Employee)
                .HasForeignKey(x => x.EmployeeId);

            // create unique index on EmployeeId and Relationship
            modelBuilder.Entity<Dependent>()
                .HasIndex(d => new { d.EmployeeId, d.Relationship })
                .IsUnique();
        }

        public override int SaveChanges()
        {
            ValidateDependents();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ValidateDependents();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        // An employee may only have 1 spouse or domestic partner (not both)
        // since Im implementing a inMemory database I will enforce this rule here
        // if this was a real database I would enforce this rule in the database using
        // a unique constraint something like this:

        // CREATE UNIQUE INDEX IX_Dependent_EmployeeId_Relationship
        // ON Dependents (EmployeeId, Relationship)
        // WHERE [Relationship] IN (1, 2); -- 1 = Spouse, 2 = DomesticPartner

        private void ValidateDependents()
        {
            var errorMessage = "An employee can only have one spouse or domestic partner.";
            var dependents = ChangeTracker.Entries<Dependent>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);

            // possible scenario when trying to add an spouse and a domestic partner at once 
            var hasInvalidSpouses = dependents
                .Where(e => e.Relationship == Relationship.Spouse || e.Relationship == Relationship.DomesticPartner)
                .GroupBy(e => e.EmployeeId)
                .Any(g => g.Count() > 1);

            if (hasInvalidSpouses)
            {
                throw new InvalidOperationException(errorMessage);
            }

            foreach (var dependent in dependents)
            {
                if (dependent.Relationship == Relationship.Spouse || dependent.Relationship == Relationship.DomesticPartner)
                {
                    var existingDependents = Dependents
                        .Where((d => d.EmployeeId == dependent.EmployeeId &&
                                    (d.Relationship == Relationship.Spouse || d.Relationship == Relationship.DomesticPartner) &&
                                    d.Id != dependent.Id));

                    if (existingDependents.Count() > 0)
                    {
                        throw new InvalidOperationException(errorMessage);
                    }
                }
            }
        }
    }
}
