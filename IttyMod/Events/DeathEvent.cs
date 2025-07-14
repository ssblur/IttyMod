using TinyLife.Actions;
using TinyLife.Objects;
using TinyLife;
using System;

namespace IttyMod.Events {
    public class DeathEvent {
        public static void OnActionsCompleted(CompletionType type, DieAction __instance) {
            PersonLike person = __instance.Person;
            Random random = new Random();
            foreach(Person owner in __instance.Person.Map.GetPeople()){
                if(random.Next(10) < 3) continue; // People are quite likely to post a bit when somebody dies.
                Relationship relationship = owner.GetRelationship(person, true);
                float friendship = relationship.FriendLevel;

                IttyMod.BitDisposition bitDisposition = IttyMod.BitDisposition.Neutral;
                if (friendship >= 10)
                    bitDisposition = IttyMod.BitDisposition.Positive;
                else if (friendship <= 0)
                    bitDisposition = IttyMod.BitDisposition.Negative;

                Bit bit = Tricks.TrickRegistry.GenerateFormattedBit("Death", bitDisposition, owner, person);
                
                BitManager.AddBit(bit);
            }
        }
    }
}