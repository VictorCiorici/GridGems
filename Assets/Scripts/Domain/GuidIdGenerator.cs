using System;

namespace GridGame.Domain
{
    /// <summary>
    /// Default <see cref="IIdGenerator"/> implementation using <see cref="Guid"/>.
    /// </summary>
    public sealed class GuidIdGenerator : IIdGenerator
    {
        /// <inheritdoc/>
        public string NewId() => Guid.NewGuid().ToString();
    }
}
