using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using DevServer.Packets;

namespace DevServer.Database
{
    public static class CharacterDatabase
    {
        private const string ConnectionString = "Data Source=characters.db";

        public static void Initialize()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Characters (
                        CharacterKey INTEGER PRIMARY KEY,
                        Gender_JobId INTEGER NOT NULL,
                        Faction INTEGER NOT NULL,
                        Name TEXT NOT NULL,
                        Guild TEXT,
                        Face_TatooId INTEGER NOT NULL,
                        HairId INTEGER NOT NULL,
                        HairColor INTEGER NOT NULL,
                        SkinColor INTEGER NOT NULL,
                        Size INTEGER NOT NULL,
                        Weigth INTEGER NOT NULL,
                        ModelTorsoId INTEGER NOT NULL,
                        ModelHandId INTEGER NOT NULL,
                        ModelShoeId INTEGER NOT NULL,
                        ModelLegId INTEGER NOT NULL,
                        Level INTEGER NOT NULL,
                        Slot INTEGER NOT NULL,
                        DeleteTime INTEGER NOT NULL,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )
                ";
                createTableCmd.ExecuteNonQuery();

                Log.WriteSuccess("[Database] Character database initialized successfully.");
            }
        }

        public static void SaveCharacter(DPKUZ_USER_RS_CHARLIST_DATA character)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT OR REPLACE INTO Characters (
                        CharacterKey, Gender_JobId, Faction, Name, Guild,
                        Face_TatooId, HairId, HairColor, SkinColor, Size, Weigth,
                        ModelTorsoId, ModelHandId, ModelShoeId, ModelLegId,
                        Level, Slot, DeleteTime
                    ) VALUES (
                        @CharacterKey, @Gender_JobId, @Faction, @Name, @Guild,
                        @Face_TatooId, @HairId, @HairColor, @SkinColor, @Size, @Weigth,
                        @ModelTorsoId, @ModelHandId, @ModelShoeId, @ModelLegId,
                        @Level, @Slot, @DeleteTime
                    )
                ";

                insertCmd.Parameters.AddWithValue("@CharacterKey", character.Key);
                insertCmd.Parameters.AddWithValue("@Gender_JobId", character.Gender_JobId);
                insertCmd.Parameters.AddWithValue("@Faction", character.Faction);
                insertCmd.Parameters.AddWithValue("@Name", character.Name ?? "");
                insertCmd.Parameters.AddWithValue("@Guild", character.Guild ?? "");
                insertCmd.Parameters.AddWithValue("@Face_TatooId", character.Face_TatooId);
                insertCmd.Parameters.AddWithValue("@HairId", character.HairId);
                insertCmd.Parameters.AddWithValue("@HairColor", character.HairColor);
                insertCmd.Parameters.AddWithValue("@SkinColor", character.SkinColor);
                insertCmd.Parameters.AddWithValue("@Size", character.Size);
                insertCmd.Parameters.AddWithValue("@Weigth", character.Weigth);
                insertCmd.Parameters.AddWithValue("@ModelTorsoId", character.ModelTorsoId);
                insertCmd.Parameters.AddWithValue("@ModelHandId", character.ModelHandId);
                insertCmd.Parameters.AddWithValue("@ModelShoeId", character.ModelShoeId);
                insertCmd.Parameters.AddWithValue("@ModelLegId", character.ModelLegId);
                insertCmd.Parameters.AddWithValue("@Level", character.Level);
                insertCmd.Parameters.AddWithValue("@Slot", character.Slot);
                insertCmd.Parameters.AddWithValue("@DeleteTime", character.DeleteTime);

                insertCmd.ExecuteNonQuery();

                Log.WriteSuccess("[Database] Character saved: {0} (Key: {1})", character.Name, character.Key);
            }
        }

        public static List<DPKUZ_USER_RS_CHARLIST_DATA> LoadAllCharacters()
        {
            var characters = new List<DPKUZ_USER_RS_CHARLIST_DATA>();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT * FROM Characters ORDER BY Slot";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var character = new DPKUZ_USER_RS_CHARLIST_DATA
                        {
                            Key = reader.GetInt32(0),
                            Gender_JobId = (short)reader.GetInt32(1),
                            Faction = (short)reader.GetInt32(2),
                            Name = reader.GetString(3),
                            Guild = reader.GetString(4),
                            Face_TatooId = (short)reader.GetInt32(5),
                            HairId = (short)reader.GetInt32(6),
                            HairColor = reader.GetInt32(7),
                            SkinColor = reader.GetInt32(8),
                            Size = reader.GetInt32(9),
                            Weigth = reader.GetInt32(10),
                            ModelTorsoId = reader.GetInt32(11),
                            ModelHandId = reader.GetInt32(12),
                            ModelShoeId = reader.GetInt32(13),
                            ModelLegId = reader.GetInt32(14),
                           Level = (byte)reader.GetInt32(15),
                            Slot = (byte)reader.GetInt32(16),
                            DeleteTime = (short)reader.GetInt32(17)
                        };

                        characters.Add(character);
                    }
                }

                Log.WriteInfo("[Database] Loaded {0} characters from database.", characters.Count);
            }

            return characters;
        }

        public static int GetMaxCharacterKey()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT MAX(CharacterKey) FROM Characters";

                var result = selectCmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 29748867;
            }
        }
    }
}
