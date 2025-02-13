using Microsoft.Azure.Cosmos;

namespace PromotionsEngine.Tests.Infrastructure.EqualityComparers;

public class PatchOperationsEqualityComparer : IEqualityComparer<PatchOperation>, IEqualityComparer<List<PatchOperation>>
{
    public bool Equals(PatchOperation? x, PatchOperation? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.OperationType == y.OperationType && x.Path == y.Path && x.From == y.From;
    }

    public int GetHashCode(PatchOperation obj)
    {
        return HashCode.Combine((int)obj.OperationType, obj.Path, obj.From);
    }

    public bool Equals(List<PatchOperation>? x, List<PatchOperation>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        if (x.Capacity != y.Capacity) return false;
        return x.Count == y.Count && x.SequenceEqual(y, this);
    }

    public int GetHashCode(List<PatchOperation> obj)
    {
        return HashCode.Combine(obj.Capacity, obj.Count);
    }
}