#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentional. Will be by the exception handling agent.", Scope = "member", Target = "~M:Agents.Net.Agent.Execute(Agents.Net.Message)")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentional. Will be by the exception handling agent.", Scope = "member", Target = "~M:Agents.Net.InterceptorAgent.Intercept(Agents.Net.Message)~Agents.Net.InterceptionAction")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions must be catched, as a non catched exception would lead to an unspecific crash of the application.", Scope = "member", Target = "~M:Agents.Net.MessageBoard.Start")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Message is disposed automatically.", Scope = "member", Target = "~M:Agents.Net.Agent.Execute(Agents.Net.Message)")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Message is disposed automatically.", Scope = "member", Target = "~M:Agents.Net.Agent.OnMessages(System.Collections.Generic.IReadOnlyCollection{Agents.Net.Message},System.Boolean)")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Message is disposed automatically.", Scope = "member", Target = "~M:Agents.Net.InterceptorAgent.Intercept(Agents.Net.Message)~Agents.Net.InterceptionAction")]
[assembly: SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Explict conversion methods are necessary.", Scope = "member", Target = "~M:Agents.Net.MessageStore`1.FromMessageStore(Agents.Net.MessageStore`1)~`0")]
[assembly: SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Explict conversion methods are necessary.", Scope = "member", Target = "~M:Agents.Net.MessageStore`1.ToMessageStore(`0)~Agents.Net.MessageStore`1")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Message is disposed automatically.", Scope = "member", Target = "~M:Agents.Net.MessageBoard.Start")]
[assembly: SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Internally used protected method in public class.", Scope = "member", Target = "~M:Agents.Net.MessageCollector`2.IsCompleted(Agents.Net.MessageDomain,Agents.Net.MessageCollection@)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Internally used protected method in public class.", Scope = "member", Target = "~M:Agents.Net.MessageCollector`3.IsCompleted(Agents.Net.MessageDomain,Agents.Net.MessageCollection@)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Internally used protected method in public class.", Scope = "member", Target = "~M:Agents.Net.MessageCollector`4.IsCompleted(Agents.Net.MessageDomain,Agents.Net.MessageCollection@)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Internally used protected method in public class.", Scope = "member", Target = "~M:Agents.Net.MessageCollector`5.IsCompleted(Agents.Net.MessageDomain,Agents.Net.MessageCollection@)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Internally used protected method in public class.", Scope = "member", Target = "~M:Agents.Net.MessageCollector`6.IsCompleted(Agents.Net.MessageDomain,Agents.Net.MessageCollection@)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Internally used protected method in public class.", Scope = "member", Target = "~M:Agents.Net.MessageCollector`7.IsCompleted(Agents.Net.MessageDomain,Agents.Net.MessageCollection@)~System.Boolean")]
[assembly: SuppressMessage("Maintainability", "CA1501:Avoid excessive inheritance", Justification = "I assume this is common practise for this case.", Scope = "type", Target = "~T:Agents.Net.MessageCollection`7")]
