using System;
using System.Diagnostics;
using System.Text;

namespace Destiny.Model.Evaluator
{
    public interface IHandEvaluator
    {
        uint EvaluateHand(ulong handMask, int cardsNumber);
        string DescriptionFromMask(ulong cards);
    }

    public class HandEvaluator : IHandEvaluator
    {

        public uint EvaluateHand(ulong handMask, int numberOfCards)
        {

#if DEBUG
            if (numberOfCards < 1 || numberOfCards > 7)
                throw new ArgumentOutOfRangeException("numberOfCards");
#endif
            uint handValue = 0, twoMask, threeMask;
            uint clubCards = (uint)((handMask >> (CardConstants.ClubOffset)) & 0x1fffUL);
            uint spadeCards = (uint)((handMask >> (CardConstants.SpadeOffset)) & 0x1fffUL);
            uint diamondCards = (uint)((handMask >> (CardConstants.DiamondOffset)) & 0x1fffUL);
            uint heartCards = (uint)((handMask >> (CardConstants.HeartOffset)) & 0x1fffUL);
            uint ranks = clubCards | spadeCards | diamondCards | heartCards;

            uint numberOfRanks = CardConstants.NumberOfBitsInMask[ranks];
            uint numberOfDuplicates = ((uint)(numberOfCards - numberOfRanks));

            /* Check for straight, flush, or straight flush, and return if we can
                          determine immediately that this is the best possible hand 
                       */
            if (numberOfRanks >= 5)
            {
                if (CardConstants.NumberOfBitsInMask[spadeCards] >= 5)
                {
                    if (CardConstants.StraightTable[spadeCards] != 0)
                        return CardConstants.ValueStraightFlush + (uint)(CardConstants.StraightTable[spadeCards] << CardConstants.TopCardShift);
                    else
                        handValue = CardConstants.ValueFlush + CardConstants.TopFiveCardsTable[spadeCards];
                }
                else if (CardConstants.NumberOfBitsInMask[clubCards] >= 5)
                {
                    if (CardConstants.StraightTable[clubCards] != 0)
                        return CardConstants.ValueStraightFlush + (uint)(CardConstants.StraightTable[clubCards] << CardConstants.TopCardShift);
                    else
                        handValue = CardConstants.ValueFlush + CardConstants.TopFiveCardsTable[clubCards];
                }
                else if (CardConstants.NumberOfBitsInMask[diamondCards] >= 5)
                {
                    if (CardConstants.StraightTable[diamondCards] != 0)
                        return CardConstants.ValueStraightFlush + (uint)(CardConstants.StraightTable[diamondCards] << CardConstants.TopCardShift);
                    else
                        handValue = CardConstants.ValueFlush + CardConstants.TopFiveCardsTable[diamondCards];
                }
                else if (CardConstants.NumberOfBitsInMask[heartCards] >= 5)
                {
                    if (CardConstants.StraightTable[heartCards] != 0)
                        return CardConstants.ValueStraightFlush + (uint)(CardConstants.StraightTable[heartCards] << CardConstants.TopCardShift);
                    else
                        handValue = CardConstants.ValueFlush + CardConstants.TopFiveCardsTable[heartCards];
                }
                else
                {
                    uint st = CardConstants.StraightTable[ranks];
                    if (st != 0)
                        handValue = CardConstants.ValueStraight + (st << CardConstants.TopCardShift);
                }

                /* 
                   Another win -- if there can't be a FH/Quads (n_dups < 3), 
                   which is true most of the time when there is a made hand, then if we've
                   found a five card hand, just return.  This skips the whole process of
                   computing two_mask/three_mask/etc.
                */
                if (handValue != 0 && numberOfDuplicates < 3)
                    return handValue;
            }

            /*
             * By the time we're here, either: 
               1) there's no five-card hand possible (flush or straight), or
               2) there's a flush or straight, but we know that there are enough
                  duplicates to make a full house / quads possible.  
             */
            switch (numberOfDuplicates)
            {
                case 0:
                    /* It's a no-pair hand */
                    return CardConstants.ValueHighCard + CardConstants.TopFiveCardsTable[ranks];
                case 1:
                    {
                        /* It's a one-pair hand */
                        uint t, kickers;

                        twoMask = ranks ^ (spadeCards ^ diamondCards ^ heartCards ^ clubCards);

                        handValue = (uint)(CardConstants.ValuePair + (CardConstants.TopCardTable[twoMask] << CardConstants.TopCardShift));
                        t = ranks ^ twoMask;      /* Only one bit set in two_mask */
                        /* Get the top five cards in what is left, drop all but the top three 
                         * cards, and shift them by one to get the three desired kickers */
                        kickers = (CardConstants.TopFiveCardsTable[t] >> CardConstants.CardWidth) & ~CardConstants.FifthCardMask;
                        handValue += kickers;
                        return handValue;
                    }

                case 2:
                    /* Either two pair or trips */
                    twoMask = ranks ^ (spadeCards ^ diamondCards ^ heartCards ^ clubCards);
                    if (twoMask != 0)
                    {
                        uint t = ranks ^ twoMask; /* Exactly two bits set in two_mask */
                        handValue = (uint)(CardConstants.ValueTwoPairs
                            + (CardConstants.TopFiveCardsTable[twoMask]
                            & (CardConstants.TopCardMAsk | CardConstants.SecondCardMask))
                            + (CardConstants.TopCardTable[t] << CardConstants.ThirdCardShift));

                        return handValue;
                    }
                    else
                    {
                        uint t, second;
                        threeMask = ((clubCards & diamondCards) | (heartCards & spadeCards)) & ((clubCards & heartCards) | (diamondCards & spadeCards));
                        handValue = (uint)(CardConstants.ValueTrips + (CardConstants.TopCardTable[threeMask] << CardConstants.TopCardShift));
                        t = ranks ^ threeMask; /* Only one bit set in three_mask */
                        second = CardConstants.TopCardTable[t];
                        handValue += (second << CardConstants.SecondCardShift);
                        t ^= (1U << (int)second);
                        handValue += (uint)(CardConstants.TopCardTable[t] << CardConstants.ThirdCardShift);
                        return handValue;
                    }

                default:
                    /* Possible quads, fullhouse, straight or flush, or two pair */
                    var fourMask = heartCards & diamondCards & clubCards & spadeCards;
                    if (fourMask != 0)
                    {
                        uint tc = CardConstants.TopCardTable[fourMask];
                        handValue = (uint)(CardConstants.ValueFourOfAKind
                            + (tc << CardConstants.TopCardShift)
                            + ((CardConstants.TopCardTable[ranks ^ (1U << (int)tc)]) << CardConstants.SecondCardShift));
                        return handValue;
                    }

                    /* Technically, three_mask as defined below is really the set of
                       bits which are set in three or four of the suits, but since
                       we've already eliminated quads, this is OK */
                    /* Similarly, two_mask is really two_or_four_mask, but since we've
                       already eliminated quads, we can use this shortcut */

                    twoMask = ranks ^ (clubCards ^ diamondCards ^ heartCards ^ spadeCards);
                    if (CardConstants.NumberOfBitsInMask[twoMask] != numberOfDuplicates)
                    {
                        /* Must be some trips then, which really means there is a 
                           full house since n_dups >= 3 */
                        uint tc, t;
                        threeMask = ((clubCards & diamondCards) | (heartCards & spadeCards)) & ((clubCards & heartCards) | (diamondCards & spadeCards));
                        handValue = CardConstants.ValueFullHouse;
                        tc = CardConstants.TopCardTable[threeMask];
                        handValue += (tc << CardConstants.TopCardShift);
                        t = (twoMask | threeMask) ^ (1U << (int)tc);
                        handValue += (uint)(CardConstants.TopCardTable[t] << CardConstants.SecondCardShift);
                        return handValue;
                    }

                    if (handValue != 0) /* flush and straight */
                        return handValue;
                    else
                    {
                        /* Must be two pair */
                        uint top, second;

                        handValue = CardConstants.ValueTwoPairs;
                        top = CardConstants.TopCardTable[twoMask];
                        handValue += (top << CardConstants.TopCardShift);
                        second = CardConstants.TopCardTable[twoMask ^ (1 << (int)top)];
                        handValue += (second << CardConstants.SecondCardShift);
                        handValue += (uint)((CardConstants.TopCardTable[ranks ^ (1U << (int)top) ^ (1 << (int)second)]) << CardConstants.ThirdCardShift);
                        return handValue;
                    }
            }
        }


