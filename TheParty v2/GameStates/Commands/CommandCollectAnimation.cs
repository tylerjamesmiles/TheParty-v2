using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandCollectAnimation : Command<TheParty>
    {
        Vector2 Pos;
        int Row;

        public CommandCollectAnimation(Vector2 pos, string kind)
        {
            Pos = pos;
            Row =
                kind == "Coin" ? 0 :
                kind == "Meat" ? 1 :
                0;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            client.CollectionAnimations.Add(
                Pos, 
                new SpriteAnimation(Row, 8, 0.1f));
            Done = true;
        }
    }
}
