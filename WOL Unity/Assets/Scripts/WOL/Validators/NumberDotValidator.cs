using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "NumberDotValidator", menuName = "TextMeshPro/Input Validators/Numbers and Dot")]
public class NumberDotValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // Allow digits (0-9) and the decimal point (.)
        if (char.IsDigit(ch) || ch == '.')
        {
            // Ensure only one decimal point exists
            if (ch == '.' && text.Contains("."))
                return '\0'; // Reject additional dots

            text += ch;
            pos += 1;
            return ch;
        }
        return '\0'; // Reject invalid characters
    }
}
