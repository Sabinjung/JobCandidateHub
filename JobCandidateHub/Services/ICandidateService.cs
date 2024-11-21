using JobCandidateHub.DTOs;
using JobCandidateHub.Models.Entities;

namespace JobCandidateHub.Services
{
    public interface ICandidateService
    {
        Task<(Candidate candidate, bool isUpdateAction)> UpsertCandidateAsync(CandidateDto candidateDto);
    }
}
