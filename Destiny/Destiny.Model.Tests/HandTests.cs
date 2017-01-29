using NUnit.Framework;

namespace Destiny.Model.Tests
{
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