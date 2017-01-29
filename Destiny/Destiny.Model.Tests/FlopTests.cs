using NUnit.Framework;

namespace Destiny.Model.Tests
{
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
}