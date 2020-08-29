Feature: Message Board Use Cases
	Here are more specific use cases for features of the message board. These
	features are to subtle or obscure to showcase them in the core use cases.

Scenario: Intercept and consume base message
This scenario shows the feature, that it is possible to consume or intercept
the type Message, meaning the base type of all messages. This will without
exception consume/intercept all messages that are send. The use cases for this
are very limited. But this is a usefull feature especially for unit tests.
In this scenario the MessageProducer sends 5 different messages. There is an
interceptor and a consuming agent for the type Message which send messages
themself after they received 6 message (the 5 message mentioned above and the 
InitializeMessage). Once they have send their messages the program terminates.
	Given I have loaded the community "CounterCommunity"
	When I start the message board
	Then the message "Consumed: 6; Intercepted: 6" was posted after a while
	And the program was terminated