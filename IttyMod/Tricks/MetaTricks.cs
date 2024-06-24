using System;
using TinyLife.Objects;
using System.Collections.Generic;

namespace IttyMod.Tricks {
    class NoPostTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, PersonLike owner, List<MapObject> involved)
        {
            IttyMod.Logger.Info($"Delaying {owner}'s Bits");
            owner.SetData("randomBitTimer", owner.Map.Time + new TimeSpan(0, Int32.Parse(trick), 0));
            return "";
        }
    }
}