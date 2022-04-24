using System;
using TinyLife.Objects;
using System.Collections.Generic;

namespace IttyMod.Tricks {
    class FirstNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, Person owner, List<MapObject> involved)
        {
            return owner.FirstName;
        }
    }

    class LastNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, Person owner, List<MapObject> involved)
        {
            return owner.FirstName;
        }
    }
    
    class TargetFirstNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, Person owner, List<MapObject> involved)
        {
            int index = Int32.Parse(trick);
            if(index < involved.Count && involved[index] is Person person)
                return person.FirstName;
            return "";
        }
    }

    class TargetLastNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, Person owner, List<MapObject> involved)
        {
            int index = Int32.Parse(trick);
            if(index < involved.Count && involved[index] is Person person)
                return person.LastName;
            return "";
        }
    }
}