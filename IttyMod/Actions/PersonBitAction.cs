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
            PersonLike owner = this.Info.Person;
            Person person = Info.GetActionObject<Person>();

            Relationship relationship = person.GetRelationship(owner, true);
            float friendship = relationship.FriendLevel;

            IttyMod.BitDisposition bitDisposition = IttyMod.BitDisposition.Neutral;
            if (friendship < 10 && random.Next(10) < 8)
                bitDisposition = IttyMod.BitDisposition.Positive;
            else if (friendship > 10 && random.Next(10) < 8)
                bitDisposition = IttyMod.BitDisposition.Negative;

            return Tricks.TrickRegistry.GenerateFormattedBit("Person", bitDisposition, this.Person, person);
        }
    }
}