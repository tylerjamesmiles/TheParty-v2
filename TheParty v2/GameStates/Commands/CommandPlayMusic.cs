using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandPlayMusic : Command<TheParty>
    {
        string SongName;
        public CommandPlayMusic(string songName)
        {
            SongName = songName;
        }
        public override void Update(TheParty client, float deltaTime)
        {
            GameContent.PlaySong(SongName);
            Done = true;
        }
    }

    class CommandPlayMapMusic : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            string SongName = client.CurrentMap.values["Song"];
            GameContent.PlaySong(SongName);
            Done = true;
        }
    }

    class CommandFadeOutMusic : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            GameContent.FadeOutMusic();
            Done = true;
        }
    }

    class CommandFadeInMusic : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            GameContent.FadeInMusic();
            Done = true;
        }
    }

    class CommandBringMusicVolumeBackUp : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            GameContent.BringMusicVolumeBackUp();
            Done = true;
        }
    }

    class CommandPlaySoundEffect : Command<TheParty>
    {
        string SfxName;
        public CommandPlaySoundEffect(string name)
        {
            SfxName = name;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            GameContent.SoundEffects[SfxName].Play();
            Done = true;
        }
    }
}
