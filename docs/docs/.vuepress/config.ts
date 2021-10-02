import { defineUserConfig } from '@vuepress/cli';
import type { DefaultThemeOptions } from '@vuepress/theme-default';
import { sidebar } from './configs';

const isProd = process.env.NODE_ENV === 'production';
console.log('isProd:', isProd);
export default defineUserConfig<DefaultThemeOptions>({
  lang: 'en-US',
  title: 'StreamCompanion',
  description: 'osu! information extractor... on steroids',
  base: isProd ? '/StreamCompanion/' : '',
  head: [
    [
      'link',
      {
        rel: 'icon',
        type: 'image/png',
        sizes: '16x16',
        href: `/images/icons/favicon-16x16.png`,
      },
    ],
    [
      'link',
      {
        rel: 'icon',
        type: 'image/png',
        sizes: '32x32',
        href: `/images/icons/favicon-32x32.png`,
      },
    ],
    ['link', { rel: 'manifest', href: '/manifest.webmanifest' }],
    ['meta', { name: 'application-name', content: 'StreamCompanion' }],
    ['meta', { name: 'apple-mobile-web-app-title', content: 'StreamCompanion' }],
    ['meta', { name: 'apple-mobile-web-app-status-bar-style', content: 'black' }],
    ['link', { rel: 'apple-touch-icon', href: `/images/icons/apple-touch-icon.png` }],
    [
      'link',
      {
        rel: 'mask-icon',
        href: '/images/icons/safari-pinned-tab.svg',
        color: '#3eaf7c',
      },
    ],
    ['meta', { name: 'msapplication-TileColor', content: '#3eaf7c' }],
    ['meta', { name: 'theme-color', content: '#3eaf7c' }],
  ],
  themeConfig: {
    logo: 'images/logo.svg',
    repo: 'Piotrekol/StreamCompanion',
    docsBranch: 'master',
    docsDir: 'docs/docs',
    locales: {
      '/': {
        navbar: [
          {
            text: 'Discord',
            link: 'https://discord.gg/N854wYZ',
          },
        ],
        sidebar: sidebar.en,
        editLinkText: 'Edit this page on GitHub',
      },
    },
  },
});
