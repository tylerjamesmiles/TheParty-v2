using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TheParty_v2
{
    struct JsonSwitches { public Dictionary<string, bool> Switches; }
    struct JsonVariables { public Dictionary<string, int> Variables; }

    static class GameContent
    {
        public static Dictionary<string, Texture2D> Sprites;
        public static Dictionary<string, Song> Songs;
        public static Dictionary<string, SoundEffect> SoundEffects;
        public static Dictionary<string, OgmoTileMap> Maps;
        public static Dictionary<string, bool> Switches;
        public static Dictionary<string, int> Variables;
        public static SpriteFont Font;

        private static Dictionary<string, T> LoadedWith<T>(ContentManager content, string folderName)
        {
            Dictionary<string, T> Result = new Dictionary<string, T>();
            foreach (string filePath in Directory.GetFiles("../../../Content/" + folderName))
            {
                string DotDot = "../../../";
                string ContentSlashFolder = "Content/" + folderName + @"\";
                string Extension = ".wtv"; // whatever
                string NoDotDot = filePath.Remove(0, DotDot.Length);
                string FileName = NoDotDot.Remove(0, ContentSlashFolder.Length);
                string NoExtension = FileName.Remove(FileName.Length - Extension.Length, Extension.Length);
                Result.Add(NoExtension, content.Load<T>(folderName + "/" + NoExtension));
            }
            return Result;
        }

        public static void Load(ContentManager content)
        {
            Sprites = LoadedWith<Texture2D>(content, "Art");
            Songs = LoadedWith<Song>(content, "Music");
            SoundEffects = LoadedWith<SoundEffect>(content, "Sfx");

            // Maps
            Maps = new Dictionary<string, OgmoTileMap>();
            foreach (string filePath in Directory.GetFiles("../../../Content/Maps"))
            {
                string Name = filePath.Remove(0, "../../../Content/Maps/".Length);

                if (Name != "TheParty.ogmo")
                {
                    OgmoTileMap Map = JsonUtility.GetDeserialized<OgmoTileMap>(filePath);
                    Map.Initialize();
                    Maps.Add(Name, Map);
                }
            }

            // Switches and Variables
            Switches = new Dictionary<string, bool>();
            string SwitchesData = File.ReadAllText("../../../Content/Data/Switches.ini");
            string[] SwitchLines = SwitchesData.Split('\n');
            foreach (string line in SwitchLines)
            {
                if (line == "") continue;
                string[] Words = line.Split(' ');
                bool Value = Words[1].ToLower() == "true";
                Switches.Add(Words[0], Value);
            }

            Variables = new Dictionary<string, int>();
            string VariablesData = File.ReadAllText("../../../Content/Data/Variables.ini");
            string[] VariablesLines = VariablesData.Split('\n');
            foreach (string line in VariablesLines)
            {
                if (line == "") continue;
                string[] Words = line.Split(' ');
                int Value = int.Parse(Words[1]);
                Variables.Add(Words[0], Value);
            }

            // Font
            Texture2D FontSprite = Sprites["Font"];
            Point GlyphSize = new Point(5, 9);
            int FontSpritePixelWidth = FontSprite.Bounds.Width;
            int FontSpriteGlyphWidth = FontSpritePixelWidth / GlyphSize.X;

            List<Rectangle> ListBounds = new List<Rectangle>();
            List<Rectangle> ListCropping = new List<Rectangle>();
            List<char> ListCharacters = new List<char>();
            int LineSpacing = 10;
            float Spacing = 1.0f;
            List<Vector3> ListKerning = new List<Vector3>();

            for (int ascii = 32; ascii < 127; ascii++)
            {
                int SourceID = ascii - 32;
                Point SourceTile = new Point(
                    SourceID % FontSpriteGlyphWidth, 
                    SourceID / FontSpriteGlyphWidth);
                Point SourcePos = new Point(
                    SourceTile.X * GlyphSize.X, 
                    SourceTile.Y * GlyphSize.Y);
                Rectangle Bounds = new Rectangle(SourcePos, GlyphSize);

                // Scan each collumn in the glyph for content, in order to set its width
                int Width = 0;
                Color[] Data = new Color[Bounds.Width * Bounds.Height];
                FontSprite.GetData(0, Bounds, Data, 0, Bounds.Width * Bounds.Height);

                for (int x = Bounds.Left; x < Bounds.Right; x++)
                {
                    bool FoundPixel = false;
                    int LocalX = x - Bounds.Left;

                    for (int y = Bounds.Top; y < Bounds.Bottom; y++)
                    {
                        int LocalY = y - Bounds.Top;
                        int Idx = LocalY * GlyphSize.X + LocalX;

                        if (Idx < Data.Length && Data[Idx] != new Color(0f, 0f, 0f, 0f))
                            FoundPixel = true;
                    }

                    if (FoundPixel)
                    {
                        Width = LocalX + 1;
                    }
                }

                Rectangle Cropping = new Rectangle(new Point(0, 0), new Point(Width, GlyphSize.Y));

                // Overwrite for Space
                if (ascii == 32)
                    Cropping = new Rectangle(new Point(0, 0), new Point(4, GlyphSize.Y));

                ListBounds.Add(Bounds);
                ListCropping.Add(Cropping);
                ListCharacters.Add((char)ascii);
                ListKerning.Add(new Vector3(0, Cropping.Width, 1));
            }


            Font = new SpriteFont(
                FontSprite,
                ListBounds,
                ListCropping,
                ListCharacters,
                LineSpacing,
                Spacing,
                ListKerning,
                null);
        }
    }


}
