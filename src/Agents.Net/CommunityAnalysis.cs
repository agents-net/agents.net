#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Agents.Net
{
    public static class CommunityAnalysis
    {
        public static AnalysisResult Analyse(Assembly[] assemblies, Agent[] agents = null)
        {
            ICollection<AgentDefinition> agentDefinitions = MineAgentDefinitions(assemblies, agents,
                                                                           out IEnumerable<Type> agentTypeWithoutDefinition,
                                                                           out IEnumerable<Type> notInitialized);
            ICollection<MessageDefinition> messageDefinitions = MineMessageDefinitions(assemblies, out IEnumerable<Type> messageTypeWithoutDefinition);
            IEnumerable<MessageDefinition> unusedMessageDefinitions = messageDefinitions
                                                                     .Where(m => !agentDefinitions.Any(
                                                                                     b => b.ConsumingTriggers
                                                                                           .Concat(b.ProducingTriggers)
                                                                                           .Concat(agentDefinitions.OfType<InterceptorAgentDefinition>()
                                                                                                                 .SelectMany(d => d.InterceptedMessages))
                                                                                           .Any(t => t == m)));
            MessageDefinition[] producedTriggers = agentDefinitions.SelectMany(b => b.ProducingTriggers)
                                                              .ToArray();
            MessageDefinition[] consumedTriggers = agentDefinitions.SelectMany(b => b.ConsumingTriggers)
                                                                 .Concat(agentDefinitions.OfType<InterceptorAgentDefinition>()
                                                                                       .SelectMany(d => d.InterceptedMessages))
                                                              .ToArray();
            IEnumerable<MessageDefinition> unproducedMessageTriggers = consumedTriggers.Except(producedTriggers)
                                                                                    .Except(new []{InitializeMessage.InitializeMessageDefinition, });
            IEnumerable<MessageDefinition> unconsumedMessageTriggers = producedTriggers.Except(consumedTriggers);
            return new AnalysisResult(unusedMessageDefinitions, unproducedMessageTriggers, unconsumedMessageTriggers,
                                      messageTypeWithoutDefinition, agentTypeWithoutDefinition,
                                      agentDefinitions.Count, messageDefinitions.Count,
                                      notInitialized);
        }

        private static ICollection<MessageDefinition> MineMessageDefinitions(
            Assembly[] assemblies, out IEnumerable<Type> messageTypeWithoutDefinition)
        {
            IEnumerable<Type> messages = assemblies.SelectMany(a => a.GetTypes())
                                                   .Where(t => t.BaseType == typeof(Message) || t.BaseType == typeof(MessageDecorator))
                                                   .Where(t => !t.IsAbstract)
                                                   .Where(t => t.Name != "DefaultMessageDomainMessage");
            List<Type> typeWithoutDefinition = new List<Type>();
            List<MessageDefinition> definitions = new List<MessageDefinition>();
            foreach (Type message in messages)
            {
                PropertyInfo definition = message
                                         .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                         .FirstOrDefault(p => p.GetCustomAttribute<MessageDefinitionAttribute>() != null);
                if (definition?.GetValue(null) is MessageDefinition messageDefinition)
                {
                    definitions.Add(messageDefinition);
                }
                else
                {
                    typeWithoutDefinition.Add(message);
                }
            }

            messageTypeWithoutDefinition = typeWithoutDefinition;
            return definitions;
        }

        private static ICollection<AgentDefinition> MineAgentDefinitions(Assembly[] assemblies, Agent[] actualAgents,
                                                                     out IEnumerable<Type> agentTypeWithoutDefinition,
                                                                     out IEnumerable<Type> notInitializedAgents)
        {
            IEnumerable<Type> agents = assemblies.SelectMany(a => a.GetTypes())
                                                   .Where(t => t.BaseType == typeof(Agent) || t.BaseType == typeof(InterceptorAgent))
                                                   .Where(t => !t.IsAbstract);
            List<Type> typeWithoutDefinition = new List<Type>();
            List<AgentDefinition> definitions = new List<AgentDefinition>();
            List<Type> notInitialized = new List<Type>();
            foreach (Type agent in agents)
            {
                PropertyInfo definition = agent
                                         .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                         .FirstOrDefault(p => p.GetCustomAttribute<AgentDefinitionAttribute>() != null);
                if (definition?.GetValue(null) is AgentDefinition agentDefinition)
                {
                    definitions.Add(agentDefinition);
                }
                else
                {
                    typeWithoutDefinition.Add(agent);
                }

                if (actualAgents != null && !actualAgents.Any(b => agent.IsInstanceOfType(b)))
                {
                    notInitialized.Add(agent);
                }
            }

            agentTypeWithoutDefinition = typeWithoutDefinition;
            notInitializedAgents = notInitialized;
            return definitions;
        }

        public class AnalysisResult
        {
            public AnalysisResult(IEnumerable<MessageDefinition> unusedMessageDefinitions,
                                  IEnumerable<MessageDefinition> unproducedMessageTrigger,
                                  IEnumerable<MessageDefinition> unconsumedMessageTrigger,
                                  IEnumerable<Type> messageTypeWithoutDefinition,
                                  IEnumerable<Type> agentWithoutDefinition,
                                  int analysedAgentCount,
                                  int analysedMessageCount, 
                                  IEnumerable<Type> notInitializedAgents)
            {
                UnusedMessageDefinitions = unusedMessageDefinitions;
                UnproducedMessageTrigger = unproducedMessageTrigger;
                UnconsumedMessageTrigger = unconsumedMessageTrigger;
                MessageTypeWithoutDefinition = messageTypeWithoutDefinition;
                AgentWithoutDefinition = agentWithoutDefinition;
                AnalysedAgentCount = analysedAgentCount;
                AnalysedMessageCount = analysedMessageCount;
                NotInitializedAgents = notInitializedAgents;
            }

            public IEnumerable<MessageDefinition> UnusedMessageDefinitions { get; }
            public IEnumerable<MessageDefinition> UnproducedMessageTrigger { get; }
            public IEnumerable<MessageDefinition> UnconsumedMessageTrigger { get; }
            public IEnumerable<Type> MessageTypeWithoutDefinition { get; }
            public IEnumerable<Type> AgentWithoutDefinition { get; }
            public IEnumerable<Type> NotInitializedAgents { get; }
            public int AnalysedAgentCount { get; }
            public int AnalysedMessageCount { get; }
        }
    }
}
