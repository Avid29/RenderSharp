// Adam Dernis 2023

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace RenderSharp.ImportExport.WaveFront;

public partial class WaveFrontImporter
{
    private const string fileRegex = @"([\.\w]*)(\.\w*)";

    private string _objFileName;
    private string? _mtlFileName;

    public WaveFrontImporter(string objFileName, string? mtlFileName = null, bool useMtl = true)
    {
        _objFileName = objFileName;

        if (useMtl)
        {
            mtlFileName ??= ;
        }
    }

    private bool ValidateFile(string fileName, out string? rawName, out string? extension)
    {
        rawName = null;
        extension = null;

        var match = Regex.Match(fileName, fileRegex);
        
        if (!match.Success)
            return false;

        rawName = match.Groups[1].Value;
        extension = match.Groups[2].Value;

        return true;
    }
}
