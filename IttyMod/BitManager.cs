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