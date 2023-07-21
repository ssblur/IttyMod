using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Linq;
using TinyLife.Objects;
using System.Runtime.Serialization;
using TinyLife;

namespace IttyMod {

    [DataContract()]
    public class Bit {
        #nullable enable
        static T? GetMapObject<T>(GameImpl gameImpl, Guid guid) where T: MapObject{
            foreach(var map in gameImpl.CurrentMaps.Values) {
                var mapObject = map.GetObject<T>(guid);
                if(mapObject != null)
                    return mapObject;
            }
            return null;
        }

        static MapObject? GetMapObject(GameImpl gameImpl, Guid guid) {
            return GetMapObject<MapObject>(gameImpl, guid);
        }


        /// <summary>
        ///     The creator of a Bit
        ///     This may be undefined for "sponsored" bits.
        /// </summary>
        public Person? creator {
            get {
                return creatorGuid != null ? GetMapObject<Person>(GameImpl.Instance, creatorGuid.Value) : null;
            }
        }
        /// <summary>All people and objects which are mentioned in this bit.</summary>
        public List<MapObject> involved {
            get {
                List<MapObject> list = new List<MapObject>();
                involvedGuids.ForEach(delegate(Guid guid){
                    MapObject? obj = GetMapObject(GameImpl.Instance, guid);
                    if(obj is not null)
                        list.Add(obj);
                });
                return list;
            }
        }
        /// <summary>The text contents of a Bit</summary>
        [DataMember()] public string content {private set; get;}

        [DataMember()] public Guid? creatorGuid;
        [DataMember()] public List<Guid> involvedGuids;
        [DataMember()] public int[] reactions;
        public Bit(string contents, Person? creator, params MapObject[] objects) {
            content = contents;

            creatorGuid = creator != null ? creator.Id : null;
            
            involvedGuids = new List<Guid>();
            if(objects != null)
                foreach(MapObject obj in objects)
                    involvedGuids.Add(obj.Id);
            reactions = new int[6];
        }

        [JsonConstructor()]
        public Bit(string content, Guid? creatorGuid, List<Guid> involvedGuids, int[] reactions) {
            this.content = content;
            this.creatorGuid = creatorGuid;
            this.involvedGuids = involvedGuids;
            if(reactions != null) {
                Array.Resize<int>(ref reactions, 6);
                this.reactions = reactions;
            } else
                this.reactions = new int[6];
        }

        public Bit Format(params object[] args) {
            content = String.Format(content, args);
            return this;
        }
    }
}