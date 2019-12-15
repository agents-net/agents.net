#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Agents.Net
{
    public abstract class DecoratorAgent : Agent
    {
        public DecoratorAgentDefinition DecoratorDefinition { get; }

        protected DecoratorAgent(DecoratorAgentDefinition decoratorDefinition, MessageBoard messageBoard) : base(decoratorDefinition, messageBoard)
        {
            DecoratorDefinition = decoratorDefinition;
        }
    }
}
