namespace GridGame.Domain
{
    /// <summary>
    /// The result of a <see cref="GridSystem.TryPlaceGem"/> call.
    /// Replaces nullable return values with a self-documenting result type.
    /// </summary>
    public readonly struct PlacementResult
    {
        /// <summary>Whether the gem was placed successfully.</summary>
        public bool Success { get; }

        /// <summary>The placed gem. <c>null</c> when <see cref="Success"/> is <c>false</c>.</summary>
        public GemEntity Gem { get; }

        /// <summary>Human-readable reason for failure. <c>null</c> when <see cref="Success"/> is <c>true</c>.</summary>
        public string FailureReason { get; }

        private PlacementResult(bool success, GemEntity gem, string failureReason)
        {
            Success = success;
            Gem = gem;
            FailureReason = failureReason;
        }

        /// <summary>Creates a successful placement result.</summary>
        public static PlacementResult Ok(GemEntity gem) =>
            new PlacementResult(true, gem, null);

        /// <summary>Creates a failed placement result with an explanatory reason.</summary>
        public static PlacementResult Fail(string reason) =>
            new PlacementResult(false, null, reason);
    }
}
