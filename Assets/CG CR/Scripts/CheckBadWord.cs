using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckBadWord {

    static List<string> badWords;

    public static bool HasBadWord(string userName) {

        userName = userName.ToLower();

        badWords = new List<string>();
        string[] lines = ((TextAsset)Resources.Load("badwords")).text.Split('\n', (char)System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
            badWords.Add(line.Remove(line.Length - 1));

        //if (badWords.Any(x => (userName).Contains(x)))
        //    return "INVALID ENTRY";

        for (int i = 0; i < badWords.Count; i++) {

            if (userName.Equals(badWords[i]))
                return true;

        }

        return false;

    }

}
