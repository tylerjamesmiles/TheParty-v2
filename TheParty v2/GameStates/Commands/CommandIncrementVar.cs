using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandIncrementVar : Command<TheParty>
    {
        string VarName;
        int IncrBy;

        public CommandIncrementVar(string varName, string incrBy)
        {
            VarName = varName;
            IncrBy = int.Parse(incrBy);
        }

        public override void Update(TheParty client, float deltaTime)
        {

            GameContent.Variables[VarName] += IncrBy;
            Done = true;
        }
    }
}
