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
    <a href="https://github.com/othneildrew/Best-README-Template/issues">Report Bug</a>
    ·
    <a href="https://github.com/othneildrew/Best-README-Template/issues">Request Feature</a>
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

As I read about event sourcing and microservices I thought that these ideas are maybe not only viable for large distributed systems but also for your everyday console or UI application. This was when I started the Agents.Net framework. Agent based programming is more of an academic topic, but fits the ides the best. The idea is to have a framework which 
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
* [.NET Core](https://dotnet.microsoft.com/)
* [NLog](https://nlog-project.org/)



<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* npm
```sh
npm install npm@latest -g
```

### Installation

1. Get a free API Key at [https://example.com](https://example.com)
2. Clone the repo
```sh
git clone https://github.com/your_username_/Project-Name.git
```
3. Install NPM packages
```sh
npm install
```
4. Enter your API in `config.js`
```JS
const API_KEY = 'ENTER YOUR API';
```



<!-- USAGE EXAMPLES -->
## Usage

Use this space to show useful examples of how a project can be used. Additional screenshots, code examples and demos work well in this space. You may also link to more resources.

_For more examples, please refer to the [Documentation](https://example.com)_



<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/othneildrew/Best-README-Template/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Your Name - [@your_twitter](https://twitter.com/your_username) - email@example.com

Project Link: [https://github.com/your_username/repo_name](https://github.com/your_username/repo_name)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Img Shields](https://shields.io)
* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Pages](https://pages.github.com)
* [Animate.css](https://daneden.github.io/animate.css)
* [Loaders.css](https://connoratherton.com/loaders)
* [Slick Carousel](https://kenwheeler.github.io/slick)
* [Smooth Scroll](https://github.com/cferdinandi/smooth-scroll)
* [Sticky Kit](http://leafo.net/sticky-kit)
* [JVectorMap](http://jvectormap.com)
* [Font Awesome](https://fontawesome.com)





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
<!--stackedit_data:
eyJoaXN0b3J5IjpbNjIyNzk0MDU1LC0xNjc3MjMxOTYxLC00MD
c0MDE5MDgsLTE3Mjc1NjU5MTUsNDM0MDI5NTUwXX0=
-->