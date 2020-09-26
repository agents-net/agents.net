using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Messages
{
    public class DisplayMessageGenerated : Message
    {
        public DisplayMessageGenerated(string displayMessage, Message predecessorMessage)
			: base(predecessorMessage)
        {
            DisplayMessage = displayMessage;
        }

        public DisplayMessageGenerated(string displayMessage, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            DisplayMessage = displayMessage;
        }

        public string DisplayMessage { get; }

        protected override string DataToString()
        {
            return $"{nameof(DisplayMessage)}: {DisplayMessage}";
        }
    }
}
