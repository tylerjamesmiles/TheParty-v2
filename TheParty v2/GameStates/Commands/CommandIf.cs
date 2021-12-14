using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandIf : Command<TheParty>
    {
        string VarName;
        enum Type { Party, Switch, Variable, Undefined };
        Type VarType;
        string Operator;
        string RHValue;
        List<Command<TheParty>> Commands;
    
        public CommandIf(string varName, string op, string rhValue, List<Command<TheParty>> commands)
        {
            VarName = varName;
            VarType =
                varName.ToLower() == "party" ? Type.Party :
                GameContent.Switches.ContainsKey(VarName) ? Type.Switch :
                GameContent.Variables.ContainsKey(VarName) ? Type.Variable :
                Type.Undefined;
            Operator = op;
            RHValue = rhValue;
            Commands = commands;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            bool True = false;
            if (VarType == Type.Party)
            {
                List<Member> Members = client.Player.ActiveParty.Members;
                bool PartyAlive = Members.Exists(m => m.HP > 0);
                bool PartyHealed = Members.TrueForAll(m => m.HP == m.MaxHP);
                bool AnyoneDead = Members.Exists(m => m.HP == 0);

                switch (RHValue.ToLower())
                {
                    case "alive":
                        switch (Operator)
                        {
                            case "==": True = PartyAlive; break;
                            case "!=": True = !PartyAlive; break;
                        }
                        break;

                    case "dead":
                        switch (Operator)
                        {
                            case "==": True = !PartyAlive; break;
                            case "!=": True = PartyAlive; break;
                        }
                        break;

                    case "healed":
                        switch (Operator)
                        {
                            case "==": True = PartyHealed; break;
                            case "!=": True = !PartyHealed; break;
                        }
                        break;

                    case "anyonedead":
                        switch (Operator)
                        {
                            case "==": True = AnyoneDead; break;
                            case "!=": True = !AnyoneDead; break;
                        }
                        break;
                }

            }
            else if (VarType == Type.Switch)
            {
                bool LH = GameContent.Switches[VarName];
                bool RH = bool.Parse(RHValue);
                switch (Operator)
                {
                    case "==": True = LH == RH; break;
                    case "!=": True = LH != RH; break;
                }
            }
            else if (VarType == Type.Variable)
            {
                int LH = GameContent.Variables[VarName];
                int RH = int.Parse(RHValue);
                switch (Operator)
                {
                    case "==": True = LH == RH; break;
                    case "!=": True = LH != RH; break;
                    case "<": True = LH < RH; break;
                    case ">": True = LH > RH; break;
                    case "<=": True = LH <= RH; break;
                    case ">=": True = LH >= RH; break;
                }
            }

            if (True)
            {
                client.CommandQueue.PushCommands(Commands);
            }

            Done = true;
        }
    }

    class CommandWhile : Command<TheParty>
    {
        string VarName;
        enum Type { Switch, Variable, Undefined };
        Type VarType;
        string Operator;
        string RHValue;
        List<Command<TheParty>> Commands;

        public CommandWhile(string varName, string op, string rhValue, List<Command<TheParty>> commands)
        {
            VarName = varName;
            VarType =
                GameContent.Switches.ContainsKey(VarName) ? Type.Switch :
                GameContent.Variables.ContainsKey(VarName) ? Type.Variable :
                Type.Undefined;
            Operator = op;
            RHValue = rhValue;
            Commands = commands;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            bool True = false;
            if (VarType == Type.Switch)
            {
                bool LH = GameContent.Switches[VarName];
                bool RH = bool.Parse(RHValue);
                switch (Operator)
                {
                    case "==": True = LH == RH; break;
                    case "!=": True = LH != RH; break;
                }
            }
            else if (VarType == Type.Variable)
            {
                int LH = GameContent.Variables[VarName];
                int RH = int.Parse(RHValue);
                switch (Operator)
                {
                    case "==": True = LH == RH; break;
                    case "!=": True = LH != RH; break;
                    case "<": True = LH < RH; break;
                    case ">": True = LH > RH; break;
                    case "<=": True = LH <= RH; break;
                    case ">=": True = LH >= RH; break;
                }
            }

            if (True)
            {
                Commands.ForEach(c => c.Entered = false);
                Commands.ForEach(c => c.Done = false);
                client.CommandQueue.PushCommand(new CommandWhile(VarName, Operator, RHValue, Commands));
                client.CommandQueue.PushCommands(Commands);
            }

            Done = true;
        }
    }
}
