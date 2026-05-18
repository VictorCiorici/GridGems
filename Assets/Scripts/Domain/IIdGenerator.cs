namespace GridGame.Domain
{
    /// <summary>
    /// Generates unique identifiers for domain entities.
    /// Inject a concrete implementation to keep ID strategy testable and swappable.
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>Returns a new unique string identifier.</summary>
        string NewId();
    }
}
