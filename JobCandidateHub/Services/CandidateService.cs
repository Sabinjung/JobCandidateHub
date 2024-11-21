using JobCandidateHub.Constants;
using JobCandidateHub.DTOs;
using JobCandidateHub.Models.Entities;
using JobCandidateHub.Respositories;

namespace JobCandidateHub.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICacheService _cacheService;

        public CandidateService(ICandidateRepository candidateRepository, ICacheService cacheService)
        {
            _candidateRepository = candidateRepository;
            _cacheService = cacheService;
        }

        public async Task<Candidate?> GetByEmailAsync(string email)
        {
            var cacheKey = CacheKeyConstants.GetCandidateByEmail(email);

            var cachedCandidate = _cacheService.GetFromCache<Candidate>(cacheKey);
            if (cachedCandidate != null)
            {
                return cachedCandidate;
            }

            var candidate = await _candidateRepository.GetByEmailAsync(email);
            if (candidate != null)
            {
                _cacheService.SetInCache(cacheKey, candidate, TimeSpan.FromMinutes(30));
            }

            return candidate;
        }

        public async Task<(Candidate candidate, bool isUpdateAction)> UpsertCandidateAsync(CandidateDto candidateDto)
        {
            var existingCandidate = await GetByEmailAsync(candidateDto.Email);

            Candidate candidate;
            if (existingCandidate != null)
            {
                existingCandidate.FirstName = candidateDto.FirstName;
                existingCandidate.LastName = candidateDto.LastName;
                existingCandidate.PhoneNumber = candidateDto.PhoneNumber;
                existingCandidate.LinkedInUrl = candidateDto.LinkedInUrl;
                existingCandidate.GitHubUrl = candidateDto.GitHubUrl;
                existingCandidate.Comment = candidateDto.Comment;
                existingCandidate.CallStartTime = candidateDto.CallStartTime;
                existingCandidate.CallEndTime = candidateDto.CallEndTime;
                existingCandidate.UpdatedAt = DateTime.UtcNow;

                candidate = await _candidateRepository.UpdateAsync(existingCandidate);

                var cacheKey = CacheKeyConstants.GetCandidateByEmail(candidate.Email);
                _cacheService.RemoveFromCache(cacheKey);
                _cacheService.SetInCache(cacheKey, candidate);
            }
            else
            {
                candidate = new Candidate
                {
                    Email = candidateDto.Email,
                    FirstName = candidateDto.FirstName,
                    LastName = candidateDto.LastName,
                    PhoneNumber = candidateDto.PhoneNumber,
                    LinkedInUrl = candidateDto.LinkedInUrl,
                    GitHubUrl = candidateDto.GitHubUrl,
                    Comment = candidateDto.Comment,
                    CallStartTime = candidateDto.CallStartTime,
                    CallEndTime = candidateDto.CallEndTime,
                };

                candidate = await _candidateRepository.CreateAsync(candidate);

                var cacheKey = CacheKeyConstants.GetCandidateByEmail(candidate.Email);
                _cacheService.SetInCache(cacheKey, candidate, TimeSpan.FromMinutes(30));
            }

            return (candidate, existingCandidate != null);
        }

    }
}
