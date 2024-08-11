using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Validation
{
    /// <summary>
    /// Custom validation attribute to validate a phone number using the libphonenumber-csharp library.
    /// </summary>
    public class PhoneNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the phone number by parsing it with libphonenumber-csharp.
        /// </summary>
        /// <param name="value">The value of the phone number to validate.</param>
        /// <param name="validationContext">The context within which validation is performed.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            // Check if the value is null or empty
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Mobile number is required.");
            }

            try
            {
                // Attempt to parse the phone number
                var phoneNumber = phoneNumberUtil.Parse(value.ToString(), null);

                // Validate if the parsed number is a valid phone number
                if (!phoneNumberUtil.IsValidNumber(phoneNumber))
                {
                    return new ValidationResult("Invalid phone number.");
                }
            }
            catch (NumberParseException)
            {
                // If parsing fails, return an error
                return new ValidationResult("Invalid phone number format.");
            }

            // If everything is valid, return success
            return ValidationResult.Success;
        }
    }
}
