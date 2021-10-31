using System.Collections.Generic;
using System;

namespace IttyMod {
    /// <summary>
    /// A monomer for managing bits per save.
    /// Theoretically, it could be useful to construct this per save, 
    /// but there should only be one open at once.
    /// Plus, this makes it easy to access.
    /// </summary>
    [Serializable()]
    class BitManager {
        public static BitManager INSTANCE = new BitManager();

        public Queue<Bit> Bits { get; private set; }
        private BitManager() {
            Bits = new Queue<Bit>();
        }

        public void AddBit(Bit bit) {
            Bits.Enqueue(bit);
            if(Bits.Count > 64) Bits.Dequeue();
            OnBitPublished(bit);
        }

        public delegate void BitPublishedHandler(Bit bit);
        public event BitPublishedHandler OnBitPublished;
    }
}