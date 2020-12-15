using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VaultSecretsTool.Services;

namespace VaultSecretsTool.Extensions.Configuration
{
    class UserInputDictionary : IDictionary<string, string>
    {
        private readonly IConfiguration _parentConfiguration;
        private readonly IUserInputService _userInputService;

        public UserInputDictionary(IConfiguration parentConfiguration, IUserInputService userInputService)
        {
            _parentConfiguration = parentConfiguration;
            _userInputService = userInputService;
        }

        public string this[string key]
        {
            get
            {
                var configuredValue = _parentConfiguration[key];
                return _userInputService.PromptUserForInput($"Enter a {key} ({{0}}): ", configuredValue);
            }
            set => Add(key, value);
        }

        public bool ContainsKey(string key) => true;

        public bool TryGetValue(string key, out string value)
        {
            value = this[key];
            return true;
        }

        public ICollection<string> Keys => Array.Empty<string>();
        public ICollection<string> Values => Array.Empty<string>();

        public bool IsReadOnly => true;
        public int Count => 0;

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<string, string> item) => Add(item.Key, item.Value);

        public void Clear() => throw new NotImplementedException();

        public bool Contains(KeyValuePair<string, string> item) => false;

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(KeyValuePair<string, string> item) => throw new NotImplementedException();

        public void Add(string key, string value) => _parentConfiguration[key] = value;

        public bool Remove(string key) => false;
    }
}