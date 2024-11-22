using FluentValidation;
using JobCandidateHub.DTOs;

namespace JobCandidateHub.Validators
{
    public class CandidateUpsertValidator : AbstractValidator<CandidateDto>
    {
        public CandidateUpsertValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty()
               .EmailAddress()
               .MaximumLength(100);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Comment)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.GitHubUrl)
                .Must(url => url == null || url.Contains("github.com"));

            RuleFor(x => x.LinkedInUrl)
                .Must(url => url == null || url.Contains("linkedin.com"));

            RuleFor(x => x.CallEndTime)
                .GreaterThan(x => x.CallStartTime)
                .When(x => x.CallStartTime.HasValue && x.CallEndTime.HasValue);
        }
    }
}
