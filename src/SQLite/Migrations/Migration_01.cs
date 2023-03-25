﻿
namespace DiscordConnector.SQLite.Migrations;

class Migration_01 : IMigration
{
    public int MigrationId => 1;
    public string MigrateUp => @"CREATE TABLE players (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name VARCHAR NOT NULL,
  hostname VARCHAR NOT NULL
);

CREATE TABLE joins (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  player_id INTEGER REFERENCES players(id),
  x NUMBER NOT NULL DEFAULT 0.0,
  y NUMBER NOT NULL DEFAULT 0.0,
  z NUMBER NOT NULL DEFAULT 0.0,
  joined_at DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE leaves (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  player_id INTEGER REFERENCES players(id),
  x NUMBER NOT NULL DEFAULT 0.0,
  y NUMBER NOT NULL DEFAULT 0.0,
  z NUMBER NOT NULL DEFAULT 0.0,
  left_at DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE deaths (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  player_id INTEGER REFERENCES players(id),
  x NUMBER NOT NULL DEFAULT 0.0,
  y NUMBER NOT NULL DEFAULT 0.0,
  z NUMBER NOT NULL DEFAULT 0.0,
  died_at DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE shouts (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  player_id INTEGER REFERENCES players(id),
  x NUMBER NOT NULL DEFAULT 0.0,
  y NUMBER NOT NULL DEFAULT 0.0,
  z NUMBER NOT NULL DEFAULT 0.0,
  shouted_at DATETIME NOT NULL DEFAULT (datetime('now')),
  text VARCHAR NOT NULL
);

CREATE TABLE pings (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  player_id INTEGER REFERENCES players(id),
  x NUMBER NOT NULL DEFAULT 0.0,
  y NUMBER NOT NULL DEFAULT 0.0,
  z NUMBER NOT NULL DEFAULT 0.0,
  ping_x NUMBER NOT NULL DEFAULT 0.0,
  ping_y NUMBER NOT NULL DEFAULT 0.0,
  ping_z NUMBER NOT NULL DEFAULT 0.0,
  pinged_at DATETIME NOT NULL DEFAULT (datetime('now'))
);";

    public string MigrateDown => @"DROP TABLE IF EXISTS pings;
DROP TABLE IF EXISTS shouts;
DROP TABLE IF EXISTS deaths;
DROP TABLE IF EXISTS leaves;
DROP TABLE IF EXISTS joins;
DROP TABLE IF EXISTS players;";
}
