# Building and previewing docs

## Prerequisites

* [Node.js v12+](https://nodejs.org/)

## Installation & running

* **Step 1**: navigate to main docs directory in cmd / powershell

* **Step 2**: fetch modules

<CodeGroup>
  <CodeGroupItem title="YARN" active>

```bash
yarn
```

  </CodeGroupItem>

  <CodeGroupItem title="NPM">

```bash
npm install
```

  </CodeGroupItem>
</CodeGroup>

* **Step 3**: run docs in development mode

<CodeGroup>
  <CodeGroupItem title="YARN" active>

```bash
yarn docs:dev
```

  </CodeGroupItem>

  <CodeGroupItem title="NPM">

```bash
npm run docs:dev
```

  </CodeGroupItem>
</CodeGroup>

* **Step 3**: While above command is running you can navigate to [http://localhost:8080/](http://localhost:8080/) (port may be different) to see local docs page.  

Any edits to local markdown files inside `/docs/docs` directory will be reflected on page within few seconds.  
Any configuration changes require docs restart to fully take effect.
