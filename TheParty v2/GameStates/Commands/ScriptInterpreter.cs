using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    static class ScriptInterpreter
    {
        private static OgmoEntity EntityFromName(string name, OgmoLayer entityLayer, OgmoEntity caller)
        {
            return (name.ToLower() == "me") ? caller : entityLayer.EntityWithName(name);
        }

        public static List<Command<TheParty>> Interpret(TheParty game, OgmoEntity caller, string script)
        {
            OgmoTileMap CurrentMap = game.CurrentMap;
            OgmoLayer EntityLayer = CurrentMap.EntityLayer;

            List<Command<TheParty>> ResultList = new List<Command<TheParty>>();

            string[] Lines = script.Split('\n');

            for (int line = 0; line < Lines.Length; line++)
            {
                string Line = Lines[line];
                int PosOfParentheses = Line.IndexOf('(');
                string CommandName = Line.Remove(PosOfParentheses);

                string ArgumentsInP = Line.Remove(0, CommandName.Length);
                string ArgumentsNoP1 = ArgumentsInP.Remove(0, 1);
                string ArgumentsNoP2 = ArgumentsNoP1.Remove(ArgumentsNoP1.Length - 1, 1);
                string[] Arguments = ArgumentsNoP2.Split(',');
                for (int i = 0; i < Arguments.Length; i++)
                    Arguments[i] = Arguments[i].Trim();

                switch (CommandName.ToLower())
                {
                    case "dialogue":
                        ResultList.Add(new CommandDialogue(Arguments));
                        break;

                    case "freeze":
                        foreach (string entityName in Arguments)
                            ResultList.Add(new CommandFreeze(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "unfreeze":
                        foreach (string entityName in Arguments)
                            ResultList.Add(new CommandUnFreeze(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "faceplayer":
                        foreach (string entityName in Arguments)
                            ResultList.Add(new CommandFacePlayer(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "stopfacingplayer":
                        foreach (string entityName in Arguments)
                            ResultList.Add(new CommandStopFacingPlayer(EntityFromName(entityName, EntityLayer, caller)));
                        break;
                    case "battle":
                        ResultList.Add(new CommandBattle(Arguments[0]));
                        break;

                }

            }

            return ResultList;
        }
    }
}
