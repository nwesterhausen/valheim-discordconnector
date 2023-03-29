import { Config } from "./types/config";

export function configToMarkdown(config: Config): string {
  let markdown = "";

  // Output the config object as markdown
  for (const [sectionTitle, settings] of Object.entries(config)) {
    // Output the section heading as a markdown heading
    markdown += `## ${sectionTitle}\n\n`;

    // Output each setting as markdown
    for (const setting of settings) {
      markdown += `### ${setting.settingName}\n\n`;
      markdown += `Type: \`${setting.settingType}\`, default value: \`${setting.defaultValue}\`\n\n`;
      if (Array.isArray(setting.acceptableValues)) {
        markdown += `Acceptable values: ${setting.acceptableValues
          .map((v) => `\`${v}\``)
          .join(", ")}\n\n`;
      }
      markdown += `${setting.description}\n\n`;
    }
  }

  return markdown;
}
