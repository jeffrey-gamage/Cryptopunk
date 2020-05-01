using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FileReaderUtils
{
    public static string GetNextLine(ref string textBody)
    {
            int indexOfNextNewline = textBody.IndexOf("\n");
            string nextLine;
            if (indexOfNextNewline < 0)
            {
                nextLine = "++ENDFILE++";
            }
            else
            {
                nextLine = textBody.Substring(0, indexOfNextNewline - 1);
            }
            textBody = textBody.Substring(indexOfNextNewline + 1);
            return nextLine;
    }
}
