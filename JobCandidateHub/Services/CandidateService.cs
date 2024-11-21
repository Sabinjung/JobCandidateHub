using JobCandidateHub.DTOs;
using JobCandidateHub.Models.Entities;
using JobCandidateHub.Respositories;

namespace JobCandidateHub.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task<(Candidate candidate, bool isUpdateAction)> UpsertCandidateAsync(CandidateDto candidateDto)
        {
            bool isUpdateAction = false;
            var existingCandidate = await _candidateRepository.GetByEmailAsync(candidateDto.Email);

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
                isUpdateAction = true;
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
            }

            return (candidate, isUpdateAction );
        }
    }
}
