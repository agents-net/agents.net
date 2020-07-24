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

### Changed
- **Breaking Change:** Switch from magic string based agent definition to type based agent definition
- **Breaking Change:** Change execution method of aggregator to `IReadonlyCollection` so that it can be passed directly to TerminateDomainsOf
- `MessageCollector` messages can know be consumed directly, so that they are removed from the collector
- `Message` predecessor property visibility changed to public

### Removed
- **Breaking Change:** Removed `Community` class. Register agents directly with `IMessageBoard`

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