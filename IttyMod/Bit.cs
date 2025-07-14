using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Linq;
using TinyLife.Objects;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;
using TinyLife;

namespace IttyMod {

    [DataContract()]
    public class Bit {
        #nullable enable
        static T? GetMapObject<T>(GameImpl gameImpl, Guid guid) where T: MapObject
        {
            return gameImpl.CurrentMaps.Values.Select(map => map.GetObject<T>(guid)).FirstOrDefault();
        }

        static MapObject? GetMapObject(GameImpl gameImpl, Guid guid) {
            return GetMapObject<MapObject>(gameImpl, guid);
        }


        /// <summary>
        ///     The creator of a Bit
        ///     This may be undefined for "sponsored" bits.
        /// </summary>
        public Person? creator => 
            creatorGuid != null ? GetMapObject<Person>(GameImpl.Instance, creatorGuid.Value) : null;

        public string nameTag
        {
            get
            {
                if (creator != null) tagCache = $"@{creator.FirstName}{creator.LastName}";
                return tagCache ?? "Deactivated";
            }
        }

        public string pronouns
        {
            get
            {
                if (creator != null) pronounCache = creator?.Pronouns;
                return pronounCache ?? "any/all";
            }
        }

        public string mapName
        {
            get
            {
                if (creator?.Map != null) mapNameCache = creator.Map.Info.DisplayName;
                return mapNameCache ?? "Somewhere";
            }
        }

        /// <summary>All people and objects which are mentioned in this bit.</summary>
        public List<MapObject> involved {
            get {
                return involvedGuids.Where(delegate(Guid guid){
                    var obj = GetMapObject(GameImpl.Instance, guid);
                    return obj is not null;
                }).Select(guid => GetMapObject(GameImpl.Instance, guid)!).ToList();
            }
        }
        /// <summary>The text contents of a Bit</summary>
        [DataMember] public string content {private set; get;}

        [DataMember] public Guid? creatorGuid;
        [DataMember] public List<Guid> involvedGuids;
        [DataMember] public int[] reactions;
        
        [DataMember] string? tagCache;
        [DataMember] string? pronounCache;
        [DataMember] string? mapNameCache;
        public Bit(string contents, PersonLike? creator, params MapObject[] objects) {
            content = contents;

            creatorGuid = creator?.Id;
            
            involvedGuids = [];
            foreach(var obj in objects)
                involvedGuids.Add(obj.Id);
            reactions = new int[6];
        }

        [JsonConstructor]
        public Bit(string content, Guid? creatorGuid, List<Guid> involvedGuids, int[] reactions) {
            this.content = content;
            this.creatorGuid = creatorGuid;
            this.involvedGuids = involvedGuids;
            
            Array.Resize(ref reactions, 6);
            this.reactions = reactions;
        }

        public Bit Format(params object[] args) {
            content = string.Format(content, args);
            return this;
        }
    }
}