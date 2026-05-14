// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.Samples;

/// <summary>
/// Provides shared input and output path helpers for sample projects.
/// </summary>
internal static partial class SamplePaths
{
    private const string SolutionFileName = "SixLabors.Samples.slnx";

    /// <summary>
    /// Gets the repository root used for shared sample output.
    /// </summary>
    public static string RepositoryDirectory { get; } = FindRepositoryDirectory();

    /// <summary>
    /// Gets the directory containing sample assets for the current project.
    /// </summary>
    public static string AssetDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "assets", AssetFolderName);

    /// <summary>
    /// Gets the directory where the current project writes generated output.
    /// </summary>
    public static string OutputDirectory { get; } = Path.Combine(RepositoryDirectory, "output", OutputFolderName);

    /// <summary>
    /// Gets the full path to one sample asset.
    /// </summary>
    /// <param name="fileName">The asset file name.</param>
    /// <returns>The full path to the sample asset.</returns>
    public static string Asset(string fileName) => Path.Combine(AssetDirectory, fileName);

    /// <summary>
    /// Gets the full path for one generated output file.
    /// </summary>
    /// <param name="fileName">The output file name.</param>
    /// <returns>The full output path.</returns>
    public static string Output(string fileName) => Path.Combine(OutputDirectory, fileName);

    private static string FindRepositoryDirectory()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            // Build output lives under artifacts/bin, so walk upward until the solution file
            // identifies the sample repository root rather than relying on the launch directory.
            if (File.Exists(Path.Combine(directory.FullName, SolutionFileName)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Directory.GetCurrentDirectory();
    }
}
