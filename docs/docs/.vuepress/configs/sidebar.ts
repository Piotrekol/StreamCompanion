import type { SidebarConfig } from '@vuepress/theme-default';

export const en: SidebarConfig = {
  '/guide/': [
    {
      text: 'Guide',
      children: [
        '/guide/README.md',
        '/guide/getting-started.md',
        '/guide/configuration.md',
        '/guide/ingame-overlay.md',
        '/guide/additional-web-overlays.md',
        
        '/cookbook/',
        '/development/'
      ],
    },
  ],
  '/development/': [
    {
      text: 'Development',
      children: [
        '/development/gettingSource.md',
        {
          text: 'Documentation',
          children: [
            '/development/docs/',
          ],
        },
        {
          text: 'StreamCompanion',
          children: [
            '/development/SC/',
            '/development/SC/api.md',
            '/development/SC/creating-plugin.md',
          ],
        },
      ],
    },
  ],
  '/cookbook/': [
    {
      text: 'Cookbook',
      children: [
        '/cookbook/README.md',
      ],
    },
  ],
};
