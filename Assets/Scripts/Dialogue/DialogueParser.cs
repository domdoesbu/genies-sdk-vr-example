using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public static class DialogueParser
{
    public static string Parse(string text, DialogueVariables vars, bool pluralVerbage)
    {
        var replacements = new Dictionary<string, string>()
        {
            // Name
            {"{playerName}", vars.playerName },
            // Pronouns
            {"{subjectivePronoun}", vars.subjectivePronoun },
            {"{SubjectivePronoun}", char.ToUpper(vars.subjectivePronoun[0]) + vars.subjectivePronoun.Substring(1) },
            {"{objectivePronoun}", vars.objectivePronoun  },
            {"posessiveAdjective}", vars.possesiveAdjectives },
            {"{posessivePronoun}", vars.possesivePronoun },
            {"{reflexivePronoun}", vars.reflexivePronoun },
            // Gendered term
            {"{person}", vars.genderedTerm },

            {"{sassy}", vars.sassy },
            {"{polite}", vars.polite },
            // Verbs
            {"{be}", pluralVerbage ? "are" : "is" },
            {"{have}", pluralVerbage ? "have" : "has"},
            {"{do}", pluralVerbage ? "do" : "does" },
            {"{seek}", pluralVerbage ? "seeks" : "seek" }
        };

        foreach (var pair in replacements)
        {
            UnityEngine.Debug.Log(text);
            text = text.Replace(pair.Key, pair.Value);
            UnityEngine.Debug.Log(text);
        }

        return text;
    }
}
