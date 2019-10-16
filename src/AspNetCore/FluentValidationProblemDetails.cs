using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.AspNetCore.FluentValidation
{
    /// <summary>
    /// Problem Details for Fluent Validation
    /// </summary>
    [PublicAPI]
    public class FluentValidationProblemDetails : ValidationProblemDetails
    {
        /// <summary>
        /// Construct the Fluent Validation Problem Details
        /// </summary>
        public FluentValidationProblemDetails() : this(Array.Empty<ValidationFailure>())
        {
        }

        /// <summary>
        /// Build Fluent Validation Problem Details from a <see cref="ValidationResult"/>
        /// </summary>
        /// <param name="result"></param>
        public FluentValidationProblemDetails(ValidationResult result) : this(result.Errors)
        {
            Rules = result.RuleSetsExecuted;
        }

        /// <summary>
        /// Build Fluent Validation Problem Details from a <see cref="IEnumerable{ValidationFailure}"/>
        /// </summary>
        /// <param name="errors"></param>
        public FluentValidationProblemDetails(IEnumerable<ValidationFailure> errors)
        {
            Errors = errors
                .ToLookup(x => x.PropertyName)
                .ToDictionary(z => z.Key, z => z.Select(item => new FluentValidationProblemDetail(item)).ToArray());
        }

        /// <summary>
        /// Gets the validation errors associated with this instance of <see cref="T:Microsoft.AspNetCore.Mvc.ValidationProblemDetails" />.
        /// </summary>
        [JsonPropertyName("errors")]
        public new IDictionary<string, FluentValidationProblemDetail[]> Errors { get; }

        /// <summary>
        /// The rules run with the validation
        /// </summary>
        public string[] Rules { get; set; } = Array.Empty<string>();

        internal class ProblemDetailsValidator : AbstractValidator<ProblemDetails>
        {
            public ProblemDetailsValidator()
            {
                RuleFor(x => x.Type).NotNull();
                RuleFor(x => x.Title).NotNull();
            }
        }

        internal class ValidationProblemDetailsValidator : AbstractValidator<ValidationProblemDetails>
        {
            public ValidationProblemDetailsValidator()
            {
                Include(new ProblemDetailsValidator());
                RuleFor(x => x.Errors).NotNull();
            }
        }

        internal class Validator : AbstractValidator<FluentValidationProblemDetails>
        {
            public Validator()
            {
                Include(new ProblemDetailsValidator());
                RuleFor(x => x.Rules).NotNull();
                RuleFor(x => x.Errors).NotNull();
            }
        }
    }
}
