using System;
using System.Collections.Generic;
using System.Linq;

namespace VaultSecretsTool.Services
{
    /// <summary>
    /// Service to query user for an input.
    /// </summary>
    public interface IUserInputService
    {
        /// <summary>
        /// Query user for an input with given <paramref name="message"/> and <paramref name="defaultValue"/>.
        /// Allow user to either submit the default value or to enter his own value. 
        /// </summary>
        /// <param name="message">A message with placeholder for the default value to be shown to the user.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>User defined value.</returns>
        string PromptUserForInput(string message, string defaultValue);

        /// <summary>
        /// Query user for an input with given <paramref name="message"/>, list of <paramref name="choices"/> and <paramref name="defaultValue"/>.
        /// Allow user to either submit the default value, to choose from the list of choices or to enter his own value. 
        /// </summary>
        /// <param name="message">A message with placeholder for the default value to be shown to the user.</param>
        /// <param name="choices">A list of choices to select from.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>User defined value.</returns>
        string PromptUserForInput(string message, string defaultValue, IEnumerable<string> choices);
    }


    /// <summary>
    /// <p>
    /// Service to query user for an input using Console.
    /// </p>
    /// <p>
    /// Writes message to the Console and waits for user input submitted by hitting Enter.
    /// </p>
    /// <ul>
    ///     <li>Default value is submitted by hitting Enter without entering any other input.</li>
    ///     <li>To insert custom value, insert the value and submit the value by hitting Enter.</li>
    ///     <li>To choose from a list of choices, enter the choice index and submit the choice by hitting Enter.</li>
    /// </ul> 
    /// </summary>
    public class UserInputService : IUserInputService
    {
        public string PromptUserForInput(string message, string defaultValue)
        {
            Console.WriteLine(message, defaultValue ?? "not configured");
            var input = Console.ReadLine();
            return input?.Trim().Length > 0 ? input : defaultValue;
        }

        public string PromptUserForInput(string message, string defaultValue, IEnumerable<string> choices)
        {
            var indexedChoices = choices
                .Select((val, index) => new { Index = (index + 1).ToString(), Value = val })
                .ToDictionary(kv => kv.Index, kv => kv.Value);

            Console.WriteLine(message, defaultValue ?? "not configured");
            foreach (var kv in indexedChoices)
            {
                Console.WriteLine($"{kv.Key} -> {kv.Value}");
            }

            var input = Console.ReadLine();
            if (input == null || input.Trim().Length == 0)
            {
                return defaultValue;
            }

            return input.Trim().Length > 0
                ? (indexedChoices.TryGetValue(input, out var filename) ? filename : input)
                : defaultValue;
        }
    }
}