using System;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.DataModel.Packer.Benchmark;

static class TextHelper
{
    const string chars =
       "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
       "abcdefghijklmnopqrstuvwxyz" +
       "0123456789";

    public static string Create(int length)
    {
        var random = new Random();

        return string.Create(length, random, (span, random) =>
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = chars[random.Next(chars.Length)];
            }
        });
    }
}
