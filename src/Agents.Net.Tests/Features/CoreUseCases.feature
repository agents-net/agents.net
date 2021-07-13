Feature: CoreUseCases
	This feature is at the same time a test of the core functionality as well
	as a documentation of possible use cases on hwo to use the agent framework.
	Each test will start with 'Given I have loaded the community "<name>"'.
	The loaded community can be found in the directory 
	"<ProjectDirectory>/Tools/Communities/<name>". There is the Agents.Net
	Designer model (.amodel), a svg visualization of the community as well as
	the actual implementation.

Scenario: Hello World community prints to console
This scenario shows a simple use case where two agents start simultaneously 
with the initialization message. They are than collected by another agent and
finally printed as message to the console.
	Given I have loaded the community "HelloWorldCommunity"
	When I start the message board
	Then the message "Hello World" was posted after a while
	And the program was terminated
    
@Parallel
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

@Parallel
Scenario: Parallel execution community executes in parallel
This scenario shows that all 4 message are executed parallel to each other.
	Given I have loaded the community "ParallelExecutionCommunity"
	When I start the message board
	Then the agent Worker executed 4 messages parallel

Scenario: Decorating interceptor community prints to console
This scenario shows an use case where a message is intercepted. Based
on a check of the original message the original message is than decorated.
Another agent consumes the intercepted message and checks whether is contains
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

Scenario: Delay community prints multiple messages to console
This scenario shows an use case where a message is intercepted. Based
on a check of the original message the original message is than delayed until
another chain of agents is completed. This scenario is useful if based on some
condition a specific action needs to be executed without letting the original
chain of messages know about it. A more concrete example would be the following.
Assuming I have a chain of agents which commits changes to a git repository and
pushes it to the remote repository. Now if the repository contains a submodule,
I want the update the submodule before the changes are committed. The original
chain of agents (creating a change, commiting, pushing) does not need to know 
about submodules. The submodule updated is done by delaying the message the
commiting agent is using.
	Given I have loaded the community "DelayCommunity"
	When I start the message board
	Then the message "Transformed Information" was posted after at most 1000 ms
	And the message "Special Information" was posted after a while
	And the program was terminated

Scenario: Precondition check community prints error message to console
This scenario shows an use case where a message is intercepted. The intercepted 
message is than validated and if it is invalid, the original message is not send.
Instead an exception message is generated. This is the most simple of use cases
of interceptors. The consuming agent of the original message does not care if
the original message is validated or not. Therefore it cannot react on an
"InformationValidated" message.
	Given I have loaded the community "PreconditionCheckCommunity"
	When I start the message board
	Then the message "Validation Failed" was posted after a while
	And the program was terminated

Scenario: Defensive programming community does not terminate on recoverable exception
This scenario shows an use case where it is show how the agent framework can be 
used to program defensively. In this use case the FaultyAgent produces a recovereable
exception which is only logged without terminating the whole program. Remember
if there is not exception message handling agent, the agent framework will treat
all exception messages as recoverable.
	Given I pass the command line argument "Recover" to the program
	And I have loaded the community "DefensiveProgrammingCommunity"
	When I start the message board
	Then the message "Recoverable Exception" was posted after a while
	And the program was not terminated

Scenario: Defensive programming community terminates on unrecoverable exception
This scenario shows an use case where it is shown how the agent framework can be 
used to program defensively. In this use case the FaultyInterceptor produces an
unrecoverable exception which is logged and the program is terminated. Remember
if there is not exception message handling agent, the agent framework will treat
all exception messages as recoverable.
	Given I pass the command line argument "Terminate" to the program
	And I have loaded the community "DefensiveProgrammingCommunity"
	When I start the message board
	Then the message "Unrecoverable Exception" was posted after a while
	And the program was terminated

Scenario: Legacy service community executes a service call
This scenario shows an use case where it is shown how the agent framework can be
used in a legacy use case. In this use case a call to a service is made which
excepts some parameters and expects a result.
    Given I have loaded the community "LegacyServiceBridgeCommunity"
	When I start the message board
    And Call the legacy service with the data "false"
	Then the legacy service returned "ServiceCallResult"

Scenario: Legacy service community handles an exception
This scenario shows an use case where it is shown how the agent framework can be
used in a legacy use case. In this use case a call to a service is made which
creates an exception that changes the service outcome.
    Given I have loaded the community "LegacyServiceBridgeCommunity"
	When I start the message board
    And Call the legacy service with the data "true"
	Then the legacy service returned "Exception"

    Scenario: Transaction manager community completes task
    This scenario shows the use case of a transaction mechanism. The idea is to hav an
    agent that starts a transaction and waits for the finished message with the help of
    a message gate. This also shows the use of the SendAndContinue method of the message
    gate.
        Given I pass the command line argument "Complete" to the program
        And I have loaded the community "TransactionManagerCommunity"
        When I start the message board
        Then the message "Transaction Successful" was posted after a while
        And the program was terminated

    Scenario: Transaction manager community rolls back on error
    This scenario shows the use case of a transaction mechanism. The idea is to hav an
    agent that starts a transaction and waits for the finished message with the help of
    a message gate. The message gate comes with the handy functionality to stop the 
    execution when there is an exception. With that it is easy to initiate a rollback.
    In a real application the rollback than needs to revert any changes made during the
    transaction.
        Given I pass the command line argument "Error" to the program
        And I have loaded the community "TransactionManagerCommunity"
        When I start the message board
        Then the message "Rollback" was posted after a while
        And the program was terminated
