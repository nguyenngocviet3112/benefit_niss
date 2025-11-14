using System;
using System.Security.Cryptography;
using System.Text;

public static class IdDecoder
{
    private const long NUM_A = 99;
    private const long NUM_B = 123456789;
    private const int SECRET_XOR = 0x5a5a5a5a;

    private const string PrivateKeyPem = @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAvQ4ENPCTcc5/pZZqY5QxYRRQ94/mYpyYb5uLd4vfL0tv/4zd
0A7nXTKa9qCGaMlMPuE1hvDCdehMe3dS27J2vg6MiK8DSPLXPiDyNzTVFnRknQJF
dPLWpaVnm+Cd08LgWhiW9KFdNKDUO/oWgK05dEXtpfZaPeHjjoGB+njXCKMmmU+h
ztCOkoTnpwQhMQs+Vu2jZlDxJt9yTb1337pWOX3dFuFRtq6eGrLUuhQMtjT0vs5F
8s9sk8WVNbk5CgIE/FaUx61stUrDdM70KVU3Pbjhin/SmqRRK5QzT4vyW3RQ6GlL
cTk0J+obNZEqkhlLdCpBmjtQDVfLM5eZrHENZQIDAQABAoIBABx7ix4Kt2a9rLTe
14J50NgGaThoVqu5WgHz65bZh4jRkxBrlv4sm1DgOevkPWZ2GrryenpXtYRxY2x1
NWeMw0pZA3U9oNS8ZA9OhA+AhzC4h8axO+j7A7m1v3QC6uRSMjOAtuV+QNOX/hcB
06K+oQRShhOrQjyb1rPL8TAyRSYIuW3qtfeO5idWAqyLrvOVRjs6kunPzgbo96m8
AVVfXbvnhw2CZGJGY3Bui+UIl4NmBZ8nfCUCtktgERxYTPxZ3HIdKXjBiBHb7TYR
Hsu9/+9GyR68tGp65YipXYcvyPW7JaaaEPn8eqi9v6Q5V3yey8WXhHrHQv0XWcXs
jGYOBPUCgYEAze2qiSiDhuxi057rGHkBttf934ZHJqfbDPAYGmqdBwd/i+krSxwd
0uqdknYxwy0Vnj7y+cWIVyZJbNpJsbQovFxIwq5qQicuMrPRklOS2uEENWafmOqj
5T1/mc/dTOUxBezYwB0XXp3ZU8lugRmJDrINCgGYMolImyfWhCtL63cCgYEA6wYG
U96rvfck0VKqMIT4rCuX28DJGx724alc7ize7v0IsHGueZnhTmCTmp6v2A9ELi+o
8njTXHMaFkicOBwaer5qq1s9QCjxP1DK7Ay459YoXAlQNgtRq2dJJluDiJ39X6l+
dS9l9rHn1o5goudTxdPBMJaPn7KU85Etfvq5zQMCgYAYKDFBNcd4JHLTcGkTzd4t
nNjjR9VA65/+vIuXTbpuAKsttoSlov68p1kkvUjJJJjMn4XRVyLPVmZ2poTOm9vL
fOfsLpm4ePAqcn27NKKdmpfry8YdIiln0vcNDB1hz+EYWNh0cPU03D+KXK7h5iT1
3F07N0SraP9jdCDuEEYvsQKBgE2nYfPYQPq1bC7Vl3IMnXWVuclcO5aAdqY9JX10
yfxBEtvp0/JNy8nv0xVllUhYUvjHupTTI1MKnPX769IGRyGDRQ91SBmo7X00Hrg7
w+SmOBOg6CXTO5rct6i62A0N1qgDAsuJa7FFOwsDZB6vPFjDDLQXK0Fi6iBIObYY
9cctAoGBAK2b4uPAWl1RxWmLum1ZOy57KvRzTPGCgRjk3pIp0K7hs2ndbYQB7gkZ
amNzpS/uHGwt1zEq8VjrBK+hpVg0Ur+OwIxC/h8xEIDgIsXDYuhR3oGVzHg/BLG9
eLJdfd7Nncgs++HRV0GG5ogacajJqKVuXCUrpPNqaQP1GIQvo5/f
-----END RSA PRIVATE KEY-----";

