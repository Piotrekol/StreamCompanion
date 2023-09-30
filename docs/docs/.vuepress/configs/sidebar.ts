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
            '/development/SC/creating-a-plugin.md',
            '/development/SC/event-flow.md',
            '/development/SC/types-rundown.md',            
            '/development/SC/linuxSupport.md',
          ],
        },
      ],
    },
  ]
};
