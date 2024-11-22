using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using JobCandidateHub.Controllers;
using JobCandidateHub.DTOs;
using JobCandidateHub.Models.Entities;
using JobCandidateHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobCandidateHub.Tests.Controllers
{
    public class CandidateControllerTests
    {
        private readonly ICandidateService _candidateService;
        private readonly IValidator<CandidateDto> _validator;
        private readonly CandidateController _controller;

        public CandidateControllerTests()
        {
            _candidateService = A.Fake<ICandidateService>();
            _validator = A.Fake<IValidator<CandidateDto>>();
            _controller = new CandidateController(_candidateService, _validator);
        }

        [Fact]
        public async Task UpsertCandidate_WithValidData_ForNewCandidate_ReturnsCreatedResponse()
        {
            //Arrange
            var candidateDto = new CandidateDto("sabin@example.com", "sabin", "chhetri", "Test Comment");

            var candidate = new Candidate
            {
                Email = candidateDto.Email,
                FirstName = candidateDto.FirstName,
                LastName = candidateDto.LastName,
                Comment = candidateDto.Comment
            };

            A.CallTo(() => _validator.ValidateAsync(candidateDto, A<CancellationToken>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _candidateService.UpsertCandidateAsync(candidateDto))
                .Returns((candidate, false));

            // Act
            var result = await _controller.UpsertCandidate(candidateDto);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(CandidateController.UpsertCandidate));
            createdAtActionResult.RouteValues["email"].Should().Be(candidate.Email);
            createdAtActionResult.Value.Should().BeEquivalentTo(candidate);
        }

        [Fact]
        public async Task UpsertCandidate_WithValidData_ForExistingCandidate_ReturnsOkResponse()
        {
            // Arrange
            var candidateDto = new CandidateDto("sabin@example.com", "sabin", "chhetri", "Test Comment");


            var candidate = new Candidate
            {
                Email = candidateDto.Email,
                FirstName = candidateDto.FirstName,
                LastName = candidateDto.LastName,
                Comment = candidateDto.Comment
            };

            A.CallTo(() => _validator.ValidateAsync(candidateDto, A<CancellationToken>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _candidateService.UpsertCandidateAsync(candidateDto))
                .Returns((candidate, true));

            // Act
            var result = await _controller.UpsertCandidate(candidateDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(candidate);
        }

        [Fact]
        public async Task UpsertCandidate_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var candidateDto = new CandidateDto("sabin@example.com", "sabin", "chhetri", "Test Comment");


            A.CallTo(() => _validator.ValidateAsync(candidateDto, A<CancellationToken>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _candidateService.UpsertCandidateAsync(candidateDto))
                .ThrowsAsync(new Exception("Test error"));

            // Act
            var result = await _controller.UpsertCandidate(candidateDto);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("Test error");
        }

        [Fact]
        public async Task UpsertCandidate_WithNullInput_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpsertCandidate(null);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Candidate data is null");
        }

        [Theory]
        [InlineData("sabin@example.com", "sabin", "Chhetri", "Test comment")]
        [InlineData("Bipin@test.com", "Bipin", "Chhetri", "Another comment")]
        public async Task UpsertCandidate_WithDifferentValidInputs_ReturnsSuccessfulResponse(
            string email, string firstName, string lastName, string comment)
        {
            // Arrange
            var candidateDto = new CandidateDto(email, firstName, lastName, comment);


            var candidate = new Candidate
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Comment = comment
            };

            A.CallTo(() => _validator.ValidateAsync(candidateDto, A<CancellationToken>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _candidateService.UpsertCandidateAsync(candidateDto))
                .Returns((candidate, false));

            // Act
            var result = await _controller.UpsertCandidate(candidateDto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}