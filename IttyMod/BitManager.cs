using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace IttyMod {
    /// <summary>
    /// A monomer for managing bits per save.
    /// Theoretically, it could be useful to construct this per save, 
    /// but there should only be one open at once.
    /// Plus, this makes it easy to access.
    /// </summary>
    [DataContract()]
    class BitManager {
        [DataMember()] public Queue<Bit> Bits { get; set; }
        static Random random = new Random();
        private BitManager() {
            Bits = new Queue<Bit>();
        }

        public static void AddBit(Bit bit) {
            var instance = Load();
            instance.Bits.Enqueue(bit);
            if(instance.Bits.Count > 64) instance.Bits.Dequeue();
            OnBitPublished(bit);
            instance.Save();
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
                Load().AddReaction();
        }

        public void Save() {
            TinyLife.GameImpl.Instance.Map.SetData("Itty.BitManager", this);
        }

        public static BitManager Load() {
            var instance = TinyLife.GameImpl.Instance.Map.GetData<BitManager>("Itty.BitManager");
            if(instance == null)
                return new BitManager();
            return instance;
        }

        public delegate void BitPublishedHandler(Bit bit);
        public static event BitPublishedHandler OnBitPublished;
    }
}