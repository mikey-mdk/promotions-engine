namespace PromotionsEngine.Application.Engines.Interfaces;

public interface IRegexEvaluationEngine
{
    /// <summary>
    /// Iterates through the provided regex list and evaluates the input against each regex.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="regexList"></param>
    /// <returns></returns>
    List<string> EvaluateRegexList(string input, List<string> regexList);
}