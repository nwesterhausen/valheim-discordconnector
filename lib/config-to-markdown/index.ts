import * as fs from "fs";
import { configToMarkdown } from "./ConfigToMarkdown";
import { parseConfigFile } from "./ParseConfig";

const basePath = "../../docs/public/default-config";
const files = [
  "discordconnector",
  "discordconnector-leaderboards",
  "discordconnector-messages",
  "discordconnector-toggles",
  "discordconnector-variables",
];

if (!fs.existsSync("./out")) {
  fs.mkdirSync("./out");
}

for (const filename of files) {
  // Read in the file
  const configFile = fs.readFileSync(`${basePath}/${filename}.cfg`, "utf-8");

  // Parse the file
  const config = parseConfigFile(configFile);
  // Generate markdown output
  const markdown = configToMarkdown(config);

  // Write the output
  fs.writeFileSync(`./out/${filename}.md`, markdown);
}
