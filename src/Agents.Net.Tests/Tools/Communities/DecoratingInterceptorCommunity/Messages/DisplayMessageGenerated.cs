using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Messages
{
    public class DisplayMessageGenerated : Message
    {
        public DisplayMessageGenerated(string displayMessage, Message predecessorMessage,
                                       params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            DisplayMessage = displayMessage;
        }

        public DisplayMessageGenerated(string displayMessage, IEnumerable<Message> predecessorMessages,
                                       params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
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
