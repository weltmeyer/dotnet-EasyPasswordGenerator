# dotnet-EasyPasswordGenerator
.net lib to generated passwords with special requirements very easy

Easy generation and validation of passwords with one ore more of the following requirements:
 - MinLength 
 - MaxLength
 - AllowLowerCase 
 - RequireLowerCase 
 - AllowUpperCase 
 - RequireUpperCase 
 - AllowNumbers 
 - RequireNumber 
 - AllowSpecial 
 - RequireSpecial 
 - MaxConsecutiveSameCharacter
 - DisableConfusableCharacters 
 - ForbiddenSequences
   -  e.g. dont allow username in password
 
# install
install via nuget: Weltmeyer.EasyPasswordGenerator 

dotnet add package Weltmeyer.EasyPasswordGenerator

https://www.nuget.org/packages/Weltmeyer.EasyPasswordGenerator

# usage
## Using parameters
 ```c#
var pass = PasswordGenerator.Generate(minLength: 10, maxLength: 15) //generates a Password with 10 to 15 characters, can container all characters
 ```
## Using Configuration
```c#
 var cfgGenerator = new PasswordConfiguration
        {
            MinLength = 10, MaxLength = 20,
            AllowSpecial = false,
            AllowNumbers = false,
            AllowUpperCase = false,
            AllowLowerCase = true,
            ForbiddenSequences = new[] { "aa", "bb", "cc" }
        };
 var pass = PasswordGenerator.Generate(cfgGenerator);
 ```
## Validate password requirements:
```c#
    [Test]
    public void TestFail_MissingNumber()
    {
        var str = "abcdefgh";
        var validationResult = PasswordGenerator.Validate(str.ToCharArray(), new PasswordConfiguration
        {
            RequireNumber = true,
        });
        Assert.False(validationResult.Success);
        Assert.Contains(PasswordValidationResult.EnmValidationError.NumbersRequired, validationResult.ValidationErrors);
    }
 ```

You can find more examples in the unit-tests
