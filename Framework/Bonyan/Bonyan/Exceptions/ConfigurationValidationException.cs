namespace Bonyan.Exceptions
{
    /// <summary>
    /// Exception that is thrown when configuration validation fails for a specific type.
    /// </summary>
    public class ConfigurationValidationException : BonyanException
    {
        /// <summary>
        /// Gets the type of the configuration that failed validation.
        /// </summary>
        public Type ConfigurationType { get; }

        /// <summary>
        /// Gets the list of validation errors.
        /// </summary>
        public IReadOnlyList<string> ValidationErrors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="configurationType">The type of configuration that failed validation.</param>
        /// <param name="message">A message describing the validation failure.</param>
        public ConfigurationValidationException(Type configurationType, string message)
            : base(message)
        {
            ConfigurationType = configurationType;
            ValidationErrors = new List<string> { message };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="configurationType">The type of configuration that failed validation.</param>
        /// <param name="errors">A list of validation error messages.</param>
        public ConfigurationValidationException(Type configurationType, IEnumerable<string> errors)
            : base($"Configuration validation failed for type '{configurationType.Name}'.")
        {
            ConfigurationType = configurationType;
            ValidationErrors = new List<string>(errors);
        }

        /// <summary>
        /// Returns a detailed message including all validation errors.
        /// </summary>
        public override string ToString()
        {
            var errorMessages = string.Join(Environment.NewLine, ValidationErrors);
            return $"{base.ToString()}\nValidation errors:\n{errorMessages}";
        }
    }
}
