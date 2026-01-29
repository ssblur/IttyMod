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
        private static readonly CanExecuteResult WaitingRequired = new("mustwait");

        // the logger that we can use to log info about this mod
        public static Logger Logger { get; private set; }

        // visual data about this mod
        public override string Name => "Itty";
        public override string Description => "Brings your Tiny Life online!\nThis might be a mistake...";
        public override string IssueTrackerUrl => "https://github.com/ssblur/IttyMod/issues";
        public override string TestedVersionRange => "[0.47.10,0.49.1]";
        public override string Version => "1.3.0";

        public override TextureRegion Icon => UiTextures[0, 0];
        public override bool RequiresHarmony => false;

        public static UniformTextureAtlas UiTextures;
        public static ModInfo Info;

        private static RawContentManager _manager;
        private static Random _generator;
        private static string _loadedLang;
        private static JObject _internalLang;

    

        // A JObject which contains localized Bit arrays.
        // Set up this way to notice language changes.
        protected static JObject Lang {
            get {
                if(_loadedLang == Options.Instance.Language)
                    return _internalLang;
                _loadedLang = Options.Instance.Language;
                
                try 
                { 
                    return _internalLang = _manager.Load<JObject>("Localization/Itty/" + Options.Instance.Language);
                } 
                catch (ContentLoadException e) when (_loadedLang != "En") // Load English if the existing lang can't be found.
                {
                    Logger.Error(e);
                    return _internalLang = _manager.Load<JObject>("Localization/Itty/En");
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
                    PassivePriority = p => 40
                },
                Texture = UiTextures[1, 0]
            });

            // People can just post randomly.
            ActionType.Register(new ActionType.TypeSettings(
                "IttyMod.Bit", 
                ObjectCategory.Self, 
                typeof(Actions.GeneralBitAction)
                ) {
                CanExecute = (info, automatic) => {
                    var time = info.Person.GetData<TimeSpan?>("randomBitTimer");
                    if (!automatic || time == null) return CanExecuteResult.Valid;
                    return time < info.Person.Map.Time ? CanExecuteResult.Valid : WaitingRequired;
                },
                Ai = {
                    CanDoRandomly = true,
                    PassivePriority = p => 1
                },
                Texture = UiTextures[1, 0]
            });
            
            SaveHandler.OnSaveDataCreated += BitManager.OnSaveDataCreated;
            SaveHandler.OnSaveDataLoaded += BitManager.OnSaveDataLoaded;
            
            game.UiSystem.OnRootAdded += IttyUI.RootHandler;
            SaveHandler.OnGameLoaded += (game, phase) => {
                if(phase == EventPhase.Post)
                    game.OnFinishedLoading += () => {
                        foreach(var (_, map) in game.CurrentMaps)
                            map.OnUpdate += (_, _, _, _) => BitManager.AddReactionHook();
                    };
            };

            MapObject.OnEventsAttachable += mapObject => {
                if(mapObject is Person person)
                    person.OnActionsCompleted += (action, completion, isAuxilliary) => {
                        if(action is DieAction die) 
                            Events.DeathEvent.OnActionsCompleted(completion, die);
                    };
            };
        }

        public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker, ModInfo info) {
            Logger = logger;
            _manager = content;
            Info = info;
            _generator = new Random();

            texturePacker.Add(content.Load<Texture2D>("UiTextures"), r => IttyMod.UiTextures = new UniformTextureAtlas(r, 8, 8));

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
            Positive,
            Negative,
            Neutral
        }

        /// <summary>
        /// Generates a localized Bit based on type and disposition.
        /// </summary>
        /// <param name="key">The key name for the type of bit being generated.</param>
        /// <param name="disposition">The user's disposition towards this bit</param>
        public static string LocalizedBit(string key, BitDisposition disposition)
        {
            if (Lang == null) return "what is this, some kind of placeholder???";
            var polarity = disposition switch
            {
                BitDisposition.Positive => "Positive",
                BitDisposition.Negative => "Negative",
                _ => "Neutral"
            };
            var array = Lang.SelectToken(key + "." + polarity)?.ToObject<string[]>();

            return array == null ? "honestly at a loss for words..." : array[_generator.Next(0, array.Length)];

        }
    }
}