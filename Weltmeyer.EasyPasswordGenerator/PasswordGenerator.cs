using System.Security.Cryptography;

namespace Weltmeyer.EasyPasswordGenerator;

public class PasswordGenerator
{
    static readonly char[] ConfusableCharacters = { 'o', 'O', '0', 'l', 'I' };
    static readonly char[] LowerCase = Enumerable.Range(97, 122 - 97).Select(x => (char)x).ToArray();
    static readonly char[] UpperCase = Enumerable.Range(65, 90 - 65).Select(x => (char)x).ToArray();
    static readonly char[] Numbers = Enumerable.Range(48, 57 - 48).Select(x => (char)x).ToArray();
    static readonly char[] Special = Enumerable.Range(33, 47 - 33).Select(x => (char)x).ToArray();

    private static int GetNextRnd(int minInclusive, int maxInclusive)
    {//RandomNumberGenerator
        return RandomNumberGenerator.GetInt32(minInclusive, maxInclusive + 1);
        return Random.Shared.Next(minInclusive, maxInclusive + 1);
    }

    private enum CharType
    {
        LowerCaseId = 1,
        UpperCaseId = 2,
        NumbersId = 3,
        SpecialId = 4
    }

    /*private const int LowerCaseId = 1;
    private const int UpperCaseId = 2;
    private const int NumbersId = 3;
    private const int SpecialId = 4;*/

    /// <summary>
    /// Validates password requirements against the given configuration
    /// </summary>
    /// <param name="password"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static bool Validate(string password, PasswordConfiguration configuration) =>
        Validate(password.ToCharArray(), configuration);

    public static bool Validate(Span<char> password, PasswordConfiguration configuration) => Validate(password,
        minLength: configuration.MinLength,
        maxLength: configuration.MaxLength,
        allowLowerCase: configuration.AllowLowerCase,
        requireLowerCase: configuration.RequireLowerCase,
        allowUpperCase: configuration.AllowUpperCase,
        requireUpperCase: configuration.RequireUpperCase,
        allowNumbers: configuration.AllowNumbers,
        requireNumber: configuration.RequireNumber,
        allowSpecial: configuration.AllowSpecial,
        requireSpecial: configuration.RequireSpecial,
        maxConsecutiveSameCharacter: configuration.MaxConsecutiveSameCharacter,
        disableConfusableCharacters: configuration.DisableConfusableCharacters,
        forbiddenSequences: configuration.ForbiddenSequences
    );

    public static bool Validate(string password, int minLength = 8, int maxLength = 99,
        bool allowLowerCase = true, bool requireLowerCase = false,
        bool allowUpperCase = true, bool requireUpperCase = false,
        bool allowNumbers = true, bool requireNumber = false,
        bool allowSpecial = true, bool requireSpecial = false,
        int maxConsecutiveSameCharacter = 2,
        bool disableConfusableCharacters = true,
        string[]? forbiddenSequences = null) => Validate(password.AsSpan()
        , minLength, maxLength,
        allowLowerCase, requireLowerCase,
        allowUpperCase, requireUpperCase,
        allowNumbers, requireNumber,
        allowSpecial, requireSpecial,
        maxConsecutiveSameCharacter, disableConfusableCharacters,
        forbiddenSequences
    );

    public static bool Validate(ReadOnlySpan<char> password, int minLength = 8, int maxLength = 99,
        bool allowLowerCase = true, bool requireLowerCase = false,
        bool allowUpperCase = true, bool requireUpperCase = false,
        bool allowNumbers = true, bool requireNumber = false,
        bool allowSpecial = true, bool requireSpecial = false,
        int maxConsecutiveSameCharacter = 2,
        bool disableConfusableCharacters = true,
        string[]? forbiddenSequences = null)
    {
        if (password.Length < minLength)
            return false;
        if (password.Length > maxLength)
            return false;


        if (!ValidateRequirement(password, LowerCase, allowLowerCase, requireLowerCase))
            return false;
        if (!ValidateRequirement(password, UpperCase, allowUpperCase, requireUpperCase))
            return false;
        if (!ValidateRequirement(password, Numbers, allowNumbers, requireNumber))
            return false;
        if (!ValidateRequirement(password, Special, allowSpecial, requireSpecial))
            return false;


        if (maxConsecutiveSameCharacter <= password.Length && HasConsecutiveChars(password, maxConsecutiveSameCharacter + 1))
            return false;

        if (!ValidateRequirement(password, ConfusableCharacters, !disableConfusableCharacters, false))
            return false;
        if (forbiddenSequences is not null)
        {
            var strPw = new string(password);
            var matches = forbiddenSequences.Any(seq => strPw.Contains(seq));
            if (matches)
                return false;
        }

        return true;
    }


