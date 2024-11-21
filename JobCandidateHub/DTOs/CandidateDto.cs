namespace JobCandidateHub.DTOs
{
    public record CandidateDto
    (
        string Email,
        string FirstName,
        string LastName,
        string Comment,
        string? PhoneNumber = null,
        string? LinkedInUrl = null,
        string? GitHubUrl = null,
        DateTime? CallStartTime = null,
        DateTime? CallEndTime = null
    );
}
