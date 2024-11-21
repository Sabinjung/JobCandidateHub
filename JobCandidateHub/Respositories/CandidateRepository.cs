using JobCandidateHub.Data;
using JobCandidateHub.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobCandidateHub.Respositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _context;
        public CandidateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Candidate?> GetByEmailAsync(string email)
        {
            return await _context.Candidates.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Candidate> CreateAsync(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<Candidate> UpdateAsync(Candidate candidate)
        {
            _context.Candidates.Update(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }
    }
}
