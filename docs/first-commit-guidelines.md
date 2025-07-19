## First Commit/Branch (Using Squash Commit)

These guidelines outline the best practices for making your first commit. Follow them to ensure clarity and consistency in your project contributions.

When merging the pull request, please select "Squash commit" merge type that way only the 1st commit from the PR get added to the base branch.

In a Squash Commit workflow, all commits from a feature branch are combined into a single commit before being merged into the main branch. This approach helps maintain a clean and concise Git history, with each feature represented by a single, meaningful commit.

### Branching Example:

- workitem/\* (e.g workitem/7526) - This is where the majority of commits will take place. Whenever you start a new piece of work, create a new `workitem/` branch.

### First Commit Message Example:

The number represents the work item you are currently working on, along with the title of that work item.

```bash
#81987 - Implement Amazing Feature
```