    private static bool HasConsecutiveChars(ReadOnlySpan<char> source, int sequenceLength)
    {
        if (source.Length < sequenceLength)
            return false;

        char lastSeen = source[0];
        var count = 1;

        foreach (var c in source.Slice(1))
        {
            if (lastSeen == c)
                count++;
            else
                count = 1;

            if (count == sequenceLength)
                return true;

            lastSeen = c;
        }

        return false;
    }

    private static bool ValidateRequirement(ReadOnlySpan<char> haystack, Span<char> needles, bool allowed, bool required)
    {
        if (allowed && !required)
            return true;
        var exists = haystack.IndexOfAny(needles) >= 0;
        if (exists && !allowed)
            return false;
        if (!exists && required)
            return false;
        return true;
    }

    public static string Generate(PasswordConfiguration configuration) => Generate(
        minLength: configuration.MinLength,
        maxLength: configuration.MaxLength,
        allowLowerCase: configuration.AllowLowerCase,
        requireLowerCase: configuration.RequireLowerCase,
        allowUpperCase: configuration.AllowUpperCase,
        requireUpperCase: configuration.RequireUpperCase,
        allowNumbers: configuration.AllowNumbers,
        requireNumber: configuration.RequireNumber,
        allowSpecial: configuration.AllowSpecial,
        requireSpecial: configuration.RequireSpecial,
        maxConsecutiveSameCharacter: configuration.MaxConsecutiveSameCharacter,
        disableConfusableCharacters: configuration.DisableConfusableCharacters,
        forbiddenSequences: configuration.ForbiddenSequences
    );


