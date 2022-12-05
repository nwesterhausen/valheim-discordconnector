export default {
    title: 'Valheim Discord Connector',
    description: 'Connect your Valheim server to a Discord webhook.',

    themeConfig: {
        logo: '/logo.png',
        nav: [
            { text: 'Changelog', link: '/changelog' },
            { text: 'Configuring', link: '/config/' },
            {
                text: 'How-to Guides',
                items: [
                    { text: 'Get a Discord Webhook', link: '/how-to/webhook-instructions' },
                    { text: 'Edit Configuration in R2Modman', link: '/how-to/edit-config-r2modman' },
                    { text: 'Edit Configuration Manually', link: '/how-to/edit-config-notepad' }
                ]
            }
        ],
        sidebar: {
            '/how-to/': [
                {
                    text: 'How-to Guides',
                    items: [
                        { text: 'Get a Discord Webhook', link: '/how-to/webhook-instructions' },
                        { text: 'Edit Configuration in R2Modman', link: '/how-to/edit-config-r2modman' },
                        { text: 'Edit Configuration Manually', link: '/how-to/edit-config-notepad' }
                    ]
                }
            ],
            '/config/': [
                {
                    text: 'Config Files',
                    items: [
                        { text: 'Overview', link: '/config/' },
                        { text: 'Main', link: '/config/main' },
                        { text: 'Variables', link: '/config/variables' },
                        { text: 'Messages', link: '/config/messages' },
                        { text: 'Toggles', link: '/config/toggles' },
                        { text: 'Leader Boards', link: '/config/leader-boards' }
                    ]
                }
            ]
        },
        socialLinks: [
            { icon: 'github', link: 'https://github.com/nwesterhausen/valheim-discordconnector' }
        ],
        editLink: {
            pattern: 'https://github.com/nwesterhausen/valheim-discordconnector/edit/main/docs/:path',
            text: 'Edit this page on GitHub'
        },
        lastUpdatedText: 'Updated Date'
    }
}