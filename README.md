# ScoreHunter

[![build status](https://github.com/AnthoChamb/ScoreHunter/actions/workflows/build.yml/badge.svg)](https://github.com/AnthoChamb/ScoreHunter/actions/workflows/build.yml)

A score optimizer for GHTV's Score Chaser.

## ScoreHunter PowerShell module

[![latest version](https://img.shields.io/powershellgallery/v/ScoreHunter)](https://www.powershellgallery.com/packages/ScoreHunter) [![downloads](https://img.shields.io/powershellgallery/dt/ScoreHunter)](https://www.powershellgallery.com/packages/ScoreHunter)

PowerShell module for ScoreHunter.

### Installation

ScoreHunter is available in the [PowerShell Gallery](https://www.powershellgallery.com/packages/ScoreHunter).

```pwsh
Install-Module ScoreHunter
```

If you have an earlier version of the ScoreHunter PowerShell module installed from the PowerShell Gallery and would like to update to the latest version.

```pwsh
Update-Module ScoreHunter
```

### Usage

The following code demonstrates usage of the ScoreHunter PowerShell module.

```pwsh
Import-Module ScoreHunter
Get-ScoreHunterPath guitar_3x2.xmk
```
