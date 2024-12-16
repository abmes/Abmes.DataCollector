using System.Collections;

namespace Abmes.Utils.Net;

public class IpAddressUtils : IIpAddressUtils
{
    public bool IpAddressMatchesPattern(string ipString, string ipPattern)
    {
        ArgumentException.ThrowIfNullOrEmpty(ipString);
        ArgumentException.ThrowIfNullOrEmpty(ipPattern);

        var patternParts = ipPattern.Split('/');

        var patternBits = GetIpAddressBits(patternParts[0]);
        var ipBits = GetIpAddressBits(ipString);

        var maskBitCount =
            patternParts.Length == 2
            ? int.Parse(patternParts[1])
            : patternBits.Length;

        var maskBits = new BitArray(maskBitCount, true);

        if ((patternBits.Length != ipBits.Length) || (maskBits.Length > ipBits.Length))
        {
            return false;
        }

        maskBits.Length = patternBits.Length;

        var patternCompareBits = patternBits.And(maskBits);
        var ipCompareBits = ipBits.And(maskBits);

        var xor = patternCompareBits.Xor(ipCompareBits);

        return !(xor.Cast<bool>().Any(x => x));
    }

    private static BitArray GetIpAddressBits(string ipString)
    {
        var a = System.Net.IPAddress.Parse(ipString);
        var bytes = a.GetAddressBytes();
        return new BitArray(bytes);
    }
}
