namespace Destiny.Model.Evaluator
{
    public enum HandTypes
    {
        /// <summary>
        /// Only a high card
        /// </summary>
        HighCard = 0,
        /// <summary>
        /// One Pair
        /// </summary>
        Pair = 1,
        /// <summary>
        /// Two Pair
        /// </summary>
        TwoPair = 2,
        /// <summary>
        /// Three of a kind (Trips)
        /// </summary>
        Trips = 3,
        /// <summary>
        /// Straight
        /// </summary>
        Straight = 4,
        /// <summary>
        /// Flush
        /// </summary>
        Flush = 5,
        /// <summary>
        /// FullHouse
        /// </summary>
        FullHouse = 6,
        /// <summary>
        /// Four of a kind
        /// </summary>
        FourOfAKind = 7,
        /// <summary>
        /// Straight Flush
        /// </summary>
        StraightFlush = 8
    }
}