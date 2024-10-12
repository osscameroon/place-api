const config = {
    "branches": ["main"],
    "plugins": [
        "@semantic-release/github",
        "@semantic-release/commit-analyzer",
        "@semantic-release/release-notes-generator",
        [
            "@semantic-release/changelog",
            {
                "changelogFile": "CHANGELOG.md"
            }
        ],
        [
            "@semantic-release/git",
            {
                "message": "chore(release): ${nextRelease.version} [skip-ci] \n\n${nextRelease.notes}",
                "assets": ["CHANGELOG.md"]
            }
        ]
    ]

}

module.exports = config
