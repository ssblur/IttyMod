using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using TinyLife.Objects;

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

        public readonly static Regex _TrickMagicRegex = new Regex("\\{\\!(.*?)\\!\\}");
        public readonly static Regex _TrickStringRegex = new Regex("\\{\\!(.*?)\\:(.*)\\!\\}");
        
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


        public static Bit GenerateBit(String content, Person creator, MapObject[] involved) {
            bool shouldReturn = true;
            string value = _TrickMagicRegex.Replace(content, match => {
                if(_TrickStringRegex.IsMatch(match.Value))
                    return _TrickStringRegex.Replace(match.Value, stringMatch => {
                        if(INSTANCE.Registry.ContainsKey(stringMatch.Groups[1].Value)){
                            var result = INSTANCE.Registry[stringMatch.Groups[1].Value].parse(stringMatch.Groups[2].Value, creator, involved.ToList());
                            if(result == false)
                                throw new BitCancelledException();
                            else if(result == true) {
                                shouldReturn = false;
                                return null;
                            } else {
                                return result.substitution;
                            }
                        } else {
                            return null;
                        }
                    });
                var result = INSTANCE.Registry[match.Groups[1].Value].parse("", creator, involved.ToList());
                if(result == false)
                    throw new BitCancelledException();
                else if(result == true) {
                    shouldReturn = false;
                    return null;
                } else
                    return result.substitution;
            });

            if(shouldReturn) return new Bit(value, creator, involved);
            else throw new BitCancelledException();
        }

        public static Bit GenerateFormattedBit(string category, IttyMod.BitDisposition disposition, Person creator, params MapObject[] involved) {
            try {
                string text = IttyMod.LocalizedBit(category, disposition);
                return GenerateBit(text, creator, involved);
            } catch(BitCancelledException) {
                return null;
            }
        }
    }
}