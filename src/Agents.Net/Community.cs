#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Linq;

namespace Agents.Net
{
    /**
     * Properties of the swarm
     *  + historic debugging by visualizing the log, which contains all messages and their predecessors
     *  + implicit parallelisation of agents - when the requirements of two agents are filled they are executed in parallel
     *  + scalable - work can be executed in parallel without the need for any agent to know about it except one how does the parallelisation
     *  + easy to implement global undo by reversing all messages
     *  + transactions are a domain (?) with explicit start and end
    **/
    public class Community
    {
        private readonly MessageBoard messageBoard;

        public Community(MessageBoard messageBoard)
        {
            this.messageBoard = messageBoard;
        }

        public void RegisterAgents(params Agent[] agents)
        {
            foreach (Agent agent in agents)
            {
                foreach (MessageDefinition trigger in agent.Definition.ConsumingTriggers)
                {
                    messageBoard.Register(trigger, agent);
                }
            }

            foreach (InterceptorAgent interceptorAgent in agents.OfType<InterceptorAgent>())
            {
                foreach (MessageDefinition interceptedMessage in interceptorAgent.InterceptorDefinition.InterceptedMessages)
                {
                    messageBoard.RegisterInterceptor(interceptedMessage, interceptorAgent);
                }
            }
        }
    }
}
