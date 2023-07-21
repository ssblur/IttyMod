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
    [DataContract()]
    class BitManager {
        public static BitManager INSTANCE;
        public static void OnSaveDataCreated(GameImpl gameImpl, SaveHandler.SaveData saveData) {
            if(INSTANCE == null)
                INSTANCE = new BitManager();
            saveData.SetData("Itty.BitManager", INSTANCE);
        }

        public static void OnSaveDataLoaded(GameImpl gameImpl, SaveHandler.SaveData saveData) {
            IttyMod.Logger.Info("Loading BitManager...");
            var instance = saveData.GetData<BitManager>("Itty.BitManager");
            if(instance == null)
                instance = new BitManager();
            INSTANCE = instance;
        }

        [DataMember()] public Queue<Bit> Bits { get; set; }
        static Random random = new Random();
        private BitManager() {
            Bits = new Queue<Bit>();
        }

        public static void AddBit(Bit bit) {
            if(INSTANCE == null) {
                IttyMod.Logger.Warn("BitManager not instantiated. Was there an error during loading?");
                return;
            }

            INSTANCE.Bits.Enqueue(bit);
            if(INSTANCE.Bits.Count > 64) INSTANCE.Bits.Dequeue();
            try {
                OnBitPublished(bit);
            } catch (NullReferenceException e) {
                IttyMod.Logger.Error(e);
            }
        }

        public void AddReaction() {
            if(this.Bits.Count == 0) return;
            
            var random = new Random();
            var item = random.NextInt64(this.Bits.Count);
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
                INSTANCE.AddReaction();
        }

        public delegate void BitPublishedHandler(Bit bit);
        public static event BitPublishedHandler OnBitPublished;
    }
}