        public string DescriptionFromMask(ulong cards)
        {
            int numberOfCards = BitCount(cards);

#if DEBUG
            // This functions supports 1-7 cards
            if (numberOfCards < 1 || numberOfCards > 7)
                throw new ArgumentOutOfRangeException("cards");
#endif
            // Seperate out by suit
            uint clubCards = (uint)((cards >> (CardConstants.ClubOffset)) & 0x1fffUL);
            uint diamondCards = (uint)((cards >> (CardConstants.DiamondOffset)) & 0x1fffUL);
            uint heartCards = (uint)((cards >> (CardConstants.HeartOffset)) & 0x1fffUL);
            uint spadeCards = (uint)((cards >> (CardConstants.SpadeOffset)) & 0x1fffUL);

            uint handvalue = EvaluateHand(cards, numberOfCards);

            switch ((HandTypes)HandType(handvalue))
            {
                case HandTypes.HighCard:
                case HandTypes.Pair:
                case HandTypes.TwoPair:
                case HandTypes.Trips:
                case HandTypes.Straight:
                case HandTypes.FullHouse:
                case HandTypes.FourOfAKind:
                    return DescriptionFromHandValueInternal(handvalue);
                case HandTypes.Flush:
                    if (CardConstants.NumberOfBitsInMask[spadeCards] >= 5)
                    {
                        return "Flush (Spades) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (CardConstants.NumberOfBitsInMask[clubCards] >= 5)
                    {
                        return "Flush (Clubs) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (CardConstants.NumberOfBitsInMask[diamondCards] >= 5)
                    {
                        return "Flush (Diamonds) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (CardConstants.NumberOfBitsInMask[heartCards] >= 5)
                    {
                        return "Flush (Hearts) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    break;
                case HandTypes.StraightFlush:
                    if (CardConstants.NumberOfBitsInMask[spadeCards] >= 5)
                    {
                        return "Straight Flush (Spades) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (CardConstants.NumberOfBitsInMask[clubCards] >= 5)
                    {
                        return "Straight (Clubs) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (CardConstants.NumberOfBitsInMask[diamondCards] >= 5)
                    {
                        return "Straight (Diamonds) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (CardConstants.NumberOfBitsInMask[heartCards] >= 5)
                    {
                        return "Straight  (Hearts) with " + Ranktbl[TopCard(handvalue)] + " high";
                    }
                    break;
            }
            Debug.Assert(false); // Should never get here
            return "";
        }

        private string DescriptionFromHandValueInternal(uint handValue)
        {
            StringBuilder description = new StringBuilder();

            switch ((HandTypes)HandType(handValue))
            {
                case HandTypes.HighCard:
                    description.Append("High card: ");
                    description.Append(Ranktbl[TopCard(handValue)]);
                    return description.ToString();
                case HandTypes.Pair:
                    description.Append("One pair, ");
                    description.Append(Ranktbl[TopCard(handValue)]);
                    return description.ToString();
                case HandTypes.TwoPair:
                    description.Append("Two pair, ");
                    description.Append(Ranktbl[TopCard(handValue)]);
                    description.Append("'s and ");
                    description.Append(Ranktbl[SecondCard(handValue)]);
                    description.Append("'s with a ");
                    description.Append(Ranktbl[ThirdCard(handValue)]);
                    description.Append(" for a kicker");
                    return description.ToString();
                case HandTypes.Trips:
                    description.Append("Three of a kind, ");
                    description.Append(Ranktbl[TopCard(handValue)]);
                    description.Append("'s");
                    return description.ToString();
                case HandTypes.Straight:
                    description.Append("A straight, ");
                    description.Append(Ranktbl[TopCard(handValue)]);
                    description.Append(" high");
                    return description.ToString();
                case HandTypes.Flush:
                    description.Append("A flush");
                    return description.ToString();
                case HandTypes.FullHouse:
                    description.Append("A fullhouse, ");
                    description.Append(Ranktbl[TopCard(handValue)]);
                    description.Append("'s and ");
                    description.Append(Ranktbl[SecondCard(handValue)]);
                    description.Append("'s");
                    return description.ToString();
                case HandTypes.FourOfAKind:
                    description.Append("Four of a kind, ");
                    description.Append(Ranktbl[TopCard(handValue)]);
                    description.Append("'s");
                    return description.ToString();
                case HandTypes.StraightFlush:
                    description.Append("A straight flush");
                    return description.ToString();
            }
            Debug.Assert(false); // Should never get here
            return "";
        }

        private static readonly string[] Ranktbl =
        {
            "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", "Ace",
            "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", "Ace",
            "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", "Ace",
            "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", "Ace"
        };

        public static int BitCount(ulong bitField)
        {
            return
                Bits[(int)(bitField & 0x00000000000000FFUL)] +
                Bits[(int)((bitField & 0x000000000000FF00UL) >> 8)] +
                Bits[(int)((bitField & 0x0000000000FF0000UL) >> 16)] +
                Bits[(int)((bitField & 0x00000000FF000000UL) >> 24)] +
                Bits[(int)((bitField & 0x000000FF00000000UL) >> 32)] +
                Bits[(int)((bitField & 0x0000FF0000000000UL) >> 40)] +
                Bits[(int)((bitField & 0x00FF000000000000UL) >> 48)] +
                Bits[(int)((bitField & 0xFF00000000000000UL) >> 56)];
        }

        private static readonly byte[] Bits =
        {
            0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,  /* 0   - 15  */
			1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,  /* 16  - 31  */
			1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,  /* 32  - 47  */
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,  /* 48  - 63  */
			1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,  /* 64  - 79  */
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,  /* 80  - 95  */
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,  /* 96  - 111 */
			3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,  /* 112 - 127 */
			1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,  /* 128 - 143 */
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,  /* 144 - 159 */
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,  /* 160 - 175 */
			3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,  /* 176 - 191 */
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,  /* 192 - 207 */
			3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,  /* 208 - 223 */
			3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,  /* 224 - 239 */
			4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8   /* 240 - 255 */
		};

        public uint TopCard(UInt32 hv)
        {
            return ((hv >> CardConstants.TopCardShift) & CardConstants.CardMask);
        }

        public uint HandType(uint handValue)
        {
            return (handValue >> CardConstants.HandTypeShift);
        }

        private uint SecondCard(UInt32 hv)
        {
            return (((hv) >> CardConstants.SecondCardShift) & CardConstants.CardMask);
        }

        /// <exclude/>
        private uint ThirdCard(UInt32 hv)
        {
            return (((hv) >> CardConstants.ThirdCardShift) & CardConstants.CardMask);
        }

        /// <exclude/>
        private uint FourthCard(UInt32 hv)
        {
            return (((hv) >> CardConstants.FourthCardShift) & CardConstants.CardMask);
        }

        /// <exclude/>
        private static uint FifthCard(System.UInt32 hv)
        {
            return (((hv) >> CardConstants.FifthCardShift) & CardConstants.CardMask);
        }

    }
}