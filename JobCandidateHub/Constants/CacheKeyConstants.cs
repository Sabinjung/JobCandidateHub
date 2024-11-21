namespace JobCandidateHub.Constants
{
    public static class CacheKeyConstants
    {
        public static string GetCandidateByEmail(string email) => $"Candidate_{email}";
    }
}
