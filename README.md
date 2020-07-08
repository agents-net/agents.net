<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![NuGet][nuget-shield]][nuget-url]
[![Workflow][workflow-shield]][workflow-url]


<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/agents-net/agents.net">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Agents.Net</h3>

  <p align="center">
    .NET class library to easily build self-organizing agents based systems!
    <br />
    <a href="https://github.com/agents-net/agents.net/wiki"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/agents-net/agents.net/issues/new?assignees=&labels=bug&template=bug_report.md&title=">Report Bug</a>
    ·
    <a href="https://github.com/agents-net/agents.net/issues/new?assignees=&labels=enhancement&template=feature_request.md&title=">Request Feature</a>
    ·
    <a href="https://discord.gg/gn3dqG4">Ask a Question</a>
  </p>
</p>



<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Getting Started](#getting-started)
  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
* [Usage](#usage)
* [Roadmap](#roadmap)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)



<!-- ABOUT THE PROJECT -->
## About The Project

[![Agents.Net Intro][product-screenshot]][product-homepage]

As I read about event sourcing and microservices I thought that these ideas are maybe not only viable for large distributed systems but also for your everyday console or UI application. This was when I started the Agents.Net framework. Agent based programming is more of an academic topic, but fits the ideas the best. The idea is to have a framework which 
* logs perfectly all necessary events to see what happens without debugging
* self-organizes all active parts (agents) so that their needs are met
* timely decouples all agents so that sending an information (message) does not block the sending agent
* executes all work that can be parallelized in parallel

Here's why:
* Your time should be focused on writing the logic without spending all your time thinking about who is talking to whom and how to optimize the performance.
* You should not be bothered with difficult mechanisms to make your UI responsive.
* You should be able to easily find an issue in your application just by looking at the log without the need for time consuming debugging and reproducing the defect.

The basic idea of the framework is this. Each agent does one thing (connects to a database, reads console input, verifies some values, ...). For that it needs specific information (location of the database, the raw console input, ...). Additionally it provides all the information it knows (the active database connection, single console arguments and their values, ...). The agent is not concerned where the information comes from or how if any needs the information provided. Based on that idea alone the system will organize all agents automatically simply based on who needs a specific information which was provided.

A list of commonly used resources that I find helpful are listed in the acknowledgements.

### Built With
* [.NET Core][dotnet-core-website]
* [NLog](https://nlog-project.org/)



<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

* [.NET Core](https://dotnet.microsoft.com/download/dotnet-core)
```sh
sudo apt-get install -y dotnet-sdk-3.1
```

### Installation

To use the current release simply add it via NuGet:
```sh
dotnet add package Agents.Net
```
To use to latest version from master you can either compile it yourself or use the latest NuGet package from github:

 1. [Authenticating to github packages for this repository][github-package-auth]
 2. Add package via NuGet
```sh
nuget install Agents.Net -prerelease
```

### Compilation

Compile using [.NET Core][dotnet-core-website]
```sh
dotnet build src
```

### Run tests

Test using [.NET Core][dotnet-core-website]
```sh
dotnet test src
```

### Run benchmarks

Run benchmarks using [.NET Core][dotnet-core-website]
```sh
dotnet run -c Release -p src/Agents.Net.Benchmarks/Agents.Net.Benchmarks.csproj
```



<!-- USAGE EXAMPLES -->
## Usage


WIP -> I will design use cases as integration tests and than based on that include examples here.



<!-- ROADMAP -->
## Roadmap

See the [open issues][issues-url] for a list of proposed features (and known issues).

### Versioning

The versioning is straight forward. I intend to make one release every half year. Releases will be versioned YEAR.[0|6].PATCH. So for the mid-year release of 2020 it is 2020.6.0. Patches will only be released for major issues. There will always be pre-releases for each commit to master  as a github package.


<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**. Please read the [contribution guideline](.github/CONTRIBUTING.md) before contributing.

In a nutshell:

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- AUTHORS -->
## Authors

  - **Tobias Wilker** - *Initial idea and implementation* - [twilker](https://github.com/twilker)

See also the list of [contributors][contributors-url] who participated in this project.

<!-- LICENSE-->
## License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.

<!-- CONTACT -->
## Contact

Mail List - [agents-net@googlegroups.com](mailto:agents-net@googlegroups.com)

Project Link: [https://github.com/agents-net/agents.net](https://github.com/agents-net/agents.net)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [BenchmakDotNet](https://benchmarkdotnet.org/)
* [NUnit](https://nunit.org/)
* [NSubstitute](https://nsubstitute.github.io/)
* [Event Sourcing - Explanation from Martin Fowler](https://martinfowler.com/eaaDev/EventSourcing.html)
* My co-workers





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/agents-net/agents.net.svg?style=flat-square
[contributors-url]: https://github.com/agents-net/agents.net/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/agents-net/agents.net.svg?style=flat-square
[forks-url]: https://github.com/agents-net/agents.net/network/members
[stars-shield]: https://img.shields.io/github/stars/agents-net/agents.net.svg?style=flat-square
[stars-url]: https://github.com/agents-net/agents.net/stargazers
[issues-shield]: https://img.shields.io/github/issues/agents-net/agents.net.svg?style=flat-square
[issues-url]: https://github.com/agents-net/agents.net/issues
[license-shield]: https://img.shields.io/github/license/agents-net/agents.net.svg?style=flat-square
[license-url]: https://github.com/agents-net/agents.net/blob/master/LICENSE
[nuget-shield]:https://img.shields.io/nuget/v/Agents.Net?style=flat-square
[nuget-url]:https://www.nuget.org/packages/Agents.Net
[workflow-shield]:https://img.shields.io/github/workflow/status/agents-net/agents.net/Build%2C%20Test%20and%20Publish?style=flat-square
[workflow-url]:https://github.com/agents-net/agents.net/actions?query=workflow%3A"Build%2C+Test+and+Publish"
[product-screenshot]: images/AgentsNetGif.gif
[product-homepage]:https://github.com/agents-net/agents.net
[dotnet-core-website]:https://dotnet.microsoft.com/
[github-package-auth]:https://docs.github.com/en/packages/using-github-packages-with-your-projects-ecosystem/configuring-dotnet-cli-for-use-with-github-packages#authenticating-to-github-packages
