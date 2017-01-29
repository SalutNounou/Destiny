using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destiny.Model
{
    public class Card
    {
        protected bool Equals(Card other)
        {
            return Suit == other.Suit && Rank == other.Rank;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Suit * 397) ^ (int)Rank;
            }
        }

        public CardSuit Suit { get; }
        public CardRank Rank { get; }
        public int Value { get { return (int)Suit * 13 + (int)Rank; } }

        public Card(CardRank rank, CardSuit suit)
        {
            Suit = suit;
            Rank = rank;
        }

        public ulong Mask { get { return 1UL << Value; } }



        public Card(int value)
        {
            if(value > 51) throw new ArgumentOutOfRangeException("value");
            Rank = (CardRank)(value % 13);
            Suit = (CardSuit)(value / 13);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Card)obj);
        }
    }
}
