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