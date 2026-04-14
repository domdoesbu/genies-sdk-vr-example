using System.Collections.Generic;

public static class DialogueParser
{
    public static string Parse(string text, DialogueVariables vars, bool pluralVerbage)
    {
        var replacements = new Dictionary<string, string>()
        {
            // Name
            {"{playerName}", vars.playerName },
            // Pronouns
            {"{subjectPronoun}", vars.subjectPronoun },
            {"{objectPronoun}", vars.objectPronoun  },
            {"{posessivePronoun}", vars.possesivePronoun },
            // Gendered term
            {"{genderedTerm}", vars.genderedTerm },
            // Verbs
            {"{be}", pluralVerbage ? "are" : "is" },
            {"{have}", pluralVerbage ? "have" : "has"},
            {"{do}", pluralVerbage ? "do" : "does" }
        };

        foreach (var pair in replacements)
        {
            text = text.Replace(pair.Key, pair.Value);
        }

        return text;
    }
}
