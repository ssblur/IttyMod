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
        protected override string BitAboutIt()
        {
            Person owner = this.Info.Person;

            IttyMod.BitDisposition bitDisposition;
            switch(owner.Emotion.Category) {
                case(EmotionType.TypeCategory.Negative): 
                    bitDisposition = IttyMod.BitDisposition.NEGATIVE;
                    break;
                case(EmotionType.TypeCategory.Positive):
                    bitDisposition = IttyMod.BitDisposition.POSITIVE;
                    break;
                default:
                    bitDisposition = IttyMod.BitDisposition.NEUTRAL;
            }

            string userTag = String.Format("@{0}{1}", owner.FirstName, owner.LastName);
            string bit = String.Format("{0}: {1}", userTag, IttyMod.LocalizedBit("General", bitDisposition));
            return bit;
        }
    }
}