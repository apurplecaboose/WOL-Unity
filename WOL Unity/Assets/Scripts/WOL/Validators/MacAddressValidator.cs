using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "MacAddressValidator", menuName = "TextMeshPro/Input Validators/MAC Address")]
public class MacAddressValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // Allow only hexadecimal characters and colons
        if (char.IsDigit(ch) || "ABCDEFabcdef".Contains(ch) || ch == ':')
        {
            text += ch;
            pos += 1;
            return ch;
        }
        return '\0'; // Reject all other characters
    }
}
