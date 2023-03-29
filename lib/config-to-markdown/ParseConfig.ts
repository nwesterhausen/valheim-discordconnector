import { Config, Setting } from "./types/config";

const sectionHeadingRegex = /\[(.*)\]/s;

export function parseConfigFile(fileContent: string): Config {
  // Replace any carriage return characters with an empty string
  fileContent = fileContent.replace(/\r/g, "");

  const config: Config = {};

  // split content on new lines
  const lines = fileContent.split("\n");

  let currentHeading = "";
  let currentDescription = "";
  let currentSetting: Setting = {
    defaultValue: "",
    description: "",
    settingName: "",
    settingType: "",
  };
  for (const line of lines) {
    if (line.length === 0) {
      continue;
    }

    if (line.startsWith("[")) {
      const match = line.match(sectionHeadingRegex);
      if (match) {
        const heading = match[1];
        // Add the heading and set current heading
        config[heading] = [];
        currentHeading = heading;
      }
      continue;
    }

    if (line.startsWith("##")) {
      // reset currentSetting if it needs it
      if (currentSetting.description.length > 0) {
        // Append setting to Config
        config[currentHeading].push(currentSetting);
        // Reset current setting
        currentSetting = {
          defaultValue: "",
          description: "",
          settingName: "",
          settingType: "",
        };
      }
      currentDescription += line.substring(2);
      continue;
    }

    if (line.startsWith("#")) {
      // reset current description if it needs it
      if (currentDescription.length > 0) {
        // apply description
        currentSetting.description = currentDescription;
        // reset description
        currentDescription = "";
      }

      if (line.startsWith("# Setting type: ")) {
        const settingType = line.substring(16);
        currentSetting.settingType = settingType;
        continue;
      }
      if (line.startsWith("# Default value: ")) {
        const defaultValue = line.substring(17);
        currentSetting.defaultValue = defaultValue;
        continue;
      }
      if (line.startsWith("# Acceptable values: ")) {
        const acceptableValues = line.substring(21);
        currentSetting.acceptableValues = acceptableValues.split(",");
        continue;
      }
    }
    if (line.indexOf("=") > -1) {
      const settingName = line.split("=")[0].trim();
      currentSetting.settingName = settingName;
      continue;
    }
  }
  if (currentSetting.description.length > 0) {
    config[currentHeading].push(currentSetting);
  }

  return config;
}
