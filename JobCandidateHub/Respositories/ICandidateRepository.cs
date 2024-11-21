using JobCandidateHub.Models.Entities;

namespace JobCandidateHub.Respositories
{
    public interface ICandidateRepository
    {
       Task<Candidate?> GetByEmailAsync(string email);
       Task<Candidate> CreateAsync(Candidate candidate);
       Task<Candidate> UpdateAsync(Candidate candidate);
    }
}
