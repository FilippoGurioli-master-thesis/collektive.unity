const fs = require("fs");
const path = require("path");
let config = require("semantic-release-preconfigured-conventional-commits");

config.plugins.push(
  ["@semantic-release/exec", {
    prepareCmd: "dotnet tool run dotnet-script .automation/UpdateUnityPackageVersion.csx ${nextRelease.version}"
  }],
  ["@semantic-release/changelog", {
    changelogFile: "Collektive.Unity/CHANGELOG.md"
  }],
  ["@semantic-release/github", {
    assets: [
      { path: "package.zip", label: "Collektive.Unity" },
      { path: "package.zip.sha256", label: "SHA256 Digest" },
      { path: "package.zip.sha256.sig", label: "Signature" }
    ]
  }],
  [
    "@semantic-release/git",
    {
      assets: ['Collektive.Unity/package.json', 'Collektive.Unity/package-lock.json', 'Collektive.Unity/CHANGELOG.md'],
      message: 'chore(release): ${nextRelease.version} [skip ci]'
    }
  ]
);

module.exports = config;
