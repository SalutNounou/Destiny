using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Destiny.Model.Tests
{
    public class CardTests
    {
        [TestCase(CardRank.Ace, CardSuit.Spades, 51)]
        [TestCase(CardRank.Two, CardSuit.Clubs, 0)]
        [TestCase(CardRank.Jack, CardSuit.Diamonds, 22)]
        [TestCase(CardRank.Eight, CardSuit.Hearts, 32)]
        [TestCase(CardRank.Three, CardSuit.Spades, 40)]

        public void Card_Should_have_correct_value(CardRank rank, CardSuit suit, int expected)
        {
            var card = new Card(rank, suit);
            Assert.AreEqual(card.Value, expected);
        }

        [TestCase(51, CardSuit.Spades, CardRank.Ace)]
        [TestCase(50, CardSuit.Spades, CardRank.King)]
        [TestCase(49, CardSuit.Spades, CardRank.Queen)]
        [TestCase(48, CardSuit.Spades, CardRank.Jack)]
        [TestCase(47, CardSuit.Spades, CardRank.Ten)]
        [TestCase(46, CardSuit.Spades, CardRank.Nine)]
        [TestCase(45, CardSuit.Spades, CardRank.Eight)]
        [TestCase(44, CardSuit.Spades, CardRank.Seven)]
        [TestCase(43, CardSuit.Spades, CardRank.Six)]
        [TestCase(42, CardSuit.Spades, CardRank.Five)]
        [TestCase(41, CardSuit.Spades, CardRank.Four)]
        [TestCase(40, CardSuit.Spades, CardRank.Three)]
        [TestCase(39, CardSuit.Spades, CardRank.Two)]
        [TestCase(38, CardSuit.Hearts, CardRank.Ace)]
        [TestCase(37, CardSuit.Hearts, CardRank.King)]
        [TestCase(36, CardSuit.Hearts, CardRank.Queen)]
        [TestCase(35, CardSuit.Hearts, CardRank.Jack)]
        [TestCase(34, CardSuit.Hearts, CardRank.Ten)]
        [TestCase(33, CardSuit.Hearts, CardRank.Nine)]
        [TestCase(32, CardSuit.Hearts, CardRank.Eight)]
        [TestCase(31, CardSuit.Hearts, CardRank.Seven)]
        [TestCase(30, CardSuit.Hearts, CardRank.Six)]
        [TestCase(29, CardSuit.Hearts, CardRank.Five)]
        [TestCase(28, CardSuit.Hearts, CardRank.Four)]
        [TestCase(27, CardSuit.Hearts, CardRank.Three)]
        [TestCase(26, CardSuit.Hearts, CardRank.Two)]
        [TestCase(25, CardSuit.Diamonds, CardRank.Ace)]
        [TestCase(24, CardSuit.Diamonds, CardRank.King)]
        [TestCase(23, CardSuit.Diamonds, CardRank.Queen)]
        [TestCase(22, CardSuit.Diamonds, CardRank.Jack)]
        [TestCase(21, CardSuit.Diamonds, CardRank.Ten)]
        [TestCase(20, CardSuit.Diamonds, CardRank.Nine)]
        [TestCase(19, CardSuit.Diamonds, CardRank.Eight)]
        [TestCase(18, CardSuit.Diamonds, CardRank.Seven)]
        [TestCase(17, CardSuit.Diamonds, CardRank.Six)]
        [TestCase(16, CardSuit.Diamonds, CardRank.Five)]
        [TestCase(15, CardSuit.Diamonds, CardRank.Four)]
        [TestCase(14, CardSuit.Diamonds, CardRank.Three)]
        [TestCase(13, CardSuit.Diamonds, CardRank.Two)]
        [TestCase(12, CardSuit.Clubs, CardRank.Ace)]
        [TestCase(11, CardSuit.Clubs, CardRank.King)]
        [TestCase(10, CardSuit.Clubs, CardRank.Queen)]
        [TestCase(9, CardSuit.Clubs, CardRank.Jack)]
        [TestCase(8, CardSuit.Clubs, CardRank.Ten)]
        [TestCase(7, CardSuit.Clubs, CardRank.Nine)]
        [TestCase(6, CardSuit.Clubs, CardRank.Eight)]
        [TestCase(5, CardSuit.Clubs, CardRank.Seven)]
        [TestCase(4, CardSuit.Clubs, CardRank.Six)]
        [TestCase(3, CardSuit.Clubs, CardRank.Five)]
        [TestCase(2, CardSuit.Clubs, CardRank.Four)]
        [TestCase(1, CardSuit.Clubs, CardRank.Three)]
        [TestCase(0, CardSuit.Clubs, CardRank.Two)]

        public void Card_should_be_built_correctly_from_value(int value, CardSuit expectedSuit, CardRank expectedRank)
        {
            var card = new Card(value);
            Assert.AreEqual(card.Rank, expectedRank);
            Assert.AreEqual(card.Suit, expectedSuit);
        }


        [TestCase(CardRank.Three, CardSuit.Clubs, (ulong)2)]
        [TestCase(CardRank.Ace, CardSuit.Spades, (ulong)0x8000000000000)]
        public void Card_Should_Return_Correct_Mask(CardRank rank, CardSuit suit, ulong expectedMask)
        {
            var card = new Card(rank, suit);
            Assert.AreEqual(card.Mask, expectedMask);
        }

    }

    public class FlopTests
    {
        [Test]
        public void Should_tell_if_flop_is_valid()
        {
            var card1 = new Card(35);
            var card2 = new Card(22);
            var card3 = new Card(51);
            var flop = new Flop(card1, card2, card3);
            Assert.AreEqual(true, flop.IsValid);
            flop = new Flop(card1, card2, card2);
            Assert.AreEqual(false, flop.IsValid);
        }
    }


    public class HandTests
    {
        [TestCase(true,2,6,45,9,16,38,8)]
        [TestCase(true, 2, 8, 45, 9, 16, 38, 8)]
        public void Should_Tell_If_Hand_Is_Valid(bool expected, int card1, int card2, int card3, int card4, int card5, int card6, int card7)
        {
            var hand = new Hand(new Card(card1), new Card(card2),
                new Flop(new Card(card3), new Card(card4), new Card(card5)), new Card(card6), new Card(card7));
            Assert.AreEqual(expected, hand.IsValid);
        }
    }
}
