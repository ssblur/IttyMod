/// <summary>Unused, as Harmony is no longer included as a dependency</summary>
using TinyLife.Actions;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using HarmonyLib;
using System.Runtime.CompilerServices;
namespace IttyMod.Events {
    public class ActionEvent<ActionType> where ActionType : TinyLife.Actions.Action {
        Harmony Patcher;
        public ActionEvent() {
            Patcher = new Harmony("com.ssblur.ittymod");
            Type type = typeof(ActionType);
            Type patcherType = this.GetType();

            MethodInfo updateOriginal = type.GetMethod("Update", new Type[] {typeof(GameTime), typeof(TimeSpan), typeof(float)}).GetDeclaredMember();
            var updatePreAction = new HarmonyMethod(
                patcherType.GetMethod("PreUpdate").GetDeclaredMember()
            );
            var updatePostAction = new HarmonyMethod(
                patcherType.GetMethod("PostUpdate").GetDeclaredMember()
            );
            Patcher.Patch(updateOriginal, updatePreAction, updatePostAction);

            var completeOriginal = type.GetRuntimeMethod("OnCompleted", new Type[] {typeof(CompletionType)}).GetDeclaredMember();
            var completePreAction = new HarmonyMethod(
                patcherType.GetMethod("PreComplete").GetDeclaredMember()
            );
            var completePostAction = new HarmonyMethod(
                patcherType.GetMethod("PostComplete").GetDeclaredMember()
            );
            Patcher.Patch(completeOriginal, completePreAction, completePostAction);
        }
        public delegate void UpdateHandler(ActionType action, GameTime time, TimeSpan passedInGame, float speedMultiplier);
        public delegate void CompletionHandler(ActionType action, CompletionType type);
        public event CompletionHandler BeforeCompleted;
        public event CompletionHandler AfterCompleted;
        public event UpdateHandler BeforeUpdate;
        public event UpdateHandler AfterUpdate;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PreComplete(ActionType __instance, CompletionType type) {
            BeforeCompleted(__instance, type);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PostComplete(ActionType __instance, CompletionType type) {
            AfterCompleted(__instance, type);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PreUpdate(ActionType __instance, GameTime time, TimeSpan passedInGame, float speedMultiplier) {
            BeforeUpdate(__instance, time, passedInGame, speedMultiplier);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PostUpdate(ActionType __instance, GameTime time, TimeSpan passedInGame, float speedMultiplier) {
            AfterUpdate(__instance, time, passedInGame, speedMultiplier);
        }
    }
}