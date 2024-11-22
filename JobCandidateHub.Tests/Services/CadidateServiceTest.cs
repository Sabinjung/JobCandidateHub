using FakeItEasy;
using FluentAssertions;
using JobCandidateHub.DTOs;
using JobCandidateHub.Models.Entities;
using JobCandidateHub.Respositories;
using JobCandidateHub.Services;

namespace JobCandidateHub.Tests.Services
{
    public class CandidateServiceTests
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICacheService _cacheService;
        private readonly CandidateService _candidateService;

        public CandidateServiceTests()
        {
            _candidateRepository = A.Fake<ICandidateRepository>();
            _cacheService = A.Fake<ICacheService>();
            _candidateService = new CandidateService(_candidateRepository, _cacheService);
        }

        [Fact]
        public async Task UpsertCandidateAsync_WhenNoExistingCandidate_CreatesNewCandidate()
        {
            // Arrange
            var candidateDto = new CandidateDto("sabin@example.com", "Sabin", "Chhetri", "Test Comment", "98765434");
            var newCandidate = new Candidate
            {
                Email = candidateDto.Email,
                FirstName = candidateDto.FirstName,
                LastName = candidateDto.LastName,
                Comment = candidateDto.Comment
            };


            A.CallTo(() => _cacheService.GetFromCache<Candidate>(A<string>.Ignored))
                .Returns(null);


            A.CallTo(() => _candidateRepository.GetByEmailAsync(candidateDto.Email))
                .Returns((Candidate?)null);
            A.CallTo(() => _candidateRepository.CreateAsync(A<Candidate>.Ignored))
                .Returns(newCandidate);

            // Act
            var (createdCandidate, isUpdateAction) = await _candidateService.UpsertCandidateAsync(candidateDto);
            Console.WriteLine(createdCandidate);

            // Assert
            createdCandidate.Should().BeEquivalentTo(newCandidate);
            isUpdateAction.Should().BeFalse();
            A.CallTo(() => _candidateRepository.CreateAsync(A<Candidate>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _cacheService.SetInCache(A<string>.Ignored, A<Candidate>.Ignored, A<TimeSpan>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpsertCandidateAsync_WhenExistingCandidate_UpdatesCandidate()
        {
            // Arrange
            var candidateDto = new CandidateDto("sabin@example.com", "Sabin", "Chhetri", "Test Comment");
            var existingCandidate = new Candidate
            {
                Email = candidateDto.Email,
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                Comment = "Old Comment"
            };

            var updatedCandidate = new Candidate
            {
                Email = candidateDto.Email,
                FirstName = candidateDto.FirstName,
                LastName = candidateDto.LastName,
                Comment = candidateDto.Comment
            };

            A.CallTo(() => _candidateRepository.GetByEmailAsync(candidateDto.Email))
                .Returns(existingCandidate); // Existing candidate
            A.CallTo(() => _candidateRepository.UpdateAsync(A<Candidate>.Ignored))
                .Returns(updatedCandidate);

            // Act
            var (result, isUpdateAction) = await _candidateService.UpsertCandidateAsync(candidateDto);

            // Assert
            result.Should().BeEquivalentTo(updatedCandidate);
            isUpdateAction.Should().BeTrue();
            A.CallTo(() => _candidateRepository.UpdateAsync(A<Candidate>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _cacheService.RemoveFromCache(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetByEmailAsync_WhenCacheMiss_RetrievesFromRepository()
        {
            // Arrange
            var candidate = new Candidate
            {
                Email = "sabin@example.com",
                FirstName = "Sabin",
                LastName = "Chhetri",
                Comment = "Test Comment"
            };

            A.CallTo(() => _cacheService.GetFromCache<Candidate>(A<string>.Ignored))
                .Returns(null); 
            A.CallTo(() => _candidateRepository.GetByEmailAsync("sabin@example.com"))
                .Returns(candidate);

            // Act
            var result = await _candidateService.GetByEmailAsync("sabin@example.com");

            // Assert
            result.Should().BeEquivalentTo(candidate);
            A.CallTo(() => _cacheService.GetFromCache<Candidate>(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _candidateRepository.GetByEmailAsync("sabin@example.com")).MustHaveHappenedOnceExactly();
            A.CallTo(() => _cacheService.SetInCache(A<string>.Ignored, candidate, A<TimeSpan>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetByEmailAsync_WhenCacheHit_ReturnsFromCache()
        {
            // Arrange
            var candidate = new Candidate
            {
                Email = "sabin@example.com",
                FirstName = "Sabin",
                LastName = "Chhetri",
                Comment = "Test Comment"
            };

            A.CallTo(() => _cacheService.GetFromCache<Candidate>(A<string>.Ignored))
                .Returns(candidate);

            // Act
            var result = await _candidateService.GetByEmailAsync("sabin@example.com");

            // Assert
            result.Should().BeEquivalentTo(candidate);
            A.CallTo(() => _cacheService.GetFromCache<Candidate>(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _candidateRepository.GetByEmailAsync(A<string>.Ignored)).MustNotHaveHappened();
        }
    }
}
