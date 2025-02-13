using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.Commands;

[ExcludeFromCodeCoverage]
public abstract class CommandBase
{
    public abstract string TransactionType { get; }
}