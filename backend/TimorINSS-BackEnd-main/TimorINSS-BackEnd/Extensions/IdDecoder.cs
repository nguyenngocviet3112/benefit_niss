using System;
using System.Text;

public static class IdDecoder
{
    private const int SECRET_A = 999;
    private const int SECRET_B = 123456789;

    public static int DecodeId(string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
            throw new ArgumentNullException(nameof(encoded));

        // 1️⃣ Base64URL → Base64 chuẩn
        string b64 = encoded.Replace('-', '+').Replace('_', '/');
        switch (b64.Length % 4)
        {
            case 2: b64 += "=="; break;
            case 3: b64 += "="; break;
        }

        // 2️⃣ Giải lớp ngoài (base64 lần 1)
        string inner = Encoding.UTF8.GetString(Convert.FromBase64String(b64)).Trim();

        // 3️⃣ Giải lớp trong (base64 lần 2)
        string numberStr = Encoding.UTF8.GetString(Convert.FromBase64String(inner)).Trim();

        // 4️⃣ Chuyển sang số và đảo công thức
        if (!int.TryParse(numberStr, out int transformed))
            throw new FormatException("ID not correct");

        int decodedId = (transformed - SECRET_B) / SECRET_A;
        return decodedId;
    }
}
