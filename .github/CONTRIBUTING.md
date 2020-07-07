# Introduction

First off, thank you for considering contributing to Agents.Net. With your help Agents.Net can develop to be an amazing framework.

Following these guidelines helps to communicate exactly what you wish to address and help you, the developers and maintainers of this project to understand each other.

There are many ways to contribute, from writing tutorials or blog posts, improving the documentation, submitting bug reports and feature requests or writing code which can be incorporated into Agents.Net itself.

Please, don't use the issue tracker for questions. There is a [discord server][discord-link] available for asking questions. Stack Overflow is also worth considering.

# Ground Rules

This project and everyone participating in it is governed by the [Agents.Net Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to [tobias.wilker@gmail.com](mailto:tobias.wilker@gmail.com).

For all contributions, please respect the following guidelines:

-   Each pull request should implement ONE feature or bugfix. If you want to add or fix more than one thing, submit more than one pull request.
- Each pull request should have at least one new or changed test covering the feature or bugfix. If no test is possible please state the reason in the pull request.
-   Do not commit changes to files that are irrelevant to your feature or bugfix (eg:  `.gitignore`).
-   Be willing to accept suggestions on how to improve your code.
-   Be aware that the pull request review process is not immediate, and is generally proportional to the size of the pull request.
-   If your pull request is merged, please do not ask for an immediate release. Releases are scheduled every half year. Only major issues will be released as a patch.

# Your First Contribution
Unsure where to begin contributing? You can start by looking for [beginner issues][beginner-issues].

If there are no beginner issues available or you want to look for something a bit more challenging? Then look out for these labels:
[Bug][bug-issues] - all currently known bugs.
[Documentation][documentation-issues] - these issues should not involve coding, but usually need a bit of knowledge about the framework.
[Enhancement][enhancement-issues] - these are new features that need to be implemented.

Working on your first Pull Request? You can learn how from this *free* series, [How to Contribute to an Open Source Project on GitHub](https://egghead.io/series/how-to-contribute-to-an-open-source-project-on-github).

Also feel free to ask for help; everyone is a beginner at first :smile_cat:

# Getting started
Here is how make a contribution to to Agents.Net:
1. **[Fork Agents.Net][fork-manual] and create a new branch**
Please do not make changes to the master branch. The new branch name should roughly describe, what you are changing.
2. **Check that [all tests are running][run-tests]**
This gives you the confidence that when you changes are done and all tests are green, that you did not breaking anything major :sweat_smile:
3. **Implement your fix or enhancement**
Please remember to write tests for your code. The code style should automatically be checked by the build system.
4. **Add a changelog entry**
If your contribution changes/removes an existing behavior or adds a new behavior, please add an entry to the [CHANGELOG](../CHANGELOG.md) under the Unreleased heading. Changes are sorted either under "Bug Fixing" or "Enhancement". Please consider linking the resolved issue.
5. **Create a pull request**
At this point, you should switch back to your master branch and make sure it's up to date with Active Admin's master branch:
        
        git remote add upstream https://github.com/agents-net/agents.net.git
        git checkout master
        git pull upstream master

   Then update your feature branch from your local copy of master, and push it!

       git checkout my-awesome-feature
       git rebase master
       git push --set-upstream origin my-awesome-feature

   Finally, go to GitHub and  [make a Pull Request](https://help.github.com/articles/creating-a-pull-request)  :smile:
6. **Keep Pull Request updated**
If a maintainer asks you to "rebase" your PR, they're saying that a lot of code has changed, and that you need to update your branch so it's easier to merge.

   Here's a suggested workflow:

       git checkout my-awesome-feature
       git pull --rebase upstream master
       git push --force-with-lease my-awesome-feature

# How to report a bug
In this section you can find the process to create a bug report.

### Security issues
Any security issues should be submitted directly to [tobias.wilker@gmail.com](mailto:tobias.wilker@gmail.com).
In order to determine whether you are dealing with a security issue, ask yourself these two questions:

-   Can I access something that’s not mine, or something I shouldn’t have access to?
-   Can I disable something for other people?

If the answer to either of those two questions are “yes”, then you’re probably dealing with a security issue. Note that even if you answer “no” to both questions, you may still be dealing with a security issue, so if you’re unsure, just email us at [tobias.wilker@gmail.com](mailto:tobias.wilker@gmail.com).

### Reporting a bug
Before filling a new bug please check the issues whether someone all ready reported the same or a related issue.
When reporting a bug please use the provided [bug template][bug-template].

# How to suggest a feature or enhancement
In this section you can find the process of suggesting a new feature.

### Goals
The goal of Agents.Net is to have a framework which

-   logs perfectly all necessary events to see what happens without debugging
-   self-organizes all active parts (agents) so that their needs are met
-   timely decouples all agents so that sending an information (message) does not block the sending agent
-   executes all work that can be parallelized in parallel
- provides the tooling necessary to design, maintain, debug and optimize large agent networks

### Suggesting a new feature

If you find yourself wishing for a feature that doesn't exist in Agents.Net, open an issue on our issues list on GitHub which describes the feature you would like to see, why you need it, and how it should work. Please use the provided template for [feature requests][feature-template].

# Code review process

Once you started a pull request the Github CI will run automatic checks such as checks for build errors or test failures. Pull requests can only be merged by collaborators. If you are a collaborator please request a review of your own pull request if you are not sure if the changes are good enough. If you wish to be a collaborator and work on improving Agents.Net please state that intend in the pull request and the maintainers will gladly accept you as a collaborator.

# Community
Stuck? Try one of the following:

-   Ask for help on  [Stack Overflow](https://stackoverflow.com/questions/tagged/agents-net).
-   You are strongly encouraged to  [file an issue][bug-template]  about the problem.
-   Ask for help on  [Discord][discord-link].

Development on Agnets.Net is community-driven:

-   Huge thanks to all the  [contributors](https://github.com/cookiecutter/cookiecutter/blob/master/AUTHORS.md)  who have pitched in to help make Cookiecutter an even better tool.
-   Everyone is invited to contribute. Read the  [contributing instructions](https://github.com/cookiecutter/cookiecutter/blob/master/CONTRIBUTING.md), then get started.
-   Connect with other Cookiecutter contributors and users on  [Slack](https://join.slack.com/t/cookie-cutter/shared_invite/enQtNzI0Mzg5NjE5Nzk5LTRlYWI2YTZhYmQ4YmU1Y2Q2NmE1ZjkwOGM0NDQyNTIwY2M4ZTgyNDVkNjMxMDdhZGI5ZGE5YmJjM2M3ODJlY2U)  (note: due to work and commitments, a core committer might not always be available)

Encouragement is unbelievably motivating. If you want more work done on Cookiecutter, show support:

-   Thank a core committer for their efforts.
-   Star  [Cookiecutter on GitHub](https://github.com/cookiecutter/cookiecutter).
-   [Support this project](https://github.com/cookiecutter/cookiecutter#support-this-project)

Got criticism or complaints?

-   [File an issue](https://github.com/cookiecutter/cookiecutter/issues?q=is%3Aopen)  so that Cookiecutter can be improved. Be friendly and constructive about what could be better. Make detailed suggestions.
-   **Keep us in the loop so that we can help.**  For example, if you are discussing problems with Cookiecutter on a mailing list,  [file an issue](https://github.com/cookiecutter/cookiecutter/issues?q=is%3Aopen)  where you link to the discussion thread and/or cc at least 1 core committer on the email.
-   Be encouraging. A comment like "This function ought to be rewritten like this" is much more likely to result in action than a comment like "Eww, look how bad this function is."

Waiting for a response to an issue/question?

-   Be patient and persistent. All issues are on the core committer team's radar and will be considered thoughtfully, but we have a lot of issues to work through. If urgent, it's fine to ping a core committer in the issue with a reminder.
-   Ask others to comment, discuss, review, etc.
-   Search the Cookiecutter repo for issues related to yours.
-   Need a fix/feature/release/help urgently, and can't wait?  [@audreyr](https://github.com/audreyr)  is available for hire for consultation or custom development.

# BONUS: Code, commit message and labeling conventions
These sections are not necessary, but can help streamline the contributions you receive.

### Explain your preferred style for code, if you have any.

**Need inspiration?** [1] [Requirejs](http://requirejs.org/docs/contributing.html#codestyle) [2] [Elasticsearch](https://github.com/elastic/elasticsearch/blob/master/CONTRIBUTING.md#contributing-to-the-elasticsearch-codebase)

### Explain if you use any commit message conventions.

**Need inspiration?** [1] [Angular](https://github.com/angular/material/blob/master/.github/CONTRIBUTING.md#submit) [2] [Node.js](https://github.com/nodejs/node/blob/master/CONTRIBUTING.md#step-3-commit)

### Explain if you use any labeling conventions for issues.

**Need inspiration?** [1] [StandardIssueLabels](https://github.com/wagenet/StandardIssueLabels#standardissuelabels) [2] [Atom](https://github.com/atom/atom/blob/master/CONTRIBUTING.md#issue-and-pull-request-labels)

[discord-link]:https://discord.gg/gn3dqG4
[beginner-issues]:https://github.com/agents-net/agents.net/labels/good%20first%20issue
[bug-issues]:https://github.com/agents-net/agents.net/labels/bug
[documentation-issues]:https://github.com/agents-net/agents.net/labels/documentation
[enhancement-issues]:https://github.com/agents-net/agents.net/labels/enhancement
[fork-manual]:https://help.github.com/articles/fork-a-repo
[run-tests]:https://github.com/agents-net/agents.net#run-tests
[bug-template]:https://github.com/agents-net/agents.net/issues/new?assignees=&labels=bug&template=bug_report.md&title=
[feature-template]:https://github.com/agents-net/agents.net/issues/new?assignees=&labels=enhancement&template=feature_request.md&title=
<!--stackedit_data:
eyJoaXN0b3J5IjpbLTEyNDQwMDQ0MDMsLTE3Mjc0MzkxOTcsLT
EzNDM3Njg5NjgsLTc4OTQ2ODk0MCwxMzc2MDc5MDE0LC0xOTU0
MjAyMDYxLC0zNzU4ODUyMDIsLTIxMzU0MjgzMSwtMTU3OTgwND
Q1LDU2ODIwMDQ0NV19
-->