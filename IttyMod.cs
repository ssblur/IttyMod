using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System;
using ExtremelySimpleLogger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Data;
using MLEM.Data.Content;
using MLEM.Textures;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Mods;
using TinyLife.Objects;
using TinyLife.Tools;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Content;
using IttyMod.UIs;

namespace IttyMod {
    public class IttyMod : Mod {
        public static readonly CanExecuteResult WaitingRequired = new CanExecuteResult("mustwait");

        // the logger that we can use to log info about this mod
        public static Logger Logger { get; private set; }

        // visual data about this mod
        public override string Name => "Itty";
        public override string Description => "Brings your Tiny Life online!\nThis might be a mistake...";
        public override string IssueTrackerUrl => "https://github.com/ssblur/IttyMod/issues";
        public override string TestedVersionRange => "[0.43.6,0.43.10]";

        public override TextureRegion Icon => IttyMod.uiTextures[0, 0];
        public override bool RequiresHarmony => false;

        public static UniformTextureAtlas uiTextures;
        public static ModInfo Info;

        private static RawContentManager Manager;
        private static Random Generator;
        private static string LoadedLang;
        private static JObject _InternalLang;

    

        // A JObject which contains localized Bit arrays.
        // Set up this way to notice language changes.
        protected static JObject Lang {
            get {
                if(LoadedLang == TinyLife.Options.Instance.Language)
                    return _InternalLang;
                LoadedLang = TinyLife.Options.Instance.Language;
                
                try 
                { 
                    return _InternalLang = Manager.Load<JObject>("Localization/Itty/" + TinyLife.Options.Instance.Language);
                } 
                catch (ContentLoadException e) when (LoadedLang != "En") // Load English if the existing lang can't be found.
                {
                    Logger.Error(e);
                    return _InternalLang = Manager.Load<JObject>("Localization/Itty/En");
                }
                catch (ContentLoadException e) { // If English is gone for some reason, don't crash pls.
                    Logger.Error(e);
                    return null;
                }
            }
        }

        public override void AddGameContent(GameImpl game, ModInfo info) {
            // People can Bit about other people.
            ActionType.Register(new ActionType.TypeSettings(
                "IttyMod.BitPerson", 
                ObjectCategory.People, 
                typeof(Actions.PersonBitAction)
                ) {
                CanExecute = (info, automatic) => CanExecuteResult.Valid,
                Ai = {
                    CanDoRandomly = true,
                    PassivePriority = p => 20
                },
                Texture = uiTextures[1, 0]
            });

            // People can just post randomly.
            ActionType.Register(new ActionType.TypeSettings(
                "IttyMod.Bit", 
                ObjectCategory.Self, 
                typeof(Actions.GeneralBitAction)
                ) {
                CanExecute = (info, automatic) => {
                    TimeSpan? time = info.Person.GetData<TimeSpan>("randomBitTimer");
                    if(time == null || time < info.Person.Map.Time)
                        return CanExecuteResult.Valid;
                    return WaitingRequired;
                },
                Ai = {
                    CanDoRandomly = true,
                    PassivePriority = p => 10
                },
                Texture = uiTextures[1, 0]
            });
            
            
            SaveHandler.OnSaveDataCreated += BitManager.OnSaveDataCreated;
            SaveHandler.OnSaveDataLoaded += BitManager.OnSaveDataLoaded;
            game.UiSystem.OnRootAdded += IttyUI.RootHandler;
            TinyLife.SaveHandler.OnGameLoaded += (game, phase) => {
                if(phase == EventPhase.Post)
                    game.OnFinishedLoading += () => {
                        foreach(var map in game.CurrentMaps)
                            game.CurrentMap.OnUpdate += (_, _, _, _) => BitManager.AddReactionHook();
                    };
            };

            Person.OnEventsAttachable += mapObject => {
                if(mapObject is Person person)
                    person.OnActionsCompleted += (action, completion, isAuxilliary) => {
                        if(action is DieAction die) 
                            Events.DeathEvent.OnActionsCompleted(completion, die);
                    };
            };
        }

        public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker, ModInfo info) {
            Logger = logger;
            Manager = content;
            Info = info;
            Generator = new Random();

            texturePacker.Add(content.Load<Texture2D>("UiTextures"), r => IttyMod.uiTextures = new UniformTextureAtlas(r, 8, 8));

            Tricks.TrickRegistry.Register("test", new Tricks.TestTrick());
            Tricks.TrickRegistry.Register("first", new Tricks.FirstNameTrick());
            Tricks.TrickRegistry.Register("last", new Tricks.LastNameTrick());
            Tricks.TrickRegistry.Register("@first", new Tricks.TargetFirstNameTrick());
            Tricks.TrickRegistry.Register("@last", new Tricks.TargetLastNameTrick());
            Tricks.TrickRegistry.Register("@tag", new Tricks.TargetTagTrick());
            Tricks.TrickRegistry.Register("nopost", new Tricks.NoPostTrick());
        }

        // Lets you select a specific disposition for your Bit.
        public enum BitDisposition { 
            POSITIVE,
            NEGATIVE,
            NEUTRAL
        }

        /// <summary>
        /// Generates a localized Bit based on type and disposition.
        /// </summary>
        /// <param name="key">The key name for the type of bit being generated.</param>
        /// <param name="disposition">The user's disposition towards this bit</param>
        public static string LocalizedBit(string key, BitDisposition disposition)
        {
            if(Lang != null)
            {
                string polarity = "Neutral";
                if(disposition == BitDisposition.POSITIVE)
                    polarity = "Positive";
                else if (disposition == BitDisposition.NEGATIVE) 
                    polarity = "Negative";
                string[] array = Lang.SelectToken(key + "." + polarity)?.ToObject<string[]>();

                if(array == null) return "honestly at a loss for words...";
                
                return array[Generator.Next(0, array.Length)];
            }

            return "feelin gprety.. .might delete l8r..,";
        }
    }
}