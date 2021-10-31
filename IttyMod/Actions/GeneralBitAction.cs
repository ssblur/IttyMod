using System;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Objects;

namespace IttyMod.Actions
{
    public class GeneralBitAction : BitAction
    {
        public GeneralBitAction(ActionType type, ActionInfo info) : base(type, info) { }
        protected override Bit BitAboutIt()
        {
            Person owner = this.Info.Person;

            IttyMod.BitDisposition bitDisposition = owner.Emotion.Category switch {
                EmotionType.TypeCategory.Negative => IttyMod.BitDisposition.NEGATIVE,
                EmotionType.TypeCategory.Positive => bitDisposition = IttyMod.BitDisposition.POSITIVE,
                _ => IttyMod.BitDisposition.NEUTRAL,
            };
            string bit = IttyMod.LocalizedBit("General", bitDisposition);
            return new Bit(bit, this.Person);
        }
    }
}