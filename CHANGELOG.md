# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

<!--
*** ## <Version>
*** ### Added
*** ### Changed
*** ### Removed
*** ### Fixed
-->

## [Unreleased]
### Fixed
- #99 - Replace inherits id and updates the whole hierarchies descendants

### Added
- #102 - A new helper class `MessageGate` was introduced which helps covering legacy uses cases as well as simplifies several other use cases
- `InterceptionAction.Delay` - this can delay the execution of a message until a token is released. `InterceptionAction` is no longer an enum, but this does not break existing code.

### Changed
- `PushAndExecute` is now obsolete. It was replaced by the new `MessageGate` type. It will be removed with 2022.6.0
- `MessageAggregator` is now obsolete. It was replaced by the new `MessageGate` type. It will be removed with 2022.6.0
- `Agent.OnMessages` is now obsolete. It was replaced by the new `MessageGate` type. It will be removed with 2022.6.0

### Removed
- **Breaking Change:** Removed the `name` parameter in `Agent` and `Message` classes

## 2021.6.3
### Fixed

- #115 - The `MessageCollector` works now with `ConcurrentDictionary` instead of locks to avoid the exception in the defect

## 2021.6.2
### Added
- `PushAndExecute` accepts a `CancellationToken` now. With this it is possible to stop the wait operation

### Fixed
- #93 - When changing the message domain the message domain of the whole hierarchy is now changed
- #95 - Terminated domains are not considered anymore when choosing the parent for a new domain

## 2021.6.1
### Changed
- **Breaking Change:** Removed the `Predecessors` and `GetPredecessor` members from `Message`. This was a huge unfixable memory leak. Instead of it `MessageDomain` and `MessageCollector` should be used to address scenarios where the predecessor message must be evaluated
- **Breaking Change:** The `Agent` class implements now `IDisposable`. Disposables can now be added with the new `AddDisposable` method
- `MessageAggregator` automatically terminates the message domains of the aggregated messages. Can be disabled with optional constructor parameter
- `MessageCollector` automatically removes messages from a terminated domain. This was a huge memory leak

### Added
- `MessageCollector.PushAndExecute` is a replacement for `MessageCollector.FindSetsForDomain`. It adds a message to the collector and waits for a complete set with that message to execute an action
- `Agent.AddDisposable` marks the passed object for disposal in a thread-safe way

### Removed
- **Breaking Change:** Removed useless `MessageDomainsCreatedMessage` and `MessageDomainsTerminatedMessage`
- **Breaking Change:** Removed the `MessageCollector.FindSetsForDomain` method. Use the new `MessageCollector.PushAndExecute` method instead

### Fixed
- `MessageDomain` did not dispose replaced messages. This is now fixed
- Several memory leaks fixed

## 2021.0.0
### Added
- Integration tests
- Community health files and appropriate readme
- It is now possible to register to all messages by using the type Message for the ConsumeAttribute or InterceptsAttribute
- Messages have now a defined lifecycle. This can be used to dispose objects during execution safely
- XML Documentation for all public classes in Agents.Net project - should be visible in nuget package
- Getting Started guide and documentation website

### Changed
- **Breaking Change:** Switch from magic string based agent definition to type based agent definition
- **Breaking Change:** Change execution method of aggregator to `IReadonlyCollection` so that it can be passed directly to TerminateDomainsOf
- **Breaking Change:** Switched logging framework from NLog to Serilog
- `MessageCollector` messages can know be consumed directly, so that they are removed from the collector
- `Message` predecessor property visibility changed to public

### Removed
- **Breaking Change:** Removed `Community` class. Register agents directly with `IMessageBoard`
- **Breaking Change:** Removed Terminate methods from MessageDomain helper
- **Breaking Change:** Removed HandledExceptionMessageDecorator as it is useless unless there is a default exeception message agent
- **Breaking Change:** Removed children from Message constructor. Each message can have only one child and only by using the MessageDecorator class.

## 2020.6.0
### Added
- Some unit tests.
- Benchmarks for the framework for all common use cases.

### Changed
- Message domain handling - it is now not dependent on creating specific messages.
- Renamed `IsDecorator` to `IsDecorated` and made it static.

### Fixed
- #28 - There were several issues which lead to the observed behavior

## 2020.0.1
### Added
- Nuget release information such as a description and icon.

## 2020.0.0
### Added
- Everything. This is the first version.