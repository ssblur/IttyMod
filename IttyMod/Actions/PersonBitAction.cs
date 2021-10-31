using System;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Objects;

namespace IttyMod.Actions
{
    public class PersonBitAction : BitAction
    {
        public PersonBitAction(ActionType type, ActionInfo info) : base(type, info) { }
        protected override Bit BitAboutIt()
        {
            Person person = Info.GetActionObject<Person>();
            Person owner = this.Info.Person;

            Relationship relationship = person.GetRelationship(owner, true);
            float friendship = relationship.FriendLevel;

            IttyMod.BitDisposition bitDisposition = IttyMod.BitDisposition.NEUTRAL;
            if (friendship < 10 && random.Next(10) < 8)
                bitDisposition = IttyMod.BitDisposition.POSITIVE;
            else if (friendship > 10 && random.Next(10) < 8)
                bitDisposition = IttyMod.BitDisposition.NEGATIVE;

            string bit = String.Format(IttyMod.LocalizedBit("Person", bitDisposition), person.FirstName, person.LastName);
            return new Bit(bit, this.Person, person);
        }
    }
}