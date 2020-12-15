using System.Collections.Generic;
using System.Linq;

namespace VaultSecretsTool.Models
{
    public class Secrets : Dictionary<string, string>
    {
        public override string ToString()
        {
            return "{" + string.Join(",", this.Select(kv => $"\"{kv.Key}\":\"{kv.Value}\"")) + "}";
        }
    }
}