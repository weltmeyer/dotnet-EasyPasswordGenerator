namespace Weltmeyer.EasyPasswordGenerator;

public class PasswordConfiguration
{
    public int MinLength { get; set; } = 8;
    public int MaxLength { get; set; } = 99;

    public bool AllowLowerCase { get; set; } = true;
    public bool RequireLowerCase { get; set; } = false;

    public bool AllowUpperCase { get; set; } = true;
    public bool RequireUpperCase { get; set; } = false;

    public bool AllowNumbers { get; set; } = true;
    public bool RequireNumber { get; set; } = false;

    public bool AllowSpecial { get; set; } = true;
    public bool RequireSpecial { get; set; } = false;

    public int MaxConsecutiveSameCharacter { get; set; } = 2;
    public bool DisableConfusableCharacters { get; set; } = true;
    public string[]? ForbiddenSequences { get; set; } = null;
}