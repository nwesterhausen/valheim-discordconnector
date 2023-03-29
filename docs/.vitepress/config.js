export default {
    lang: 'en-US',
    title: 'Valheim Discord Connector',
    description: 'Connect your Valheim server to a Discord webhook.',

    lastUpdated: true,

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
                        {
                            text: 'Main', link: '/config/main', items: [
                                { text: "Webhook Event Definitions", link: '/config/webhook.events' }
                            ]
                        },
                        {
                            text: 'Message Content', link: '/config/messages',
                            items: [
                                { text: 'Server Status Messages', link: '/config/messages.server' },
                                { text: 'Random Event Messages', link: '/config/messages.events' },
                                { text: 'Player Event Messages', link: '/config/messages.player' },
                                { text: 'Player "Firsts" Messages', link: '/config/messages.playerfirsts' },
                            ]
                        },
                        {
                            text: 'Variable Customization', link: '/config/variables',
                            items: [
                                { text: 'Custom Variables', link: '/config/variables.custom' },
                                { text: '"Dynamic" Variables', link: '/config/variables.dynamic' },
                            ]
                        },
                        {
                            text: "Enable / Disable Toggles", link: '/config/toggles',
                            items: [
                                { text: 'Debug Messages', link: '/config/toggles.debugmessages' },
                                { text: 'Messages', link: '/config/toggles.messages' },
                                { text: 'Player Firsts', link: '/config/toggles.playerfirsts' },
                                { text: 'Position Data', link: '/config/toggles.position' },
                                { text: 'Stats', link: '/config/toggles.stats' },
                            ]
                        },
                        {
                            text: "Leaderboards", link: '/config/leaderboards',
                            items: [
                                { text: 'Player Activity', link: '/config/leaderboards.playerstats' },
                                { text: 'Custom Leaderboards', link: '/config/leaderboards.custom' },
                            ]
                        }
                    ]
                },

            ],
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