    public static int DecodeId(string encodedRsa)
    {
        if (string.IsNullOrWhiteSpace(encodedRsa))
            throw new ArgumentNullException(nameof(encodedRsa));

        // Base64URL → Base64
        string b64 = encodedRsa.Replace('-', '+').Replace('_', '/');
        switch (b64.Length % 4)
        {
            case 2: b64 += "=="; break;
            case 3: b64 += "="; break;
        }

        var cipherBytes = Convert.FromBase64String(b64);

        // 1) Load private key
        using RSA rsa = CreateRsaFromPrivateKey(PrivateKeyPem);

        // 2) RSA decrypt
        byte[] innerBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.Pkcs1);
        string inner = Encoding.UTF8.GetString(innerBytes).Trim();

        // 3) Decode lớp trong
        return DecodeInner(inner);
    }

    private static int DecodeInner(string encoded)
    {
        string b64 = encoded.Replace('-', '+').Replace('_', '/');
        switch (b64.Length % 4)
        {
            case 2: b64 += "=="; break;
            case 3: b64 += "="; break;
        }

        var rawBytes = Convert.FromBase64String(b64);
        string raw = Encoding.UTF8.GetString(rawBytes);

        var parts = raw.Split(':');
        if (parts.Length != 2)
            throw new FormatException("Invalid Format");

        if (!long.TryParse(parts[1], out long mixed))
            throw new FormatException("Invalid mixed");

        long xoredLong = (mixed - NUM_B) / NUM_A;

        int id = (int)xoredLong ^ SECRET_XOR;

        return id;
    }

    /// <summary>
    /// Convert PEM PKCS#1 → RSAParameters → RSA
    /// </summary>
    private static RSA CreateRsaFromPrivateKey(string privateKeyPem)
    {
        var privateKey = DecodeRsaPrivateKeyPem(privateKeyPem);
        var rsa = RSA.Create();
        rsa.ImportParameters(privateKey);
        return rsa;
    }

    // Parse PKCS#1 PEM → RSAParameters
    private static RSAParameters DecodeRsaPrivateKeyPem(string pem)
    {
        var base64 = pem
            .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
            .Replace("-----END RSA PRIVATE KEY-----", "")
            .Replace("\n", "")
            .Replace("\r", "");

        var privateKeyBytes = Convert.FromBase64String(base64);
        return DecodeRsaPrivateKey(privateKeyBytes);
    }

    /// <summary>
    /// Decode PKCS#1 (DER) binary
    /// </summary>
    private static RSAParameters DecodeRsaPrivateKey(byte[] privkey)
    {
        using var mem = new System.IO.MemoryStream(privkey);
        using var binr = new System.IO.BinaryReader(mem);

        byte bt = 0;
        ushort twobytes = binr.ReadUInt16();
        if (twobytes == 0x8130) binr.ReadByte();
        else if (twobytes == 0x8230) binr.ReadInt16();
        else throw new Exception("Invalid PKCS#1 key");

        twobytes = binr.ReadUInt16();
        if (twobytes != 0x0102) throw new Exception("Invalid version");
        if (binr.ReadByte() != 0x00) throw new Exception("Invalid key");

        RSAParameters rsAparams = new RSAParameters();
        rsAparams.Modulus = ReadInteger(binr);
        rsAparams.Exponent = ReadInteger(binr);
        rsAparams.D = ReadInteger(binr);
        rsAparams.P = ReadInteger(binr);
        rsAparams.Q = ReadInteger(binr);
        rsAparams.DP = ReadInteger(binr);
        rsAparams.DQ = ReadInteger(binr);
        rsAparams.InverseQ = ReadInteger(binr);

        return rsAparams;
    }

    private static byte[] ReadInteger(System.IO.BinaryReader br)
    {
        if (br.ReadByte() != 0x02) throw new Exception("Invalid integer tag");

        int count = 0;
        byte bt = br.ReadByte();

        if (bt == 0x81) count = br.ReadByte();
        else if (bt == 0x82)
        {
            var highByte = br.ReadByte();
            var lowByte = br.ReadByte();
            count = BitConverter.ToUInt16(new[] { lowByte, highByte }, 0);
        }
        else count = bt;

        byte[] integer = br.ReadBytes(count);
        if (integer[0] == 0x00)
        {
            byte[] tmp = new byte[integer.Length - 1];
            Array.Copy(integer, 1, tmp, 0, tmp.Length);
            integer = tmp;
        }
        return integer;
    }
}
