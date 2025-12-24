namespace GameFrameworks.StatSystem.Core;

/// <summary>
///     Interface that is used in <see cref="IStatContainer{TStatDefinition, TNumber}"/> simulations to make copies of entities
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICreateCopy<T>
{
    /// <summary>
    ///     Creates a copy of an object
    /// </summary>
    /// <returns>New instance of <see cref="T"/> object</returns>
    T CreateCopy();
}
