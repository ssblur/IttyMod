using System;
using TinyLife.Objects;
using System.Collections.Generic;

namespace IttyMod.Tricks {
    class FirstNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, PersonLike owner, List<MapObject> involved)
        {
            return owner.FirstName;
        }
    }

    class LastNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, PersonLike owner, List<MapObject> involved)
        {
            return owner.FirstName;
        }
    }
    
    class TargetFirstNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, PersonLike owner, List<MapObject> involved)
        {
            int index = Int32.Parse(trick);
            if(index < involved.Count && involved[index] is PersonLike person)
                return person.FirstName;
            return "";
        }
    }

    class TargetLastNameTrick : Trick
    {
        public override SubstitutionOrSuccess parse(string trick, PersonLike owner, List<MapObject> involved)
        {
            int index = Int32.Parse(trick);
            if(index < involved.Count && involved[index] is PersonLike person)
                return person.LastName;
            return "";
        }
    }

    class TargetTagTrick : Trick 
    {
        public override SubstitutionOrSuccess parse(string trick, PersonLike owner, List<MapObject> involved)
        {
            int index = Int32.Parse(trick);
            if(index < involved.Count && involved[index] is Person person) {
                var nameTag = String.Format("@{0}{1}", person.FirstName, person.LastName);
                return nameTag.Substring(0, Math.Min(13, nameTag.Length));
            }
            return "";
        }
    }
}