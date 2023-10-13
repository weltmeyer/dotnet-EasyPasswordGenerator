namespace Weltmeyer.EasyPasswordGenerator.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void LenghtTest_1()
    {
        var str = PasswordGenerator.Generate(minLength: 5, maxLength: 10);
        //Console.WriteLine($"LenghtTest1:{str}");
        Assert.GreaterOrEqual(str.Length, 5);
        Assert.LessOrEqual(str.Length, 10);
    }


    [Test]
    public void ConfigTest_1()
    {
        var cfg = new PasswordConfiguration { MinLength = 5, MaxLength = 10 };
        var str = PasswordGenerator.Generate(cfg);
        Assert.GreaterOrEqual(str.Length, 5);
        Assert.LessOrEqual(str.Length, 10);

        Assert.True(PasswordGenerator.Validate(str, cfg));
    }

    [Test]
    public void ConfigTest_2()
    {
        var cfg = new PasswordConfiguration { MinLength = 50, MaxLength = 60 };
        var str = PasswordGenerator.Generate(cfg);

        Assert.True(PasswordGenerator.Validate(str, cfg));
        cfg.MaxLength = 10;
        cfg.MinLength = 8;
        Assert.False(PasswordGenerator.Validate(str, cfg));
    }

    [Test]
    public void ConfigTest_3()
    {
        var cfg = new PasswordConfiguration { MinLength = 10, MaxLength = 20, AllowSpecial = false };
        var str = PasswordGenerator.Generate(cfg);

        Assert.True(PasswordGenerator.Validate(str, cfg));
        cfg.AllowSpecial = true;
        cfg.RequireSpecial = true;
        Assert.False(PasswordGenerator.Validate(str, cfg));
        Assert.False(PasswordGenerator.Validate(str, allowSpecial: true, requireSpecial: true));
    }


    [Test]
    public void TestFail_PasswordTooShort()
    {
        var cfg = new PasswordConfiguration { MinLength = 10, MaxLength = 20 };
        var str = PasswordGenerator.Generate(cfg);

        Assert.True(PasswordGenerator.Validate(str, cfg));
        cfg.MinLength = 30;
        Assert.False(PasswordGenerator.Validate(str, cfg));
    }

    [Test]
    public void TestFail_ForbiddenSequence()
    {
        var cfgGenerator = new PasswordConfiguration
        {
            MinLength = 10, MaxLength = 20,
            AllowSpecial = false,
            AllowNumbers = false,
            AllowUpperCase = false,
            AllowLowerCase = true
        };
        var cfgValidator = new PasswordConfiguration
        {
            ForbiddenSequences = new[] { "aa", "bb", "cc" }
        };

        var needFalse = true;
        do
        {
            //ehhh
            var pass = PasswordGenerator.Generate(cfgGenerator);
            needFalse = PasswordGenerator.Validate(pass.ToString(), cfgValidator);
        } while (needFalse);

        Assert.False(needFalse);
    }

    [Test]
    public void TestSuccess_ForbiddenSequence()
    {
        var cfgGenerator = new PasswordConfiguration
        {
            MinLength = 10, MaxLength = 20,
            AllowSpecial = false,
            AllowNumbers = false,
            AllowUpperCase = false,
            AllowLowerCase = true,
            ForbiddenSequences = new[] { "aa", "bb", "cc" }
        };


        for (int i = 0; i < 100; i++)
        {
            var pass = PasswordGenerator.Generate(cfgGenerator);
            Assert.True(PasswordGenerator.Validate(pass.ToString(), cfgGenerator));
        }
    }

    [Test]
    public void TestFail_MissingNumber()
    {
        var str = "abcdefgh";

        Assert.False(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            RequireNumber = true,
        }));
    }

    [Test]
    public void TestFail_MissingLower()
    {
        var str = "ABCDEFGH";

        Assert.False(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            RequireLowerCase = true,
        }));
    }

    [Test]
    public void TestFail_MissingUpper()
    {
        var str = "abcdefgh";

        Assert.False(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            RequireUpperCase = true,
        }));
    }

    [Test]
    public void TestFail_MissingSpecial()
    {
        var str = "abcdefgh";

        Assert.False(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            RequireSpecial = true,
        }));
    }

    [Test]
    public void TestFail_MaxConsecutive()
    {
        var str = "L9k9skka126777";

        Assert.False(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            MaxConsecutiveSameCharacter = 2
        }));
    }

    [Test]
    public void TestGenerate_MaxConsecutive()
    {
        var cfg = new PasswordConfiguration
        {
            MaxConsecutiveSameCharacter = 1
        };
        for (int i = 0; i < 99; i++)
        {
            _ = PasswordGenerator.Generate(cfg);
        }
    }

    [Test]
    public void TestSuccess_MaxConsecutive()
    {
        var str = "L9k9skka126777";

        Assert.True(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            MaxConsecutiveSameCharacter = 3
        }));
    }

    [Test]
    public void TestSuccess_Confusable()
    {
        var str = "HeliIL01";

        Assert.True(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            DisableConfusableCharacters = false
        }));
    }

    [Test]
    public void TestFail_Confusable()
    {
        var str = "HeliIL01";

        Assert.False(PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            DisableConfusableCharacters = true
        }));
    }

    [Test]
    public void LenghtTest_Fail_1()
    {
        Assert.Catch(() => _ = PasswordGenerator.Generate(minLength: 15, maxLength: 10));
    }

    [Test]
    public void LenghtTest_Fail_2()
    {
        Assert.Catch(() => _ = PasswordGenerator.Generate(minLength: 1, maxLength: 3, requireNumber: true, requireSpecial: true, requireUpperCase: true, requireLowerCase: true));
    }

    [Test]
    public void LenghtTest_Fail_3()
    {
        Assert.Catch(() => _ = PasswordGenerator.Generate(minLength: -1, maxLength: -1, requireNumber: true, requireSpecial: true, requireUpperCase: true, requireLowerCase: true));
    }

    [Test]
    public void RequireTest_Fail_1()
    {
        Assert.Catch(() => _ = PasswordGenerator.Generate(requireLowerCase: true, allowLowerCase: false));
        Assert.Catch(() => _ = PasswordGenerator.Generate(requireUpperCase: true, allowUpperCase: false));
        Assert.Catch(() => _ = PasswordGenerator.Generate(requireNumber: true, allowNumbers: false));
        Assert.Catch(() => _ = PasswordGenerator.Generate(requireSpecial: true, allowSpecial: false));
    }

    [Test]
    public void RequireTest_Low_1()
    {
        var pass = PasswordGenerator.Generate(requireLowerCase: true, requireSpecial: true, requireUpperCase: true, requireNumber: true, maxLength: 4, minLength: 4);
        Assert.That(pass.Length == 4);
    }

    [Test]
    public void StringTest_1()
    {
        var pass = PasswordGenerator.Generate(requireLowerCase: true, requireSpecial: true, requireUpperCase: true, requireNumber: true, maxLength: 4, minLength: 4);
        string toast = pass;

        Assert.That(pass.Length == 4);
    }
}