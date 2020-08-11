Feature: CoreUseCases
	This feature is at the same time a test of the core functionality as well
	as a documentation of possible use cases on hwo to use the agent framework.
	Each test will start with 'Given I have loaded the community "<name>"'.
	The loaded community can be found in the directory 
	"<ProjectDirectory>/Tools/Communities/<name>". There is the Agents.Net
	Designer model (.amodel), a svg visualization of the community as well as
	the actual implementation.

	Not Implemented Use Cases:
	Interceptor delays a message while waiting on a self created message chain
	Interceptor checks precondition
	Interceptor implements reactive design (delay last message until same process finished)
	Transaction with undo redo stack?

Scenario: Hello World community prints to console
This scenario shows a simple use case where two agents start simultaneously 
with the initialization message. They are than collected by another agent and
finally printed as message to the console.
	Given I have loaded the community "HelloWorldCommunity"
	When I start the message board
	Then the message "Hello World" was posted after a while
	And the program was terminated

Scenario: Hello World community executes agents parallel
This scenario shows that the HelloAgent and the WorldAgent are executed
parallel, although it was not specified this way. It is simply coincidence
that the requirements (InitializeMessage) of both agents are satisfied at
the same time.
	Given I have loaded the community "HelloWorldCommunity"
	When I start the message board
	Then the agents HelloAgent, WorldAgent were executed parallel

Scenario: Parallel execution community prints to console
This scenario shows an use case where one agent produces 4 messages of
the same kind. Once these messages are all executed the result is aggregated
and printed to the console. Additionally, this scenario shows, that the message
collector can collect message across message domains as each of the 4 messages
gets a new domain.
	Given I have loaded the community "ParallelExecutionCommunity"
	When I start the message board
	Then the message "10" was posted after a while
	And the program was terminated

Scenario: Parallel execution community executes in parallel
This scenario shows that all 4 message are executed parallel to each other.
	Given I have loaded the community "ParallelExecutionCommunity"
	When I start the message board
	Then the agent Worker executed 4 messages parallel

Scenario: Decorating interceptor community prints to console
This scenario shows an use case where a message is intercepted. Based
on a check of the original message the original message is than decorated.
Another agent consumes the intercepted message and checks wether is contains
the decorator or not. If the message is decorated the decorated information
is displayed too. This shows that interceptors always are executed before
the acutal consuming agents.
	Given I have loaded the community "DecoratingInterceptorCommunity"
	When I start the message board
	Then the message "Information Detailed" was posted after a while
	And the program was terminated

Scenario: Replacing interceptor community prints to console
This scenario shows an use case where a message is intercepted. Based
on a check of the original message the original message is than replaced.
Replacing a message means that the new message pretends to be the old message
by providing the same parent, children and predecessor. This mechanism is useful,
because all messages should be immutable. If I want to change a certain kind of
property of a message based on a specific condition this is the only option.
	Given I have loaded the community "ReplaceMessageCommunity"
	When I start the message board
	Then the message "Replaced Information" was posted after a while
	And the program was terminated
