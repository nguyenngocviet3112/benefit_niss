using System;
using System.Text;

public static class IdDecoder
{

    private const int NUM_A = 99;
    private const int NUM_B = 123456789;
    private const int SECRET_XOR = 0x5a5a5a5a;

    public static int DecodeId(string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
            throw new ArgumentNullException(nameof(encoded));

        // 1. Base64URL -> Base64 chuẩn
        string b64 = encoded.Replace('-', '+').Replace('_', '/');
        switch (b64.Length % 4)
        {
            case 2: b64 += "=="; break;
            case 3: b64 += "="; break;
        }

        // 2. Base64 decode
        string raw = Encoding.UTF8.GetString(Convert.FromBase64String(b64)).Trim();

        // raw phải có dạng "salt:mixed"
        var parts = raw.Split(':');
        if (parts.Length != 2)
            throw new FormatException($"Invalid encoded ID: '{raw}'");

        // 3. Parse salt (không dùng, chủ yếu để validate)
        if (!long.TryParse(parts[0], out long salt))
            throw new FormatException($"Invalid salt: '{parts[0]}'");

        // 4. Parse mixed bằng long
        if (!long.TryParse(parts[1], out long mixed))
            throw new FormatException($"Invalid mixed value: '{parts[1]}'");

        // 5. Giải ngược công thức:
        // mixed = (id ^ SECRET_XOR) * NUM_A + NUM_B
        long xoredLong = (mixed - NUM_B) / NUM_A;

        // 6. Check range rồi cast về int
        if (xoredLong < int.MinValue || xoredLong > int.MaxValue)
            throw new OverflowException($"Decoded value out of Int32 range: {xoredLong}");

        int xored = (int)xoredLong;
        int id = xored ^ SECRET_XOR;

        return id;
    }
}