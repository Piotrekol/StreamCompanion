import{_ as d,r as t,o as r,c,a as e,b as n,d as s,w as l}from"./app-c88ab080.js";const u={},h=e("h1",{id:"building-and-previewing-docs",tabindex:"-1"},[e("a",{class:"header-anchor",href:"#building-and-previewing-docs","aria-hidden":"true"},"#"),n(" Building and previewing docs")],-1),_=e("h2",{id:"prerequisites",tabindex:"-1"},[e("a",{class:"header-anchor",href:"#prerequisites","aria-hidden":"true"},"#"),n(" Prerequisites")],-1),p={href:"https://nodejs.org/",target:"_blank",rel:"noopener noreferrer"},g=e("h2",{id:"installation-running",tabindex:"-1"},[e("a",{class:"header-anchor",href:"#installation-running","aria-hidden":"true"},"#"),n(" Installation & running")],-1),m=e("ul",null,[e("li",null,[e("p",null,[e("strong",null,"Step 1"),n(": navigate to main docs directory in cmd / powershell")])]),e("li",null,[e("p",null,[e("strong",null,"Step 2"),n(": fetch modules")])])],-1),f=e("div",{class:"language-bash line-numbers-mode","data-ext":"sh"},[e("pre",{class:"language-bash"},[e("code",null,[e("span",{class:"token function"},"yarn"),n(`
`)])]),e("div",{class:"line-numbers","aria-hidden":"true"},[e("div",{class:"line-number"})])],-1),b=e("div",{class:"language-bash line-numbers-mode","data-ext":"sh"},[e("pre",{class:"language-bash"},[e("code",null,[e("span",{class:"token function"},"npm"),n(),e("span",{class:"token function"},"install"),n(`
`)])]),e("div",{class:"line-numbers","aria-hidden":"true"},[e("div",{class:"line-number"})])],-1),v=e("ul",null,[e("li",null,[e("strong",null,"Step 3"),n(": run docs in development mode")])],-1),x=e("div",{class:"language-bash line-numbers-mode","data-ext":"sh"},[e("pre",{class:"language-bash"},[e("code",null,[e("span",{class:"token function"},"yarn"),n(` docs:dev
`)])]),e("div",{class:"line-numbers","aria-hidden":"true"},[e("div",{class:"line-number"})])],-1),k=e("div",{class:"language-bash line-numbers-mode","data-ext":"sh"},[e("pre",{class:"language-bash"},[e("code",null,[e("span",{class:"token function"},"npm"),n(` run docs:dev
`)])]),e("div",{class:"line-numbers","aria-hidden":"true"},[e("div",{class:"line-number"})])],-1),w=e("strong",null,"Step 3",-1),y={href:"http://localhost:8080/",target:"_blank",rel:"noopener noreferrer"},N=e("p",null,[n("Any edits to local markdown files inside "),e("code",null,"/docs/docs"),n(" directory will be reflected on page within few seconds."),e("br"),n(" Any configuration changes require docs restart to fully take effect.")],-1);function C(I,q){const o=t("ExternalLinkIcon"),a=t("CodeGroupItem"),i=t("CodeGroup");return r(),c("div",null,[h,_,e("ul",null,[e("li",null,[e("a",p,[n("Node.js v12+"),s(o)])])]),g,m,s(i,null,{default:l(()=>[s(a,{title:"YARN",active:""},{default:l(()=>[f]),_:1}),s(a,{title:"NPM"},{default:l(()=>[b]),_:1})]),_:1}),v,s(i,null,{default:l(()=>[s(a,{title:"YARN",active:""},{default:l(()=>[x]),_:1}),s(a,{title:"NPM"},{default:l(()=>[k]),_:1})]),_:1}),e("ul",null,[e("li",null,[w,n(": While above command is running you can navigate to "),e("a",y,[n("http://localhost:8080/"),s(o)]),n(" (port may be different) to see local docs page.")])]),N])}const B=d(u,[["render",C],["__file","index.html.vue"]]);export{B as default};
