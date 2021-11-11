using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandSetVar : Command<TheParty>
    {
        enum Type { Switch, Variable, Undefined };
        Type VarType;
        string VarName;
        string NewValue;

        public CommandSetVar(string varName, string newValue)
        {
            VarType =
                GameContent.Switches.ContainsKey(varName) ? Type.Switch :
                GameContent.Variables.ContainsKey(varName) ? Type.Variable :
                Type.Undefined;
            VarName = varName;
            NewValue = newValue;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if (NewValue == "false")
                ;

            if (VarType == Type.Switch)
                GameContent.Switches[VarName] = bool.Parse(NewValue);
            else if (VarType == Type.Variable)
                GameContent.Variables[VarName] = int.Parse(NewValue);

            Done = true;
        }
    }
}
