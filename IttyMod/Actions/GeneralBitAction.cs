using System;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Objects;
using TinyLife.Utilities;

namespace IttyMod.Actions
{
    public class GeneralBitAction : BitAction
    {
        public GeneralBitAction(ActionType type, ActionInfo info) : base(type, info) { }
        protected override Bit BitAboutIt()
        {
            Person owner = this.Info.Person;
            owner.SetTimeSpan("randomBitTimer", TinyLife.GameImpl.Instance.CurrentTime + new TimeSpan(0, 30, 0));

            IttyMod.BitDisposition bitDisposition = owner.Emotion.Category switch {
                EmotionType.TypeCategory.Negative => IttyMod.BitDisposition.NEGATIVE,
                EmotionType.TypeCategory.Positive => bitDisposition = IttyMod.BitDisposition.POSITIVE,
                _ => IttyMod.BitDisposition.NEUTRAL,
            };

            // Most of the time, we want Neutral bits, because there are a lot of them and people are frequently in moods.
            if(random.Next(50) > 10) {
                bitDisposition = IttyMod.BitDisposition.NEUTRAL;
            }
            string bit = IttyMod.LocalizedBit("General", bitDisposition);
            return new Bit(bit, this.Person);
        }
    }
}