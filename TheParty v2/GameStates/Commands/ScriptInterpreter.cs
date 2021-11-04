using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    static class ScriptInterpreter
    {
        private static OgmoEntity EntityFromName(string name, OgmoLayer entityLayer, OgmoEntity caller)
        {
            return 
                (name.ToLower() == "me") ? caller : entityLayer.EntityWithName(name);
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

                if (Line == "")
                    continue;

                // skip any lines which begin with "/t"
                if (Line.Substring(0, 3) == "   ")
                    continue;

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
                    case "wait":
                        ResultList.Add(new CommandWait(float.Parse(Arguments[0])));
                        break;

                    case "fadein":
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        break;

                    case "fadeout":
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        break;

                    case "dialogue":
                        ResultList.Add(new CommandDialogue(Arguments));
                        break;

                    case "freeze":
                        foreach (string entityName in Arguments)
                            if (entityName.ToLower() == "player")
                                ResultList.Add(new CommandFreezePlayer());
                            else
                                ResultList.Add(new CommandFreeze(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "unfreeze":
                        foreach (string entityName in Arguments)
                            if (entityName.ToLower() == "player")
                                ResultList.Add(new CommandUnfreezePlayer());
                            else
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
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandBattle(Arguments[0]));
                        ResultList.Add(new CommandLevelUp());
                        ResultList.Add(new CommandDecrementHunger());
                        ResultList.Add(new CommandLeaveDead());
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        break;

                    case "teleport":
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandTeleport(Arguments[0], int.Parse(Arguments[1]), int.Parse(Arguments[2])));
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        break;

                    case "set":
                        ResultList.Add(new CommandSetVar(Arguments[0], Arguments[1]));
                        break;

                    case "incr":
                        ResultList.Add(new CommandIncrementVar(Arguments[0], Arguments[1]));
                        break;

                    case "erase":
                        string Name = (Arguments[0].ToLower() == "me") ? caller.values["Name"] : Arguments[0];
                        ResultList.Add(new CommandEraseMe(Name));
                        break;

                    case "hitpartyhp":
                        ResultList.Add(new CommandHitPartyHP(int.Parse(Arguments[0])));
                        break;

                    case "addpartymember":
                        ResultList.Add(new CommandAddPartyMember(Arguments[0]));
                        break;

                    case "if":
                        string SubScript = "";
                        for (
                            int subLine = line + 1; 
                            subLine < Lines.Length && Lines[subLine] != "" && Lines[subLine].Substring(0, 3) == "   "; 
                            subLine++)
                            SubScript += Lines[subLine].Remove(0, 3) + '\n';    // remove first character (\t)
                        var Commands = Interpret(game, caller, SubScript);
                        string[] Tokens = Arguments[0].Split(' ');
                        ResultList.Add(new CommandIf(Tokens[0], Tokens[1], Tokens[2], Commands));
                        break;

                    case "choice":
                        string[] Choices = Arguments;
                        List<Command<TheParty>>[] ChoiceCommands = new List<Command<TheParty>>[Choices.Length];
                        int CommandIdx = 0;
                        for (int subLine = line+1; subLine < Lines.Length && Lines[subLine].Substring(0, 3) == "   "; subLine++)
                        {
                            if (Lines[subLine].Substring(0, 7) == "   Case")
                            {
                                string SubSubScript = "";
                                for (int subsub = subLine+1; subsub < Lines.Length && Lines[subsub].Substring(0, 6) == "      "; subsub++)
                                {
                                    SubSubScript += Lines[subsub].Remove(0, 6) + '\n';
                                }
                                ChoiceCommands[CommandIdx] = Interpret(game, caller, SubSubScript);
                                CommandIdx++;
                            }
                        }

                        ResultList.Add(new CommandFreezePlayer());
                        ResultList.Add(new CommandChoice(Choices, ChoiceCommands));
                        ResultList.Add(new CommandUnfreezePlayer());
                        break;

                    case "levelup":
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandLevelUp());
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));

                        break;

                    case "daypass":
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandTeleport(Arguments[0], int.Parse(Arguments[1]), int.Parse(Arguments[2])));
                        ResultList.Add(new CommandDecrementHunger());
                        ResultList.Add(new CommandLeaveDead());
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        ResultList.Add(new CommandIncrementVar("DaysRemaining", "-1"));
                        ResultList.Add(new CommandDialogue("*DaysRemaining days until the world ends."));
                        break;
                }

            }

            ResultList.Add(new CommandWait(0.1f));
            return ResultList;
        }
    }
}
