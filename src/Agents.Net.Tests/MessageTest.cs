using FluentAssertions;
using NUnit.Framework;

namespace Agents.Net.Tests
{
    public class MessageTest
    {
        [Test]
        public void ReplaceMessageInheritsPredecessorIds()
        {
            TestMessage origin = new TestMessage();
            TestMessage context = new TestMessage(origin);
            TestMessage replacing = new TestMessage();
            context.ReplaceWith(replacing);

            replacing.ToMessageLog().Predecessors.Should().BeEquivalentTo(context.ToMessageLog().Predecessors);
        }
        
        [Test]
        public void ReplaceMessageInheritsChild()
        {
            TestMessage child = new TestMessage();
            TestMessageDecorator context = TestMessageDecorator.Decorate(child);
            TestMessageDecorator replacing = TestMessageDecorator.Decorate(null);
            context.ReplaceWith(replacing);

            replacing.ToMessageLog().Child.Id.Should().Be(child.Id);
        }
        
        [Test]
        public void ReplaceMessageInheritsMessageDomain()
        {
            TestMessage context = new TestMessage();
            MessageDomain.CreateNewDomainsFor(context);
            TestMessage replacing = new TestMessage();
            context.ReplaceWith(replacing);

            replacing.ToMessageLog().Domain.Should().Be(context.ToMessageLog().Domain);
        }
        
        [Test]
        public void ReplaceMessageReplacesParentConnection()
        {
            TestMessage context = new TestMessage();
            TestMessageDecorator decorator = TestMessageDecorator.Decorate(context);
            TestMessage replacing = new TestMessage();
            context.ReplaceWith(replacing);

            replacing.HeadMessage.Should().Be(decorator);
            decorator.DescendantsAndSelf.Should().Contain(replacing);
            foreach (Message descendant in decorator.DescendantsAndSelf)
            {
                descendant.Should().NotBeSameAs(context);
            }
        }
        
        [Test]
        public void ReplaceMessageInheritsMessageId()
        {
            TestMessage context = new TestMessage();
            TestMessage replacing = new TestMessage();
            context.ReplaceWith(replacing);

            replacing.Id.Should().Be(context.Id);
        }
    }
}