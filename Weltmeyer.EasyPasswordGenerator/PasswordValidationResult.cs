namespace Weltmeyer.EasyPasswordGenerator;

public class PasswordValidationResult
{
    public bool Success => ValidationErrors.Count == 0;

    public List<EnmValidationError> ValidationErrors { get; } = new();

    public enum EnmValidationError
    {
        LowerCaseNotAllowed,
        LowerCaseRequired,

        UpperCaseNotAllowed,
        UpperCaseRequired,

        NumbersNotAllowed,
        NumbersRequired,

        SpecialCharsNotAllowed,
        SpecialCharsRequired,

        TooLong,
        TooShort,

        MaxConsecutiveSameCharacter,

        ContainsConfusableCharacters,

        ContainsForbiddenSequence,
    }
}