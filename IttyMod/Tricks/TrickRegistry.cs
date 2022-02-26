using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace IttyMod.Tricks {
    class BitCancelledException : Exception {
        public BitCancelledException() : base("Bit creation cancelled by a Trick.") {}
    }
    class TrickRegistry {
        public static TrickRegistry _InstanceInternal;
        public static TrickRegistry INSTANCE {
            get {
                if(_InstanceInternal is not null) 
                    return _InstanceInternal;
                return _InstanceInternal = new TrickRegistry();
                
            }
            private set {}
        }

        public readonly static Regex _TrickMagicRegex = new Regex("/\\{\\!(.*?)\\!\\}/g");
        public readonly static Regex _TrickStringRegex = new Regex("/(.*?)\\:(.*)/g");
        
        private Dictionary<string, Trick> Registry = new();

        public static void Register(string key, Trick trick, bool overwrite = false) {
            if(INSTANCE.Registry.ContainsKey(key)) {
                if(overwrite) 
                    INSTANCE.Registry[key] = trick;
                else
                    throw new AccessViolationException(String.Format("Cannot register trick to key {0}, key is already taken!", key));
            } else 
                INSTANCE.Registry[key] = trick;
        }


        public static string Format(Bit bit) {
            bool shouldReturn = true;
            string value = _TrickMagicRegex.Replace(bit.content, match => {
                return _TrickStringRegex.Replace(match.Value, stringMatch => {
                    if(INSTANCE.Registry.ContainsKey(match.Captures[1].Value)){
                        var result = INSTANCE.Registry[match.Captures[1].Value].parse(match.Captures[2].Value, bit.creator, bit.involved);
                        if(result == false)
                            throw new BitCancelledException();
                        else if(result == true) {
                            shouldReturn = false;
                            return "";
                        } else {
                            return result.substitution;
                        }
                    } else {
                        return "";
                    }
                });
            });

            if(shouldReturn) return value;
            else throw new BitCancelledException();
        }
    }
}