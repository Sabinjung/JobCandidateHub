using JobCandidateHub.Data;
using JobCandidateHub.Models.Entities;
using JobCandidateHub.Respositories;
using Microsoft.EntityFrameworkCore;

namespace JobCandidateHub.Tests.Repositories
{
    public class CandidateRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly CandidateRepository _repository;

        public CandidateRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestCandidateDb")
                .Options;

            _context = new ApplicationDbContext(_options);
            _repository = new CandidateRepository(_context);

            // Clear database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetByEmailAsync_ExistingEmail_ReturnsCandidate()
        {
            // Arrange
            var expectedCandidate = new Candidate
            {
                Email = "sabin@example.com",
                FirstName = "sabin",
                LastName = "chhetri",
                Comment = "testing"
            };
            await _context.Candidates.AddAsync(expectedCandidate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("sabin@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCandidate.Email, result.Email);
            Assert.Equal(expectedCandidate.FirstName, result.FirstName);
            Assert.Equal(expectedCandidate.LastName, result.LastName);
        }

        [Fact]
        public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByEmailAsync("nonexisting@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ValidCandidate_CreatesAndReturnsCandidate()
        {
            // Arrange
            var newCandidate = new Candidate
            {
                Email = "bipin@example.com",
                FirstName = "bipin",
                LastName = "Chhetri",
                Comment = "testing"
            };

            // Act
            var result = await _repository.CreateAsync(newCandidate);

            // Assert
            Assert.NotNull(result);
            var savedCandidate = await _context.Candidates.FirstOrDefaultAsync(x => x.Email == "bipin@example.com");
            Assert.NotNull(savedCandidate);
            Assert.Equal(newCandidate.Email, savedCandidate.Email);
            Assert.Equal(newCandidate.FirstName, savedCandidate.FirstName);
            Assert.Equal(newCandidate.LastName, savedCandidate.LastName);
        }

        [Fact]
        public async Task UpdateAsync_ExistingCandidate_UpdatesAndReturnsCandidate()
        {
            // Arrange
            var existingCandidate = new Candidate
            {
                Email = "sabin@example.com",
                FirstName = "sabin",
                LastName = "chhetri",
                Comment = "testing"
            };
            await _context.Candidates.AddAsync(existingCandidate);
            await _context.SaveChangesAsync();

            // Update candidate
            existingCandidate.FirstName = "Updated";
            existingCandidate.LastName = "NewName";

            // Act
            var result = await _repository.UpdateAsync(existingCandidate);

            // Assert
            Assert.NotNull(result);
            var updatedCandidate = await _context.Candidates.FirstOrDefaultAsync(x => x.Email == "sabin@example.com");
            Assert.NotNull(updatedCandidate);
            Assert.Equal("Updated", updatedCandidate.FirstName);
            Assert.Equal("NewName", updatedCandidate.LastName);
        }
    }
}