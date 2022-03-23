using TinyLife.Actions;
using TinyLife.Objects;
using TinyLife;
using HarmonyLib;
using System;

namespace IttyMod.Events {
    [HarmonyPatch(typeof(DieAction), nameof(DieAction.OnCompleted))]
    public class DeathEvent {
        static void Prefix(CompletionType type, DieAction __instance) {
            Person person = __instance.Person;
            Random random = new Random();
            foreach(Person owner in GameImpl.Instance.Map.GetPeople()){
                if(random.Next(10) < 3) continue; // People are quite likely to post a bit when somebody dies.
                Relationship relationship = person.GetRelationship(owner, true);
                float friendship = relationship.FriendLevel;

                IttyMod.BitDisposition bitDisposition = IttyMod.BitDisposition.NEUTRAL;
                if (friendship >= 10)
                    bitDisposition = IttyMod.BitDisposition.POSITIVE;
                else if (friendship <= 0)
                    bitDisposition = IttyMod.BitDisposition.NEGATIVE;

                string bit = String.Format(IttyMod.LocalizedBit("Death", bitDisposition), person.FirstName, person.LastName);
                
                BitManager.AddBit(new Bit(bit, owner, person));
            }
        }
    }
}