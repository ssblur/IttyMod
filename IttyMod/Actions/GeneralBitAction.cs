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

            IttyMod.BitDisposition bitDisposition = (
                owner.Emotion == EmotionType.Happy
                || owner.Emotion == EmotionType.Creative
                || owner.Emotion == EmotionType.Energetic
                ) ? IttyMod.BitDisposition.POSITIVE : (
                    (
                    owner.Emotion == EmotionType.Angry
                    || owner.Emotion == EmotionType.Bored
                    || owner.Emotion == EmotionType.Embarrassed
                    || owner.Emotion == EmotionType.Sad
                    ) ? IttyMod.BitDisposition.NEGATIVE : IttyMod.BitDisposition.NEUTRAL
                );

            string userTag = String.Format("@{0}{1}", owner.FirstName, owner.LastName);
            string bit = String.Format("{0}: {1}", userTag, IttyMod.LocalizedBit("General", bitDisposition));
            return bit;
        }
    }
}