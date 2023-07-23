using System;
using TinyLife.Objects;
using System.Collections.Generic;

namespace IttyMod.Tricks {
    class NoPostTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, Person owner, List<MapObject> involved)
        {
            IttyMod.Logger.Info($"Delaying {owner}'s Bits");
            owner.SetData("randomBitTimer", TinyLife.GameImpl.Instance.CurrentMap.Time + new TimeSpan(0, Int32.Parse(trick), 0));
            return "";
        }
    }
}