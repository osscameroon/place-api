{
  "branches": ["main"],
    "plugins": [
      "@semantic-release/github",
      "@semantic-release/commit-analyzer",
      "@semantic-release/release-notes-generator",
      "@semantic-release/git":{
      "message": "chore(release): ${nextRelease.version} [skip-ci] \n\n${nextRelease.notes}"
      },
    ["@semantic-release/exec", {
        "verifyConditionsCmd": "./verify.sh",
        "publishCmd": "./publish.sh ${nextRelease.version} ${branch.name} ${commits.length} ${Date.now()}"
    }],
      ["@semantic-release/changelog", {
            "changelogFile": "CHANGELOG.md"
          }]
    ]
}