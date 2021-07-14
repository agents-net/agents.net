Feature: Message Board Use Cases
	Here are more specific use cases for features of the message board. These
	features are to subtle or obscure to showcase them in the core use cases.

Scenario: Intercept and consume base message
This scenario shows the feature, that it is possible to consume or intercept
the type Message, meaning the base type of all messages. This will without
exception consume/intercept all messages that are send. The use cases for this
are very limited. But this is a useful feature especially for unit tests.
In this scenario the MessageProducer sends 5 different messages. There is an
interceptor and a consuming agent for the type Message which send messages
themself after they received 6 message (the 5 message mentioned above and the 
InitializeMessage). Once they have send their messages the program terminates.
	Given I have loaded the community "CounterCommunity"
	When I start the message board
	Then the message "Consumed: 6; Intercepted: 6" was posted after at most 600 ms
	And the program was terminated

Scenario: Detect interception conflicts
This scenario shows the feature, that the message board detects when a message was 
intercepted by at least two agents that returned DoNotPublish and Delay. In this case
the message board needs to send an exception message.
    Given I pass the command line argument "simulate interception conflict" to the program
	And I have loaded the community "DelayCommunity"
	When I start the message board
	Then the message "Interception conflict detected." was posted after a while
	And the program was terminated

Scenario: Stop publishing after delay
This scenario shows the feature, an interceptor agent delays a message and later decides to 
not publish the message. In this case the intention can be passed to the delay token release
method. Once at least one token has the intention to not publish the final message, then the
delayed message will not be published. 
    Given I pass the command line argument "DoNotPublish intention" to the program
	And I have loaded the community "DelayCommunity"
	When I start the message board
	Then the message "Delayed message was not published." was posted after a while
	And the program was not terminated
	
@DisposeCheck
Scenario: Dispose messages after they were used
This scenario shows the feature, that messages are disposed after they were
used. The heuristic that is used to determine whether a message is used or not
is complex as the agents themself cannot determine that as they do not know 
how many agents react to the message. The heuristic used to determine when a 
message can be disposed is that by default after the Execute method of any
Agent the message will count it as used. After it was used as many times as
there are agents it will dispose itself. The community used will have different
messages. Afterwards it is checked whether all messages are disposed 
	Given I have loaded the community "DefaultDisposeCommunity"
	When I start the message board
	Then the program was terminated
	And all messages are disposed