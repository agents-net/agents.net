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
### Explain security disclosures first!
At bare minimum, include this sentence:
> If you find a security vulnerability, do NOT open an issue. Email XXXX instead.

If you don’t want to use your personal contact information, set up a “security@” email address. Larger projects might have more formal processes for disclosing security, including encrypted communication. (Disclosure: I am not a security expert.)

> Any security issues should be submitted directly to security@travis-ci.org
> In order to determine whether you are dealing with a security issue, ask yourself these two questions:
> * Can I access something that's not mine, or something I shouldn't have access to?
> * Can I disable something for other people?
>
> If the answer to either of those two questions are "yes", then you're probably dealing with a security issue. Note that even if you answer "no" to both questions, you may still be dealing with a security issue, so if you're unsure, just email us at security@travis-ci.org.

[source: [Travis CI](https://github.com/travis-ci/travis-ci/blob/master/CONTRIBUTING.md)] **Need more inspiration?** [1] [Celery](https://github.com/celery/celery/blob/master/CONTRIBUTING.rst#security) [2] [Express.js](https://github.com/expressjs/express/blob/master/Security.md)

### Tell your contributors how to file a bug report.
You can even include a template so people can just copy-paste (again, less work for you).

> When filing an issue, make sure to answer these five questions:
>
> 1. What version of Go are you using (go version)?
> 2. What operating system and processor architecture are you using?
> 3. What did you do?
> 4. What did you expect to see?
> 5. What did you see instead?
> General questions should go to the golang-nuts mailing list instead of the issue tracker. The gophers there will answer or ask you to file an issue if you've tripped over a bug.

[source: [Go](https://github.com/golang/go/blob/master/CONTRIBUTING.md#filing-issues)] **Need more inspiration?** [1] [Celery](https://github.com/celery/celery/blob/master/CONTRIBUTING.rst#other-bugs ) [2] [Atom](https://github.com/atom/atom/blob/master/CONTRIBUTING.md#reporting-bugs) (includes template)

# How to suggest a feature or enhancement
### If you have a particular roadmap, goals, or philosophy for development, share it here.
This information will give contributors context before they make suggestions that may not align with the project’s needs.

> The Express philosophy is to provide small, robust tooling for HTTP servers, making it a great solution for single page applications, web sites, hybrids, or public HTTP APIs.
>
> Express does not force you to use any specific ORM or template engine. With support for over 14 template engines via Consolidate.js, you can quickly craft your perfect framework.

[source: [Express](https://github.com/expressjs/express#philosophy)] **Need more inspiration?** [Active Admin](https://github.com/activeadmin/activeadmin#goals)

### Explain your desired process for suggesting a feature.
If there is back-and-forth or signoff required, say so. Ask them to scope the feature, thinking through why it’s needed and how it might work.

> If you find yourself wishing for a feature that doesn't exist in Elasticsearch, you are probably not alone. There are bound to be others out there with similar needs. Many of the features that Elasticsearch has today have been added because our users saw the need. Open an issue on our issues list on GitHub which describes the feature you would like to see, why you need it, and how it should work.

[source: [Elasticsearch](https://github.com/elastic/elasticsearch/blob/master/CONTRIBUTING.md#feature-requests)] **Need more inspiration?** [1] [Hoodie](https://github.com/hoodiehq/hoodie/blob/master/CONTRIBUTING.md#feature-requests) [2] [Ember.js](https://github.com/emberjs/ember.js/blob/master/CONTRIBUTING.md#requesting-a-feature)

# Code review process
### Explain how a contribution gets accepted after it’s been submitted.
Who reviews it? Who needs to sign off before it’s accepted? When should a contributor expect to hear from you? How can contributors get commit access, if at all?

> The core team looks at Pull Requests on a regular basis in a weekly triage meeting that we hold in a public Google Hangout. The hangout is announced in the weekly status updates that are sent to the puppet-dev list. Notes are posted to the Puppet Community community-triage repo and include a link to a YouTube recording of the hangout.
> After feedback has been given we expect responses within two weeks. After two weeks we may close the pull request if it isn't showing any activity.

[source: [Puppet](https://github.com/puppetlabs/puppet/blob/master/CONTRIBUTING.md#submitting-changes)] **Need more inspiration?** [1] [Meteor](https://meteor.hackpad.com/Responding-to-GitHub-Issues-SKE2u3tkSiH ) [2] [Express.js](https://github.com/expressjs/express/blob/master/Contributing.md#becoming-a-committer)

# Community
If there are other channels you use besides GitHub to discuss contributions, mention them here. You can also list the author, maintainers, and/or contributors here, or set expectations for response time.

> You can chat with the core team on https://gitter.im/cucumber/cucumber. We try to have office hours on Fridays.

[source: [cucumber-ruby](https://github.com/cucumber/cucumber-ruby/blob/master/CONTRIBUTING.md#talking-with-other-devs)] **Need more inspiration?**
 [1] [Chef](https://github.com/chef/chef/blob/master/CONTRIBUTING.md#-developer-office-hours) [2] [Cookiecutter](https://github.com/audreyr/cookiecutter#community)

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
<!--stackedit_data:
eyJoaXN0b3J5IjpbLTE5NTQyMDIwNjEsLTM3NTg4NTIwMiwtMj
EzNTQyODMxLC0xNTc5ODA0NDUsNTY4MjAwNDQ1XX0=
-->