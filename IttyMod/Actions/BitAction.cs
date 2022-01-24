using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Objects;
using TinyLife.Uis;
using Action = TinyLife.Actions.Action;
using IttyMod.UIs;

namespace IttyMod.Actions {
    public abstract class BitAction : MultiAction {
        protected Random random;
        protected Person.Pose pose;

        public BitAction(ActionType type, ActionInfo info) : base(type, info) {
            random = new Random();
            if(random.Next(50) == 0)
                this.pose = Person.Pose.Laying;
            else
                this.pose = Person.Pose.Standing;
        }

        protected override IEnumerable<Action> CreateFirstActions() {
            yield return ActionType.GoHere.Construct(this.Info);
        }

        protected override void AndThenInitialize() {
            
        }

        protected override void AndThenUpdate(GameTime time, TimeSpan passedInGame, float speedModifier) {
            base.AndThenUpdate(time, passedInGame, speedModifier);
            this.Person.CurrentPose = pose;

            this.Person.RestoreNeed(NeedType.Entertainment, 0.5F, speedModifier);
            this.Person.RestoreNeed(NeedType.Social, 0.1F, speedModifier);
        }

        protected override CompletionType AndThenIsCompleted() {
            return this.CompleteInTime(TimeSpan.FromMinutes(1));
        }

        protected override void AndThenOnCompleted(CompletionType type) {
            Bit bit = BitAboutIt();
            BitManager.INSTANCE.AddBit(bit);

            base.AndThenOnCompleted(type);
        }

        /// <summary>
        ///     What should the Person performing this action Bit about?
        /// </summary>
        /// <returns>The string to post to Itty</returns>
        protected abstract Bit BitAboutIt();
    }
}