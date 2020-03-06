﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentional. Will be by the exception handling agent.", Scope = "member", Target = "~M:Agents.Net.Agent.Execute(Agents.Net.Message)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentional. Will be by the exception handling agent.", Scope = "member", Target = "~M:Agents.Net.InterceptorAgent.Intercept(Agents.Net.Message)~Agents.Net.InterceptionAction")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions must be catched, as a non catched exception would lead to an unspecific crash of the application.", Scope = "member", Target = "~M:Agents.Net.MessageBoard.Start")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not possible to check without C# 7.0.", Scope = "member", Target = "~M:Agents.Net.MessageDecorator.#ctor(Agents.Net.Message,Agents.Net.MessageDefinition,System.Collections.Generic.IEnumerable{Agents.Net.Message})")]
