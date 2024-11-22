using FluentValidation;
using JobCandidateHub.DTOs;
using JobCandidateHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobCandidateHub.Controllers
{
    [Route("api/candidate")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;
        private readonly IValidator<CandidateDto> _validator;
        public CandidateController(ICandidateService candidateService, IValidator<CandidateDto> validator)
        {
            _candidateService = candidateService;
            _validator = validator;
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertCandidate([FromBody] CandidateDto candidateDto)
        {
            if (candidateDto == null)
            {
                return BadRequest("Candidate data is null");
            }

            var validationResult = await _validator.ValidateAsync(candidateDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => new
                {
                    Field = x.PropertyName,
                    Error = x.ErrorMessage
                }));
            }

            try
            {
                var (candidate, isUpdateAction) = await _candidateService.UpsertCandidateAsync(candidateDto);

                if (isUpdateAction)
                {
                    return Ok(candidate);
                }
                else
                {
                    return CreatedAtAction(nameof(UpsertCandidate), new { email = candidate.Email }, candidate);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
