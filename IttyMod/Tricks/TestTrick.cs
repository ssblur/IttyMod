using System;
using TinyLife.Objects;
using System.Collections.Generic;

namespace IttyMod.Tricks {
    class TestTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, Person owner, List<MapObject> involved)
        {
            return "~wowweee~";
        }
    }
}