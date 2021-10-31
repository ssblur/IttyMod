using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using TinyLife.Objects;
using GameImpl = TinyLife.GameImpl;

namespace IttyMod {
    public class Bit {
        /// <summary>The creator of a Bit</summary>
        [JsonIgnore] public Person creator {
            get {
                return TinyLife.GameImpl.Instance.Map.GetObject<Person>(creatorGuid);
            }
        }
        /// <summary>All people and objects which are mentioned in this bit.</summary>
        [JsonIgnore] public List<MapObject> involved {
            get {
                List<MapObject> list = new List<MapObject>();
                involvedGuids.ForEach(delegate(Guid guid){
                    list.Add(TinyLife.GameImpl.Instance.Map.GetObject<MapObject>(guid));
                });
                return list;
            }
        }
        /// <summary>The text contents of a Bit</summary>
        public string content {private set; get;}

        private Guid creatorGuid;
        private List<Guid> involvedGuids;
        #nullable enable
        public Bit(string contents, Person? creator, params MapObject[] objects) {
            content = contents;

            creatorGuid = creator!.Id;
            
            involvedGuids = new List<Guid>();
            foreach(MapObject obj in objects) {
                involvedGuids.Add(obj.Id);
            }
        }
    }
}