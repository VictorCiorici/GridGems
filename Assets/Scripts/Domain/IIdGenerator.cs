namespace GridGame.Domain
{
    /// <summary>
    /// Generates unique identifiers for domain entities.
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>Returns a new unique string identifier.</summary>
        string NewId();
    }
}
