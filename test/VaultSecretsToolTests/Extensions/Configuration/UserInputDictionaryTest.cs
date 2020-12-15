using Microsoft.Extensions.Configuration;
using Moq;
using VaultSecretsTool.Extensions.Configuration;
using VaultSecretsTool.Services;
using Xunit;

namespace VaultSecretsToolTests.Extensions.Configuration
{
    public class UserInputDictionaryTest
    {
        [Fact]
        public void Indexer()
        {
            //before
            var parentConfigurationMock = new Mock<IConfiguration>();
            parentConfigurationMock.SetupGet(mock => mock[It.Is<string>(s => s == "key")])
                .Returns("defaultValue");

            var userInputServiceMock = new Mock<IUserInputService>();
            userInputServiceMock
                .Setup(mock =>
                    mock.PromptUserForInput(It.Is<string>(s => s == "Enter a key ({0}): "),
                        It.Is<string>(s => s == "defaultValue")))
                .Returns("userDefinedValue");

            var dictionary = new UserInputDictionary(parentConfigurationMock.Object, userInputServiceMock.Object);

            //when
            var value = dictionary["key"];

            //then
            Assert.Equal("userDefinedValue", value);
        }
   }
}