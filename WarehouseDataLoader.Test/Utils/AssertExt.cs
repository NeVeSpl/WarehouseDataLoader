using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Test.Utils
{
    /// <summary>
    /// Yup, I do not like fluent asserts therefore I wrap them up :D
    /// </summary>
    internal static class AssertExt
    {
        public static void AreEquivalent(string expected, string actual)
        {
            var normalizedExpected = expected.Replace("\r\n", "\n");
            var normalizedActual = actual.Replace("\r\n", "\n");
            normalizedActual.Should().BeEquivalentTo(normalizedExpected);
        }
    }
}
