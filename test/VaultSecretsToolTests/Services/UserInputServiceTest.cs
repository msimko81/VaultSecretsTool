using System;
using System.Collections.Generic;
using System.IO;
using VaultSecretsTool.Services;
using Xunit;

namespace VaultSecretsToolTests.Services
{
    public class UserInputServiceTest
    {
        [Theory]
        [MemberData(nameof(PromptUserForInput_singleTestData))]
        public void PromptUserForInput(string message, string defaultValue, string expectedValue,
            string expectedStdout, string stdinInput)
        {
            //before
            var service = new UserInputService();

            var stdout = new StringWriter();
            Console.SetOut(stdout);

            var stdin = new StringReader(stdinInput);
            Console.SetIn(stdin);

            //when
            var value = service.PromptUserForInput(message, defaultValue);

            //then
            Assert.Equal(expectedStdout, stdout.ToString());
            Assert.Equal(expectedValue, value);
        }

        public static IEnumerable<object[]> PromptUserForInput_singleTestData()
        {
            return new List<object[]>
            {
                //user hits Enter, no default value
                new object[]
                {
                    "Enter a value ({0}): ", null, null,
                    "Enter a value (not configured): " + Environment.NewLine, Environment.NewLine
                },
                //user hits Enter submitting default value
                new object[]
                {
                    "Enter a value ({0}): ", "defaultValue", "defaultValue",
                    "Enter a value (defaultValue): " + Environment.NewLine, Environment.NewLine
                },
                //user enters value, no default value
                new object[]
                {
                    "Enter a value ({0}): ", null, "userInput",
                    "Enter a value (not configured): " + Environment.NewLine, "userInput" + Environment.NewLine
                },
                //user enters value, default value defined
                new object[]
                {
                    "Enter a value ({0}): ", "defaultValue", "userInput",
                    "Enter a value (defaultValue): " + Environment.NewLine, "userInput" + Environment.NewLine
                }
            };
        }

        [Theory]
        [MemberData(nameof(PromptUserForInput_collectionTestData))]
        public void PromptUserForInputWithChoices(string message, string defaultValue, IEnumerable<string> choices,
            string expectedValue,
            string expectedStdout, string stdinInput)
        {
            //before
            var service = new UserInputService();

            var stdout = new StringWriter();
            Console.SetOut(stdout);

            var stdin = new StringReader(stdinInput);
            Console.SetIn(stdin);

            //when
            var value = service.PromptUserForInput(message, defaultValue, choices);

            //then
            Assert.Equal(expectedStdout, stdout.ToString());
            Assert.Equal(expectedValue, value);
        }

        public static IEnumerable<object[]> PromptUserForInput_collectionTestData()
        {
            var choices = new[] { "one", "two", "three" };
            var choicesPrompt = "Enter a value ({0}): " + Environment.NewLine +
                                "1 -> one" + Environment.NewLine +
                                "2 -> two" + Environment.NewLine +
                                "3 -> three" + Environment.NewLine;

            return new List<object[]>
            {
                //user hits Enter, no default value
                new object[]
                {
                    "Enter a value ({0}): ", null, choices,
                    null,
                    string.Format(choicesPrompt, "not configured"),
                    Environment.NewLine
                },
                //user hits Enter submitting default value
                new object[]
                {
                    "Enter a value ({0}): ", "two", choices,
                    "two",
                    string.Format(choicesPrompt, "two"),
                    Environment.NewLine
                },
                //user enters value by index, no default value
                new object[]
                {
                    "Enter a value ({0}): ", null, choices,
                    "three",
                    string.Format(choicesPrompt, "not configured"),
                    "3" + Environment.NewLine
                },
                //user enters value by index, default value defined
                new object[]
                {
                    "Enter a value ({0}): ", "two", choices,
                    "one",
                    string.Format(choicesPrompt, "two"),
                    "1" + Environment.NewLine
                },
                //user enters custom value
                new object[]
                {
                    "Enter a value ({0}): ", "two", choices,
                    "custom",
                    string.Format(choicesPrompt, "two"),
                    "custom" + Environment.NewLine
                },
                //user inserts only white chars
                new object[]
                {
                    "Enter a value ({0}): ", "two", choices,
                    "two",
                    string.Format(choicesPrompt, "two"),
                    " " + Environment.NewLine
                },
            };
        }
    }
}