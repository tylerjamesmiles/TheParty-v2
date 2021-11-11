using System;
using System.Collections.Generic;
using System.Text;

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
}
