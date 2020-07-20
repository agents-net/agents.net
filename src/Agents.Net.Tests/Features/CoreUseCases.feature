Feature: CoreUseCases
	This feature is at the same time a test of the core functionality as well
	as a documentation of possible use cases on hwo to use the agent framework.
	Each test will start with 'Given I have loaded the community "<name>"'.
	The loaded community can be found in the directory 
	"<ProjectDirectory>/Tools/Communities/<name>". There is the Agents.Net
	Designer model (.amodel), a svg visualization of the community as well as
	the actual implementation.

	Not Implemented Use Cases:
	Implicit parallelisation (agents executed parallel) - use HelloWorldCommunity
	Explicit parallelisation (agents executed parallel; community ran to finish (count executed agents))
	Message collector collects message from parent and child domain with explicit parallelisation _î
	Interceptor decorates a message
	Interceptor replaces a message with a changed value
	Interceptor delays a message while waiting on a self created message chain
	Interceptor checks precondition
	Interceptor implements reactive design (delay last message until same process finished)
	Transaction with undo redo stack?

@CollectExecutionOrderInfo
Scenario: Hello World community prints to console
This scenario shows a simple use case where two agents start simultaneously 
with the initialization message. They are than collected by another agent and
finally printed as message to the console.
	Given I have loaded the community HelloWorldCommunity
	When I start the message board
	Then the message "Hello World" was posted after a while
	And the program was terminated