    public static string Generate(
        int minLength = 8, int maxLength = 99,
        bool allowLowerCase = true, bool requireLowerCase = false,
        bool allowUpperCase = true, bool requireUpperCase = false,
        bool allowNumbers = true, bool requireNumber = false,
        bool allowSpecial = true, bool requireSpecial = false,
        int maxConsecutiveSameCharacter = 2,
        bool disableConfusableCharacters = true,
        string[]? forbiddenSequences = null
    )
    {
        if (minLength < 1)
            minLength = 1;
        if (maxLength < 1)
            throw new ArgumentException("length<1 dont make sense...");


        var requireLength =
            (requireLowerCase ? 1 : 0) +
            (requireUpperCase ? 1 : 0) +
            (requireNumber ? 1 : 0) +
            (requireSpecial ? 1 : 0);


        if (requireLowerCase && !allowLowerCase)
            throw new ArgumentException("Required but not allowed?", nameof(requireLowerCase));
        if (requireUpperCase && !allowUpperCase)
            throw new ArgumentException("Required but not allowed?", nameof(requireUpperCase));
        if (requireNumber && !allowNumbers)
            throw new ArgumentException("Required but not allowed?", nameof(requireNumber));
        if (requireSpecial && !allowSpecial)
            throw new ArgumentException("Required but not allowed?", nameof(requireSpecial));

        if (minLength < requireLength)
            minLength = requireLength;

        if (maxLength < minLength)
        {
            throw new InvalidOperationException($"Required Length of Password is {minLength}. MaxLength is set to {maxLength}");
        }

        var forceAddLower = requireLowerCase;
        var forceAddUpper = requireUpperCase;
        var forceAddNumber = requireNumber;
        var forceAddSpecial = requireSpecial;

        var randomLength = GetNextRnd(minLength, maxLength);
        var remainingRequireLength = requireLength;
        Span<char> passwordChars = new char[randomLength];

        //var password = new StringBuilder(randomLength);
        var allowedCharTypes = new List<CharType>();
        if (allowLowerCase)
            allowedCharTypes.Add(CharType.LowerCaseId);
        if (allowUpperCase)
            allowedCharTypes.Add(CharType.UpperCaseId);
        if (allowNumbers)
            allowedCharTypes.Add(CharType.NumbersId);
        if (allowSpecial)
            allowedCharTypes.Add(CharType.SpecialId);

        for (int i = 0; i < randomLength; i++)
        {
            var nextType = allowedCharTypes[GetNextRnd(0, allowedCharTypes.Count-1)];
            if (remainingRequireLength >= randomLength - i)
            {
                if (forceAddLower)
                    nextType = CharType.LowerCaseId;
                if (forceAddUpper)
                    nextType = CharType.UpperCaseId;
                if (forceAddNumber)
                    nextType = CharType.NumbersId;
                if (forceAddSpecial)
                    nextType = CharType.SpecialId;
            }

            //var nextChar=switch nextType

            passwordChars[i] = nextType switch
            {
                CharType.LowerCaseId => GetLowerChar(disableConfusableCharacters),
                CharType.UpperCaseId => GetUpperChar(disableConfusableCharacters),
                CharType.NumbersId => GetNumberChar(disableConfusableCharacters),
                CharType.SpecialId => GetSpecialChar(disableConfusableCharacters),
                _ => throw new ArgumentOutOfRangeException()
            };


            if (maxConsecutiveSameCharacter < i + 1)
            {
                var antiConsecutive = false;
                for (int c = i - 1; c >= i - maxConsecutiveSameCharacter; c--)
                {
                    if (passwordChars[c] != passwordChars[i])
                    {
                        antiConsecutive = true;
                        break;
                    }
                }

                if (!antiConsecutive)
                {
                    i--;
                    goto loopCont;
                }
            }


            if (forbiddenSequences is not null && forbiddenSequences.Length > 0)
            {
                foreach (var seq in forbiddenSequences)
                {
                    if (seq.Length > i + 1)
                        continue;
                    var test = passwordChars.Slice(i - seq.Length + 1, seq.Length);
                    if (!test.SequenceEqual(seq.AsSpan()))
                    {
                        continue;
                    }

                    i--;
                    goto loopCont;
                }
            }


            switch (nextType)
            {
                case CharType.LowerCaseId:
                {
                    forceAddLower = false;
                    if (requireLowerCase)
                        remainingRequireLength -= 1;

                    break;
                }
                case CharType.UpperCaseId:
                {
                    forceAddUpper = false;
                    if (requireUpperCase)
                        remainingRequireLength -= 1;

                    break;
                }
                case CharType.NumbersId:
                {
                    forceAddNumber = false;
                    if (requireNumber)
                        remainingRequireLength -= 1;

                    break;
                }
                case CharType.SpecialId:
                {
                    forceAddSpecial = false;
                    if (requireSpecial)
                        remainingRequireLength -= 1;

                    break;
                }
            }


            loopCont: ;
        }


        return new string(passwordChars);
    }


    private static char GetChar(char[] source, bool disableConfusableCharacters)
    {
        char result;
        do
        {
            result = source[GetNextRnd(0, source.Length-1)];
        } while (disableConfusableCharacters && Array.IndexOf(ConfusableCharacters, result) >= 0);


        return result;
    }


    private static char GetUpperChar(bool disableConfusableCharacters) => GetChar(UpperCase, disableConfusableCharacters);

    private static char GetLowerChar(bool disableConfusableCharacters) => GetChar(LowerCase, disableConfusableCharacters);


    private static char GetNumberChar(bool disableConfusableCharacters) => GetChar(Numbers, disableConfusableCharacters);


    private static char GetSpecialChar(bool disableConfusableCharacters) => GetChar(Special, disableConfusableCharacters);
}