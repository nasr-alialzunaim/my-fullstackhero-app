namespace DNationalSystem.DbMigrator;

internal sealed record MigratorCommand(
    string Command,
    bool SeedAfter,
    bool Help)
{
    private static readonly string[] KnownVerbs = ["apply", "seed"];

    public static MigratorCommand Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var rawVerb = args.FirstOrDefault(a => !a.StartsWith('-')) ?? "apply";
        var verb = KnownVerbs.FirstOrDefault(v =>
            string.Equals(v, rawVerb, StringComparison.OrdinalIgnoreCase)) ?? rawVerb;

        var seedAfter = args.Any(a =>
            string.Equals(a, "--seed", StringComparison.OrdinalIgnoreCase));
        var help = args.Any(a => a is "-h" or "--help");

        return new MigratorCommand(verb, seedAfter, help);
    }

    public const string HelpText = """
        DNationalSystem DbMigrator — single-installation database migration tool.

        Usage:
          dotnet run --project src/Host/DNationalSystem.DbMigrator -- [verb] [options]

        Verbs:
          apply           Apply pending migrations for every registered module (default).
          seed            Run idempotent installation seed data.

        Options:
          --seed          After apply, also run each module's seed step.
          -h, --help      Print this help text.

        Exit codes:
          0 — success
          1 — failure
        """;
}
