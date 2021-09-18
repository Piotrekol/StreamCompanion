import type { SidebarConfig } from '@vuepress/theme-default';

export const en: SidebarConfig = {
  '/guide/': [
    {
      text: 'Guide',
      children: [
        '/guide/README.md',
        '/guide/getting-started.md',
        '/guide/configuration.md',
        '/guide/in-game-overlays.md',
        '/guide/gamma.md',
        '/guide/tournament-mode.md',
        
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
            '/development/SC/linuxSupport.md',
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
