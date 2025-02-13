using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using PromotionsEngine.Application.Engines.Interfaces;

namespace PromotionsEngine.Application.Engines.Implementations;

[ExcludeFromCodeCoverage(Justification = "Simple implementation of regex matching that does not require testing.")]
public class RegexEvaluationEngine : IRegexEvaluationEngine
{
    public List<string> EvaluateRegexList(string input, List<string> regexList)
    {
        return regexList.Where(pattern =>
            Regex.IsMatch(input, pattern, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1))).ToList();
    }
}