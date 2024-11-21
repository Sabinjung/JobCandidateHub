using System.ComponentModel.DataAnnotations;
namespace JobCandidateHub.Models.Entities
{
    public class Candidate
    {
        [Key]
        public required string Email { get; set; }
        public required string FirstName {get; set;}
        public required string LastName {get; set;}
        public string? PhoneNumber {get; set;}
        public required string Comment {get; set;}
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public DateTime? CallStartTime { get; set; }
        public DateTime? CallEndTime { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
