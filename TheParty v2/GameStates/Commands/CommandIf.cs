using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandIf : Command<TheParty>
    {
        string VarName;
        enum Type { Switch, Variable, Undefined };
        Type VarType;
        string Operator;
        string RHValue;
        List<Command<TheParty>> Commands;
    
        public CommandIf(string varName, string op, string rhValue, List<Command<TheParty>> commands)
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
                client.CommandQueue.AddCommands(Commands);

            Done = true;
        }
    }
}
