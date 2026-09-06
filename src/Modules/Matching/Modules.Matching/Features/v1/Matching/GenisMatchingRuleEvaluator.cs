namespace FSH.Modules.Matching.Features.v1.Matching;

internal static class GenisMatchingRuleEvaluator
{
    private static readonly Dictionary<string, int> StringencyOrder =
        new(StringComparer.Ordinal)
        {
            ["ImpossibleMatch"] = 0,
            ["HighStringency"] = 1,
            ["ModerateStringency"] = 2,
            ["LowStringency"] = 3,
            ["Mismatch"] = 4,
            ["NoMatch"] = 5,
        };

    internal static (int RuleMismatches, int QualifiedLoci, bool Qualified) Evaluate(
        IReadOnlyDictionary<string, string> detailed,
        string minimumStringency,
        int mismatchsAllowed,
        int minLocusMatch)
    {
        ArgumentNullException.ThrowIfNull(detailed);
        ArgumentException.ThrowIfNullOrWhiteSpace(minimumStringency);

        int minimum = Rank(minimumStringency);
        int ruleMismatches = 0;
        int qualifiedLoci = 0;

        foreach (string value in detailed.Values)
        {
            int rank = Rank(value);
            bool isMismatch = rank > minimum;
            if (isMismatch)
            {
                ruleMismatches++;
                continue;
            }

            if (rank > Rank("ImpossibleMatch") && rank < Rank("Mismatch"))
            {
                qualifiedLoci++;
            }
        }

        return (
            ruleMismatches,
            qualifiedLoci,
            ruleMismatches <= mismatchsAllowed && qualifiedLoci >= minLocusMatch);
    }

    internal static bool IsValidStringency(string value) => StringencyOrder.ContainsKey(value);

    private static int Rank(string value)
    {
        if (!StringencyOrder.TryGetValue(value, out int rank))
        {
            throw new InvalidOperationException($"Unsupported GENis stringency '{value}'.");
        }

        return rank;
    }
}
