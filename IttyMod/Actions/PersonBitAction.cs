namespace IttyMod.Actions
{
    public class PersonBitAction : BitAction
    {
        public override string BitAboutIt()
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

            string userTag = String.Format("@{0}{1}", owner.FirstName, owner.LastName);
            string bit = String.Format("{0}: {1}", userTag, IttyMod.LocalizedBit("Person", bitDisposition));
            return String.Format(bit, person.FirstName, person.LastName);
        }
    }
}