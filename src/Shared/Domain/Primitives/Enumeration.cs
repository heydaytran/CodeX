﻿namespace Domain.Primitives;

/// <summary>
/// Represents an enumeration of objects with a unique numeric identifier and a name.
/// </summary>
/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<Dictionary<int, TEnum>> EnumerationsDictionary = new(() => CreateEnumerationDictionary(typeof(TEnum)));

    protected Enumeration(int id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    protected Enumeration() => Name = string.Empty;

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public int Id { get; protected init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; protected init; }

    public static bool operator ==(Enumeration<TEnum>? a, Enumeration<TEnum>? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Enumeration<TEnum> a, Enumeration<TEnum> b) => !(a == b);

    /// <summary>
    /// Gets the enumeration values.
    /// </summary>
    /// <returns>The read-only collection of enumeration values.</returns>
    public static IReadOnlyCollection<TEnum> GetValues() => [.. EnumerationsDictionary.Value.Values];

    /// <summary>
    /// Creates an enumeration of the specified type based on the specified identifier.
    /// </summary>
    /// <param name="id">The enumeration identifier.</param>
    /// <returns>The enumeration instance that matches the specified identifier, if it exists.</returns>
    public static TEnum? FromId(int? id) => id is not null && EnumerationsDictionary.Value.TryGetValue(id.Value, out TEnum? enumeration) ? enumeration : null;

    /// <summary>
    /// Creates an enumeration of the specified type based on the specified name.
    /// </summary>
    /// <param name="name">The enumeration name.</param>
    /// <returns>The enumeration instance that matches the specified name, if it exists.</returns>
    public static TEnum? FromName(string name) => EnumerationsDictionary.Value.Values.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Checks if the enumeration with the specified identifier exists.
    /// </summary>
    /// <param name="id">The enumeration identifier.</param>
    /// <returns>True if an enumeration with the specified identifier exists, otherwise false.</returns>
    public static bool Contains(int id) => EnumerationsDictionary.Value.ContainsKey(id);

    public static implicit operator int(Enumeration<TEnum> e) => e.Id;

    /// <inheritdoc />
    public virtual bool Equals(Enumeration<TEnum>? other)
    {
        if (other is null)
        {
            return false;
        }

        return GetType() == other.GetType() && other.Id.Equals(Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        return obj is Enumeration<TEnum> otherValue && otherValue.Id.Equals(Id);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode() * 37;

    public override string ToString() => $"{Name}({Id})";

    private static Dictionary<int, TEnum> CreateEnumerationDictionary(Type enumType) => GetFieldsForType(enumType).ToDictionary(t => t.Id);

    private static IEnumerable<TEnum> GetFieldsForType(Type enumType) =>
        enumType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => enumType.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);
}
