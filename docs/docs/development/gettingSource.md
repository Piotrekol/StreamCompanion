# Getting full source code

## Why

StreamCompanion repository makes use of git submodules with need to be fetched in order to be able to compile StreamCompanion itself.  
If you do not intend to compile StreamCompanion you can get source by simply downloading repository zip from github, otherwise continue.

## Prerequisites

* [Git (any recent version)](https://git-scm.com/downloads)

## Downloading source

* **Step 1:** Navigate to directory where you want StreamCompanion source to be downloaded in your shell (cmd / powershell)

* **Step 2:** Execute this command in shell:

```bash
git clone https://github.com/Piotrekol/StreamCompanion --recurse-submodules
```

* Wait for the command to end and you should see new `StreamCompanion` folder created along with all required files inside!
