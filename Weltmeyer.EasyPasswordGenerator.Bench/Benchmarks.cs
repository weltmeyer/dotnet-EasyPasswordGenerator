using BenchmarkDotNet.Attributes;

namespace Weltmeyer.EasyPasswordGenerator.Bench;

[MemoryDiagnoser]
public class Benchmarks
{
    [Params(10, 50, 100)] public int MinLength;

    public PasswordConfiguration _testConfig = new PasswordConfiguration
    {
        RequireNumber = true,
        RequireSpecial = true,
        RequireLowerCase = true,
        RequireUpperCase = true,
        DisableConfusableCharacters = true,
        MaxConsecutiveSameCharacter = 2,
        ForbiddenSequences = new[] { "asdf", "qwertz" }
    };

    [Benchmark]
    public void CreatePassword()
    {
        _testConfig.MinLength = MinLength;
        _testConfig.MaxLength = MinLength;
        PasswordGenerator.Generate(_testConfig);
    }

    [Benchmark]
    public void CreatePasswordAndValidate()
    {
        _testConfig.MinLength = MinLength;
        _testConfig.MaxLength = MinLength;
        var testPassword = PasswordGenerator.Generate(_testConfig);
        _ = PasswordGenerator.Validate(testPassword, _testConfig);
    }
}