using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class CommandChangePlayerSprite : Command<TheParty>
    {
        string SpriteName;

        public CommandChangePlayerSprite(string spriteName)
        {
            SpriteName = spriteName;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            client.Player.Sprite.SpriteName = SpriteName;
            Done = true;
        }
    }

    class CommandChangeEntitySprite : Command<TheParty>
    {
        OgmoEntity Entity;
        string SpriteName;

        public CommandChangeEntitySprite(OgmoEntity entity, string spriteName)
        {
            Entity = entity;
            SpriteName = spriteName;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Entity.Sprite = new FourDirSprite2D(SpriteName, new Point(-16, -16));
            Done = true;
        }
    }
}
