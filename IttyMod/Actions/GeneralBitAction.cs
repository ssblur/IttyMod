using System;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Objects;
using TinyLife.Utilities;

namespace IttyMod.Actions
{
    public class GeneralBitAction(ActionType type, ActionInfo info) : BitAction(type, info)
    {
        protected override Bit BitAboutIt()
        {
            var owner = Info.Person;

            var timer = owner.Map.Time + new TimeSpan(0, 30, 0);
            if(owner.GetData<TimeSpan?>("randomBitTimer") == null || owner.GetData<TimeSpan?>("randomBitTimer") < timer)
                owner.SetData("randomBitTimer", timer);

            var bitDisposition = owner.Emotion.Category switch {
                EmotionType.TypeCategory.Negative => IttyMod.BitDisposition.Negative,
                EmotionType.TypeCategory.Positive => IttyMod.BitDisposition.Positive,
                _ => IttyMod.BitDisposition.Neutral,
            };

            // Most of the time, we want Neutral bits, because there are a lot of them and people are frequently in moods.
            if(random.Next(50) > 10) {
                bitDisposition = IttyMod.BitDisposition.Neutral;
            }
            
            return Tricks.TrickRegistry.GenerateFormattedBit("General", bitDisposition, this.Person);
        }
    }
}