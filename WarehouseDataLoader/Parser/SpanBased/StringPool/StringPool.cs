using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser.SpanBased.StringPool
{
    internal class StringPool : IStringPool
    {
        private readonly Dictionary<int, string> pool = new Dictionary<int, string>();


        public string GetString(in ReadOnlySpan<char> span)
        {
            int hash = GetDeterministicHashCode(in span);
            if (pool.ContainsKey(hash))
            {
                string pooledString = pool[hash];

                bool areTheSame = true;

                if (span.Length == pooledString.Length)
                {
                    for (int i = 0; i < span.Length; ++i)
                    {
                        if (pooledString[i] != span[i])
                        {
                            areTheSame = false;
                            break;
                        }
                    }
                }

                if (areTheSame)
                {
                    return pool[hash];
                }
            }
            pool[hash] = span.ToString();
            return pool[hash];
        }

        private int GetDeterministicHashCode(in ReadOnlySpan<char> text)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < text.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ text[i];
                    if (i == text.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ text[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
