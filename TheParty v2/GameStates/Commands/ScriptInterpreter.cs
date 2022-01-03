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
            List<OgmoEntity> Entities = EntityLayer.entities;

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

                    case "befaded":
                        ResultList.Add(new CommandBeFaded());
                        break;

                    case "showscreen":
                        ResultList.Add(new CommandShowScreen());
                        break;

                    case "dialogue":
                        ResultList.Add(new CommandDialogue(Arguments));
                        break;

                    case "save":
                        ResultList.Add(new CommandSave());
                        break;

                    case "load":
                        ResultList.Add(new CommandLoad());
                        break;

                    case "move":
                        if (Arguments[0] == "Player")
                            ResultList.Add(new CommandMovePlayer(Arguments[1]));
                        else
                            ResultList.Add(new CommandMoveEntity(EntityFromName(Arguments[0], EntityLayer, caller), Arguments[1]));
                        break;

                    

                    case "playanimation":
                        ResultList.Add(new CommandPlayAnimation(EntityFromName(Arguments[0], EntityLayer, caller), Arguments[1], Arguments[2]));
                        break;

                    case "clearqueue":
                        ResultList.Add(new CommandClearQueue());
                        break;

                    case "freeze":
                        foreach (string entityName in Arguments)
                            if (entityName.ToLower() == "player")
                                ResultList.Add(new CommandFreezePlayer());
                            else if (entityName.ToLower() == "all")
                                ResultList.Add(new CommandFreezeAll(Entities));
                            else
                                ResultList.Add(new CommandFreeze(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "unfreeze":
                        foreach (string entityName in Arguments)
                            if (entityName.ToLower() == "player")
                                ResultList.Add(new CommandUnfreezePlayer());
                            else if (entityName.ToLower() == "all")
                                ResultList.Add(new CommandUnFreezeAll(Entities));
                            else
                                ResultList.Add(new CommandUnFreeze(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "face":
                            ResultList.Add(new CommandFace(EntityFromName(Arguments[0], EntityLayer, caller), Arguments[1]));
                        break;

                    case "faceplayer":
                        foreach (string entityName in Arguments)
                            ResultList.Add(new CommandFacePlayer(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "stopfacingplayer":
                        foreach (string entityName in Arguments)
                            ResultList.Add(new CommandStopFacingPlayer(EntityFromName(entityName, EntityLayer, caller)));
                        break;

                    case "decrementhunger":
                        ResultList.Add(new CommandFreezePlayer());
                        ResultList.Add(new CommandDecrementHunger());
                        ResultList.Add(new CommandUnfreezePlayer());
                        break;

                    case "healdead":
                        ResultList.Add(new CommandHealDead());
                        break;

                    case "battle":
                        ResultList.Add(new CommandPlayMusic("MonsterBattle"));
                        ResultList.Add(new CommandFreezeAll(Entities));
                        ResultList.Add(new CommandFreezePlayer());
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandBeFaded());

                        // Normal battle
                        if (Arguments.Length == 1)
                        {
                            ResultList.Add(new CommandBattle(Arguments[0], "DidntFlee"));
                            ResultList.Add(new CommandFadeOutMusic());
                            ResultList.Add(new CommandIf("Party", "==", "Dead",
                                new List<Command<TheParty>>
                                {
                                    new CommandShowScreen(),
                                    new CommandGameOver()
                                }));
                            ResultList.Add(new CommandIf("DidntFlee", "==", "true",
                                new List<Command<TheParty>>
                                {
                                    new CommandLevelUp()
                                }));
                            ResultList.Add(new CommandDecrementHunger());
                        }
                        // If battle operates a switch, only do level-up rigamarole if successful battle
                        else if (Arguments.Length == 2)
                        {
                            ResultList.Add(new CommandBattle(Arguments[0], Arguments[1]));
                            ResultList.Add(new CommandIf("Party", "==", "Dead",
                            new List<Command<TheParty>>
                            {
                                    new CommandShowScreen(),
                                    new CommandGameOver()
                            }));
                            ResultList.Add(new CommandIf(Arguments[1], "==", "true",
                                new List<Command<TheParty>>
                                {
                                    new CommandLevelUp(),
                                    new CommandDecrementHunger(),
                                }));
                        }

                        ResultList.Add(new CommandPlayMapMusic());
                        ResultList.Add(new CommandFadeInMusic());
                        ResultList.Add(new CommandShowScreen());
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        ResultList.Add(new CommandUnFreezeAll(Entities));
                        ResultList.Add(new CommandUnfreezePlayer());

                        break;

                    case "teleport":
                        string OldSong = game.CurrentMap.values["Song"];
                        string NewSong = GameContent.Maps[Arguments[0]].values["Song"];

                        ResultList.Add(new CommandFreezePlayer());
                        if (OldSong != NewSong)
                            ResultList.Add(new CommandFadeOutMusic());
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandBeFaded());
                        ResultList.Add(new CommandWait(0.5f));
                        if (OldSong != NewSong)
                            ResultList.Add(new CommandFadeInMusic());
                        ResultList.Add(new CommandTeleport(Arguments[0], int.Parse(Arguments[1]), int.Parse(Arguments[2])));
                        ResultList.Add(new CommandShowScreen());
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        ResultList.Add(new CommandUnfreezePlayer());
                        break;

                    case "set":
                        ResultList.Add(new CommandSetVar(Arguments[0], Arguments[1]));
                        break;

                    case "incr":
                        ResultList.Add(new CommandIncrementVar(Arguments[0], Arguments[1]));
                        break;

                    case "erase":
                        int Id = EntityFromName(Arguments[0], EntityLayer, caller).EntityId;
                        ResultList.Add(new CommandErase(Id));
                        break;

                    case "collectanimation":
                        ResultList.Add(new CommandCollectAnimation(game.Player.Transform.Position, Arguments[0]));
                        break;

                    case "gameover":
                        ResultList.Add(new CommandGameOver());
                        break;

                    case "additem":
                        ResultList.Add(new CommandAddItem(Arguments[0]));
                        break;

                    case "chest":
                        ResultList.Add(new CommandFace(caller, "Down"));
                        ResultList.Add(new CommandWait(0.2f));
                        ResultList.Add(new CommandFace(caller, "Left"));
                        ResultList.Add(new CommandWait(0.2f));
                        ResultList.Add(new CommandAddItem(Arguments[0]));
                        ResultList.Add(new CommandDialogue("The party found a " + Arguments[0] + "!"));
                        ResultList.Add(new CommandErase(caller.EntityId));
                        break;

                    case "hitpartyhp":
                        ResultList.Add(new CommandHitPartyHP(int.Parse(Arguments[0])));
                        break;

                    case "inn":
                        ResultList.Add(new CommandInn());
                        break;

                    case "addpartymember":
                        ResultList.Add(new CommandAddPartyMember(Arguments[0]));
                        break;

                    case "soundeffect":
                        ResultList.Add(new CommandPlaySoundEffect(Arguments[0]));
                        break;

                    case "changesprite":
                        if (Arguments[0] == "Player")
                            ResultList.Add(new CommandChangePlayerSprite(Arguments[1]));
                        else
                            ResultList.Add(new CommandChangeEntitySprite(EntityFromName(Arguments[0], EntityLayer, caller), Arguments[1]));
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

                    case "while":
                        string WhileSubScript = "";
                        for (
                            int subLine = line + 1;
                            subLine < Lines.Length && Lines[subLine] != "" && Lines[subLine].Substring(0, 3) == "   ";
                            subLine++)
                            WhileSubScript += Lines[subLine].Remove(0, 3) + '\n';    // remove first character (\t)
                        var WhileCommands = Interpret(game, caller, WhileSubScript);
                        string[] WhileTokens = Arguments[0].Split(' ');
                        ResultList.Add(new CommandWhile(WhileTokens[0], WhileTokens[1], WhileTokens[2], WhileCommands));
                        break;

                    case "choice":
                        string Arg1 = Arguments[0];
                        bool Show = Arg1.Split(' ')[0].ToLower() == "show";

                        string[] Choices;
                        if (Show)
                        {
                            Choices = new string[Arguments.Length - 1];
                            for (int i = 1; i < Arguments.Length; i++)
                                Choices[i - 1] = Arguments[i];
                        }
                        else
                            Choices = Arguments;


                        List<Command<TheParty>>[] ChoiceCommands = new List<Command<TheParty>>[Choices.Length];
                        int CommandIdx = 0;
                        // look through all lines below
                        for (int subLine = line +1 ; subLine < Lines.Length; subLine++)
                        {
                            // if line begins with "   Case", gather commands below it
                            if (Lines[subLine].Length > 0 &&
                                Lines[subLine].Substring(0, 7) == "   Case")
                            {
                                string SubSubScript = "";
                                for (int subsub = subLine + 1; subsub < Lines.Length; subsub++)
                                {
                                    if (Lines[subsub].Length > 0 && 
                                        Lines[subsub].Substring(0, 6) == "      ")
                                        SubSubScript += Lines[subsub].Remove(0, 6) + '\n';
                                    else
                                        break;
                                }
                                ChoiceCommands[CommandIdx] = Interpret(game, caller, SubSubScript);
                                CommandIdx++;
                            }
                            
                        }

                        ResultList.Add(new CommandFreezePlayer());

                        if (Show)
                            ResultList.Add(new CommandChoice(Choices, Arguments[0], ChoiceCommands));
                        else
                            ResultList.Add(new CommandChoice(Choices, null, ChoiceCommands));

                        ResultList.Add(new CommandUnfreezePlayer());
                        break;

                    case "levelup":
                        ResultList.Add(new CommandFreezePlayer());
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandLevelUp());
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        ResultList.Add(new CommandUnfreezePlayer());
                        break;

                    case "daypass":
                        ResultList.Add(new CommandFadeOutMusic());
                        ResultList.Add(new CommandFade(CommandFade.Direction.Out));
                        ResultList.Add(new CommandBeFaded());
                        ResultList.Add(new CommandDayPass());
                        ResultList.Add(new CommandTeleport(Arguments[0], int.Parse(Arguments[1]), int.Parse(Arguments[2])));
                        ResultList.Add(new CommandFadeInMusic());
                        ResultList.Add(new CommandShowScreen());
                        ResultList.Add(new CommandFade(CommandFade.Direction.In));
                        ResultList.Add(new CommandDialogue(GUIDialogueBox.Position.SkinnyTop, "*DaysRemaining days until the world ends."));
                        break;

                    case "esc":
                        ResultList.Add(new CommandESC());
                        break;
                }

            }

            //ResultList.Add(new CommandWait(0.0001f));
            return ResultList;
        }
    }
}
