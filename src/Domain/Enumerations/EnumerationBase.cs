using System.Reflection;

namespace PromotionsEngine.Domain.Enumerations;

public abstract class EnumerationBase : IComparable
{
    public string Name { get; }

    public int Id { get; }

    protected EnumerationBase(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : EnumerationBase =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj is not EnumerationBase otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Id);
    }

    public int CompareTo(object? obj)
    {
        return obj != null ? Id.CompareTo(((EnumerationBase)obj).Id) : 1; //all instances are greater than null
    }
}