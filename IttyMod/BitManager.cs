#nullable enable
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using TinyLife;

namespace IttyMod {
    /// <summary>
    /// A monomer for managing bits per save.
    /// Theoretically, it could be useful to construct this per save, 
    /// but there should only be one open at once.
    /// Plus, this makes it easy to access.
    /// </summary>
    [DataContract]
    class BitManager {
        public static BitManager? Instance;
        public static void OnSaveDataCreated(GameImpl gameImpl, SaveHandler.SaveData saveData) {
            IttyMod.Logger.Info("Creating BitManager...");
            Instance ??= new BitManager();
            saveData.SetData("Itty.BitManager", Instance);
        }

        public static void OnSaveDataLoaded(GameImpl gameImpl, SaveHandler.SaveData saveData) {
            IttyMod.Logger.Info("Loading BitManager...");
            var instance = saveData.GetData<BitManager>("Itty.BitManager") ?? new BitManager();
            Instance = instance;
        }

        [DataMember()] public Queue<Bit> Bits { get; set; }
        static Random random = new();
        private BitManager() {
            Bits = new Queue<Bit>();
        }

        public static void AddBit(Bit bit) {
            if(Instance == null) {
                IttyMod.Logger.Warn("BitManager not instantiated. Was there an error during loading?");
                return;
            }

            Instance.Bits.Enqueue(bit);
            if(Instance.Bits.Count > 64) Instance.Bits.Dequeue();
            try {
                OnBitPublished(bit);
            } catch (NullReferenceException e) {
                IttyMod.Logger.Error(e);
            }
        }

        public void AddReaction() {
            if(Bits.Count == 0) return;
            
            var random = new Random();
            var item = random.NextInt64(Bits.Count);
            var bit = this.Bits.ToArray()[item];
            var reaction = random.NextInt64(bit.reactions.Length);
            var reroll = 0;

            // Reactions after the third should be rare
            while(reaction >= 3 && reroll < 2) {
                reroll++;
                reaction = random.NextInt64(bit.reactions.Length);
            }

            bit.reactions[reaction] = Math.Min(9, bit.reactions[reaction] + 1);
        }

        public static void AddReactionHook() {
            if(random.NextInt64(50) == 0) 
                Instance?.AddReaction();
        }

        public delegate void BitPublishedHandler(Bit bit);
        public static event BitPublishedHandler OnBitPublished;
    }
}