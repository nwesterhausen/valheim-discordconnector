export interface Setting {
  settingName: string;
  settingType: string;
  defaultValue: string;
  description: string;
  acceptableValues?: string[];
}

export interface Config {
  [heading: string]: Setting[];
}
