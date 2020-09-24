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
### Added
- Integration tests
- Community health files and appropriate readme
- It is now possible to register to all messages by using the type Message for the ConsumeAttribute or InterceptsAttribute
- Messages have now a defined lifecycle. This can be used to dispose objects during execution safely

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