using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandIf : Command<TheParty>
    {
        string VarName;
        enum Type { Party, Commitment, Switch, Variable, Undefined };
        Type VarType;
        string Operator;
        string RHValue;
        List<Command<TheParty>> Commands;
    
        public CommandIf(string varName, string op, string rhValue, List<Command<TheParty>> commands)
        {
            VarName = varName;
            VarType = GetVarType(varName);
            Operator = op;
            RHValue = rhValue;
            Commands = commands;
        }

        private static Type GetVarType(string varName)
        {
            return
                varName.ToLower() == "party" ? Type.Party :
                varName.ToLower() == "commitment" ? Type.Commitment :
                GameContent.Switches.ContainsKey(varName) ? Type.Switch :
                GameContent.Variables.ContainsKey(varName) ? Type.Variable :
                Type.Undefined;
        }

        public static bool ConditionTrue(string varName, string op, string rhValue, TheParty client)
        {
            Type VarType = GetVarType(varName);

            bool True = false;
            if (VarType == Type.Party)
            {
                List<Member> Members = client.Player.ActiveParty.Members;
                bool PartyAlive = Members.Exists(m => m.HP > 0);
                bool PartyHealed = Members.TrueForAll(m => m.HP == m.MaxHP);
                bool AnyoneDead = Members.Exists(m => m.HP == 0);

                switch (rhValue.ToLower())
                {
                    case "alive":
                        switch (op)
                        {
                            case "==": True = PartyAlive; break;
                            case "!=": True = !PartyAlive; break;
                        }
                        break;

                    case "dead":
                        switch (op)
                        {
                            case "==": True = !PartyAlive; break;
                            case "!=": True = PartyAlive; break;
                        }
                        break;

                    case "healed":
                        switch (op)
                        {
                            case "==": True = PartyHealed; break;
                            case "!=": True = !PartyHealed; break;
                        }
                        break;

                    case "anyonedead":
                        switch (op)
                        {
                            case "==": True = AnyoneDead; break;
                            case "!=": True = !AnyoneDead; break;
                        }
                        break;
                }

            }
            else if (VarType == Type.Commitment)
            {
                List<Member> Members = client.Player.ActiveParty.Members;
                int TotalCommitment = 0;
                Members.ForEach(m => TotalCommitment += m.Stance);

                int LH = TotalCommitment;
                int RH = int.Parse(rhValue);

                switch (op)
                {
                    case "==": True = LH == RH; break;
                    case "!=": True = LH != RH; break;
                    case "<": True = LH < RH; break;
                    case ">": True = LH > RH; break;
                    case "<=": True = LH <= RH; break;
                    case ">=": True = LH >= RH; break;
                }
            }
            else if (VarType == Type.Switch)
            {
                bool LH = GameContent.Switches[varName];
                bool RH = bool.Parse(rhValue);
                switch (op)
                {
                    case "==": True = LH == RH; break;
                    case "!=": True = LH != RH; break;
                }
            }
            else if (VarType == Type.Variable)
            {
                int LH = GameContent.Variables[varName];
                int RH = int.Parse(rhValue);
                switch (op)
                {
                    case "==": True = LH == RH; break;
                    case "!=": True = LH != RH; break;
                    case "<": True = LH < RH; break;
                    case ">": True = LH > RH; break;
                    case "<=": True = LH <= RH; break;
                    case ">=": True = LH >= RH; break;
                }
            }

            return True;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if (ConditionTrue(VarName, Operator, RHValue, client))
            {
                Commands.ForEach(c => c.Done = false);
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
