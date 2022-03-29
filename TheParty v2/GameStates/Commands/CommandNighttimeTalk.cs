using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheParty_v2
{
    class TalkData
    {
        public string Member { get; set; }
        public int Priority { get; set; }
        public List<string> Conditions { get; set; }
        public bool OnlyHappensOnce { get; set; }
        public string Script { get; set; }
    }

    class CommandNighttimeTalk : Command<TheParty>
    {
        public override void Enter(TheParty client)
        {
            // Choose an appropriate talk
            List<TalkData> ToSort = new List<TalkData>(GameContent.Talks);

            // Remove any that feature a member not currently in the party
            Party Party = client.Player.ActiveParty;
            List<string> MemberNames = Party.Members.ConvertAll(m => m.Name);
            ToSort.RemoveAll(t => !MemberNames.Exists(n => n == t.Member));

            // Remove any whose conditions aren't all true
            ToSort.RemoveAll(
                t => !t.Conditions.TrueForAll(
                    c => CommandIf.ConditionTrue(c.Split()[0], c.Split()[1], c.Split()[2], client)));

            // Sort by priority
            ToSort.Sort((t1, t2) => t1.Priority.CompareTo(t2.Priority));

            if (ToSort.Count > 0)
            {
                // Choose the top of the sorted list
                TalkData ChosenTalk = ToSort[0];

                // Enqueue its commands
                var Commands = ScriptInterpreter.Interpret(client, null, ChosenTalk.Script);

                client.CommandQueue.PushCommands(Commands);
            }

            Entered = true;
            Done = true;
        }
    }
}
